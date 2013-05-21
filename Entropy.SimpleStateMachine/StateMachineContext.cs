using System;
using System.Collections.Generic;
using System.Transactions;
using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateMachineContext : StateMachineComponent, IStateMachineContext
    {
        private IStateMachineState _currentState;

        public StateMachineContext(IStateMachine stateMachine, object domainContext)
            : this(stateMachine, domainContext, null)
        {
        }

        public StateMachineContext(IStateMachine stateMachine, object domainContext,IStateMachineContextPersistenceService persistenceService)
            : this(stateMachine, null, domainContext,persistenceService)
        {
        }

        public StateMachineContext(IStateMachine stateMachine, object currentState, object domainContext)
            : this(stateMachine, currentState, domainContext, null)
        {
        }

        public StateMachineContext(IStateMachine stateMachine, object currentState, object domainContext,IStateMachineContextPersistenceService persistenceService)
            : base(stateMachine)
        {
            if (stateMachine == null)
                throw new ArgumentNullException("stateMachine");

            PersistenceService = persistenceService;

            DomainContext = domainContext;

            if (currentState != null)
                CurrentState = StateMachine.FindState(currentState);
        }

        public bool IsTransactional { get; set; }

        public IStateMachineContextPersistenceService PersistenceService { get; set; }

        #region IStateMachineContext Members

        public event EventHandler<StateChangingEventArgs> BeforeStateChanged;
        public event EventHandler<StateChangedEventArgs> AfterStateChanged;

        public event EventHandler<StateTransitionEventArgs> StateFinalized;
        public event EventHandler<StateTransitionEventArgs> StateInitialized;

        public event EventHandler<StateTransitionEventArgs> StateMachineStarted;
        public event EventHandler<StateTransitionEventArgs> StateMachineComplete;


        public IStateMachineState CurrentState
        {
            get { return _currentState; }
            private set
            {
                _currentState = value;
                if (DomainContext != null && DomainContext is IStateMachineDomainContext)
                    ((IStateMachineDomainContext) DomainContext).AcceptNewState(value);
            }
        }

        public object DomainContext { get; private set; }

    	public Guid Id { get; set; }

        public string UserRole { get; set; }

    	public bool IsStarted
        {
            get { return CurrentState != null; }
        }

        public bool IsComplete
        {
            get { return CurrentState != null && CurrentState.Transitions.Length < 1; }
        }

        public void Start()
        {
            if (!IsStarted && StateMachine.States.Length > 0)
            {
                var startTransition = new StateTransition(StateMachine, StateMachine.States[0]);
                PerformTransition(startTransition);
            }
        }

        /// <summary>
        /// Takes an event from the outside world and performs
        /// a state transition if necessary. 
        /// </summary>
        /// <param name="transitionEvent"></param>
        /// <returns></returns>
        public EventResponse HandleEvent(object transitionEvent)
        {
            EnsureStateMachineStarted();

            if (transitionEvent == null)
                return EventResponse.EventNotRegistered;

            IStateMachineEvent evt;

            if (transitionEvent is IStateMachineEvent)
                evt = (IStateMachineEvent) transitionEvent;
            else
            {
                evt = StateMachine.FindEvent(transitionEvent);
                if (evt == null)
                    return EventResponse.EventNotRegistered;
            }

            IStateTransition transition = this.FindFirstMatchingTransition(evt);
            this.FindMatchingTransitions();

            if (transition == null) return EventResponse.EventNotInScope;

            if (IsTransactional)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    EventResponse result = PerformTransition(transition);
                    scope.Complete();
                    return result;
                }
            }
            return PerformTransition(transition);
        }

        public void SetState(IStateMachineState targetState)
        {
            EnsureStateMachineStarted();

            if (targetState.StateMachine != StateMachine)
                throw new ArgumentException(
                    "The target state provided does not belong to the same StateMachine as this StateMachineContext.");
            if (targetState == null)
                throw new ArgumentNullException("targetState");

            var transition = new StateTransition(CurrentState, null, targetState);
            PerformTransition(transition);
        }

        #endregion

        protected virtual EventResponse PerformTransition(IStateTransition transition)
        {

            IStateMachineState prevState = CurrentState;

            if (prevState != null)
            {
                if (OnBeforeStateChanged(transition) == false)
                    return EventResponse.TransitionCancelled;

                FinalizeState(transition);
            }
            else
            {
                StartStateMachine(transition);
            }

            FireEventActions(transition);

            CurrentState = transition.TargetState;

            InitializeState(transition, prevState);

            OnAfterStateChanged(prevState);

            if (IsComplete)
            {
                CompleteStateMachine(transition);
            }

            if (PersistenceService != null)
                PersistenceService.Save(this);

            return EventResponse.TransitionComplete;
        }

        protected virtual void FireEventActions(IStateTransition transition)
        {
            //State Machine initialization
            if (transition.TriggerEvent == null)
                return;

            List<IStateMachineEventAction> actions =
                this.GetMatchingEventActions(transition.TriggerEvent.EventIdentifier);

            if (actions != null)
                actions.ForEach(action => ExecuteAction(action,transition,StateTransitionPhase.All));

        }

        protected virtual void FinalizeState(IStateTransition transition)
        {
            List<IStateTransitionAction> actions =
                this.GetMatchingTransitionActions(StateTransitionPhase.StateFinalization);

            if (actions != null)
                actions.ForEach(action => ExecuteAction(action, transition, StateTransitionPhase.StateFinalization));

            OnStateFinalized(transition);
        }

        protected virtual void InitializeState(IStateTransition transition, IStateMachineState prevState)
        {
            List<IStateTransitionAction> actions =
                this.GetMatchingTransitionActions(StateTransitionPhase.StateInitialization);

            if (actions != null)
                actions.ForEach(action => ExecuteAction(action, transition, StateTransitionPhase.StateInitialization));

            OnStateInitialized(transition, prevState);
        }

        protected virtual void StartStateMachine(IStateTransition transition)
        {
            List<IStateTransitionAction> actions =
                this.GetMatchingTransitionActions(StateTransitionPhase.StateMachineStarting);
            if (actions != null)
            {
                actions.ForEach(action => ExecuteAction(action, transition, StateTransitionPhase.StateMachineStarting));
            }

            OnStateMachineStarted(transition);
        }

        protected virtual void CompleteStateMachine(IStateTransition transition)
        {
            List<IStateTransitionAction> actions =
                this.GetMatchingTransitionActions(StateTransitionPhase.StateMachineComplete);
            if (actions != null)
                actions.ForEach(action => ExecuteAction(action, transition, StateTransitionPhase.StateMachineComplete));

            OnStateMachineComplete(transition);
        }

        protected void ExecuteAction(IStateMachineAction action, IStateTransition transition,
                                     StateTransitionPhase phase)
        {
            var context = new StateMachineActionContext
                              {
                                  WorkflowContext = this,
                                  Action = action,
                                  CurrentTransition = transition,
                                  CurrentPhase = phase
                              };
            action.Execute(context);
        }

        /// <summary>
        /// Returns false if the consumer of the event cancelled the process,
        /// true otherwise.
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected virtual bool OnBeforeStateChanged(IStateTransition transition)
        {
            var args = new StateChangingEventArgs
                           {
                               StateMachineContext = this,
                               NewState = transition.TargetState,
                               Cancel = false
                           };

            EventHandler<StateChangingEventArgs> evt = BeforeStateChanged;
            if (evt != null)
                evt(this, args);

            return !args.Cancel;
        }

        protected virtual void OnAfterStateChanged(IStateMachineState prevState)
        {
            var stateChangedArgs = new StateChangedEventArgs
                                       {
                                           StateMachineContext = this,
                                           PreviousState = prevState
                                       };

            EventHandler<StateChangedEventArgs> evt = AfterStateChanged;
            if (evt != null)
                evt(this, stateChangedArgs);
        }

        protected virtual void OnStateFinalized(IStateTransition transition)
        {
            var transitionArgs = new StateTransitionEventArgs
                                     {
                                         StateMachineContext = this,
                                         StartingState = CurrentState,
                                         Transition = transition,
                                         TransitionPhase = StateTransitionPhase.StateFinalization
                                     };

            EventHandler<StateTransitionEventArgs> stateFinalizedHandler = StateFinalized;
            if (stateFinalizedHandler != null) stateFinalizedHandler(this, transitionArgs);
        }

        protected virtual void OnStateInitialized(IStateTransition transition, IStateMachineState prevState)
        {
            var transitionArgs = new StateTransitionEventArgs
                                     {
                                         StateMachineContext = this,
                                         StartingState = prevState,
                                         Transition = transition,
                                         TransitionPhase = StateTransitionPhase.StateInitialization
                                     };

            EventHandler<StateTransitionEventArgs> stateInitializedHandler = StateInitialized;
            if (stateInitializedHandler != null) stateInitializedHandler(this, transitionArgs);
        }

        protected virtual void OnStateMachineStarted(IStateTransition initialTransition)
        {
            var transitionArgs = new StateTransitionEventArgs
                                     {
                                         StateMachineContext = this,
                                         StartingState = null,
                                         Transition = initialTransition,
                                         TransitionPhase = StateTransitionPhase.All
                                     };

            EventHandler<StateTransitionEventArgs> evt = StateMachineStarted;
            if (evt != null)
                evt(this, transitionArgs);
        }

        protected virtual void OnStateMachineComplete(IStateTransition finalTransition)
        {
            var transitionArgs = new StateTransitionEventArgs
                                     {
                                         StateMachineContext = this,
                                         StartingState = null,
                                         Transition = finalTransition,
                                         TransitionPhase = StateTransitionPhase.All
                                     };

            EventHandler<StateTransitionEventArgs> evt = StateMachineComplete;
            if (evt != null)
                evt(this, transitionArgs);
        }

        private void EnsureStateMachineStarted()
        {
            if (!IsStarted)
                throw new InvalidOperationException(
                    "This state machine workflow has not been started for this context. Please make sure you call Start() before attempting to interact with this StateMachineContext.");
        }
    }
}
