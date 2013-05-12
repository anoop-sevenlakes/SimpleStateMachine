using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateMachineActionContext
    {
        public IStateMachineContext WorkflowContext { get; set; }
        public IStateMachineAction Action { get; set; }
        public IStateTransition CurrentTransition { get; set; }
        public StateTransitionPhase CurrentPhase { get; set; }
    }
}