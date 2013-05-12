using System;
using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateMachineEventArgs : EventArgs
    {
        public IStateMachineContext StateMachineContext { get; set; }
    }
}