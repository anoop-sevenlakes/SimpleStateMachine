using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateTransitionEventArgs : StateMachineEventArgs
    {
        public IStateMachineState StartingState { get; set; }
        public IStateTransition Transition { get; set; }
        public StateTransitionPhase TransitionPhase { get; set; }
    }
}