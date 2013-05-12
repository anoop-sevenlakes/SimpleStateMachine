using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateChangedEventArgs : StateMachineEventArgs
    {
        public IStateMachineState PreviousState { get; set; }
    }
}