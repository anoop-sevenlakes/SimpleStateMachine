using System;
using System.Collections.Generic;
using Entropy.SimpleStateMachine;
using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateMachineState : StateMachineComponent, IStateMachineStateBuilder
    {
        public StateMachineState(IStateMachine parent, string stateName) : this(parent, stateName, null)
        {
        }

        public StateMachineState(IStateMachine parent, string stateName, object stateIdentifier) : base(parent)
        {
            if (String.IsNullOrEmpty("stateName"))
                throw new ArgumentNullException("stateName");
            StateName = stateName;

            TransitionList = new List<IStateTransition>();
            StateTransitionActionList = new List<IStateTransitionAction>();
            EventActionList = new List<IStateMachineEventAction>();
            StateIdentifier = stateIdentifier;
        }

        protected List<IStateTransitionAction> StateTransitionActionList { get; private set; }
        protected List<IStateTransition> TransitionList { get; private set; }
        protected List<IStateMachineEventAction> EventActionList { get; private set; }

        #region IStateMachineStateBuilder Members

        public string StateName { get; private set; }
        public object StateIdentifier { get; private set; }

        public IStateTransition[] Transitions
        {
            get { return TransitionList.ToArray(); }
        }

        public IStateTransitionAction[] StateTransitionActions
        {
            get { return StateTransitionActionList.ToArray(); }
        }

        public IStateMachineEventAction[] EventActions { get { return EventActionList.ToArray(); } }

        public bool Matches(object stateIdentifier)
        {
            if (stateIdentifier == null)
                return false;

            if (stateIdentifier is IStateMachineState)
                return StateName.Equals(((IStateMachineState) stateIdentifier).StateName);

            if (stateIdentifier is string)
                return ((string) stateIdentifier).Equals(StateIdentifier.ToString(),
                                                         StringComparison.CurrentCultureIgnoreCase);

            if (StateIdentifier is string)
                return ((string) StateIdentifier).Equals(stateIdentifier.ToString(),
                                                         StringComparison.CurrentCultureIgnoreCase);

            if (StateIdentifier is IComparable)
                return ((IComparable) StateIdentifier).CompareTo(stateIdentifier) == 0;


            return StateIdentifier.Equals(stateIdentifier);
        }

        public virtual IStateTransition AddNewTransition(IStateMachineEvent trigger, IStateMachineState destination, List<string> rolesRequired)
        {
            var transition = new StateTransition(this, trigger, destination, rolesRequired);
            TransitionList.Add(transition);
            return transition;
        }

        public virtual IStateTransition AddNewTransition(IStateMachineEvent trigger, IStateMachineState destination)
        {
            var transition = new StateTransition(this, trigger, destination);
            TransitionList.Add(transition);
            return transition;
        }

        public virtual IStateTransitionAction AddNewTransitionAction(string actionTaskName,
                                                                     StateTransitionPhase targetPhase,
                                                                     params object[] actionArgs)
        {
            var action = new StateTransitionAction(StateMachine, actionTaskName, targetPhase, actionArgs);
            StateTransitionActionList.Add(action);
            return action;
        }

        public virtual IStateMachineEventAction AddNewEventAction(string actionTaskName,IStateMachineEvent triggerEvent,params object[] actionArgs)
        {
            var action = new StateMachineEventAction(StateMachine, actionTaskName, triggerEvent, actionArgs);
            EventActionList.Add(action);
            return action;
        }

        #endregion

        public override string ToString()
        {
            return StateName;
        }
    }
}