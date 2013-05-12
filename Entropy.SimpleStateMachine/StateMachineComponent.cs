using System;
using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateMachineComponent : IStateMachineComponent
    {
        public StateMachineComponent(IStateMachine stateMachine)
        {
            if (stateMachine == null)
                throw new ArgumentNullException("stateMachine");
            StateMachine = stateMachine;
        }

        #region IStateMachineComponent Members

        public IStateMachine StateMachine { get; private set; }

        #endregion
    }
}