using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateChangingEventArgs : StateMachineEventArgs
    {
        public IStateMachineState NewState { get; set; }
        public bool Cancel { get; set; }
    }
}