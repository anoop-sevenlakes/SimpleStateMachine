using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateTransition : StateMachineComponent, IStateTransition
    {
        public StateTransition(IStateMachineState parent, IStateMachineEvent triggerEvent,
                               IStateMachineState targetState) : base(parent.StateMachine)
        {
            ParentState = parent;
            TriggerEvent = triggerEvent;
            TargetState = targetState;
        }

        internal StateTransition(IStateMachine stateMachine, IStateMachineState initialState) : base(stateMachine)
        {
            TargetState = initialState;
        }

        #region IStateTransition Members

        public virtual bool Matches(object eventId, IStateMachineContext context)
        {
            if (TriggerEvent != null)
                return TriggerEvent.Matches(eventId);
            return false;
        }

        public IStateMachineState ParentState { get; protected set; }
        public IStateMachineEvent TriggerEvent { get; protected set; }
        public IStateMachineState TargetState { get; protected set; }

        #endregion

        public override string ToString()
        {
            if (TriggerEvent != null)
                return TriggerEvent.EventName + " => " + TargetState.StateName;
            return "=> " + TargetState.StateName;
        }
    }
}