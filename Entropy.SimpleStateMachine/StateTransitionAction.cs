using System;
using Entropy.SimpleStateMachine.Interfaces;
using Entropy.SimpleStateMachine.TaskManagement;

namespace Entropy.SimpleStateMachine
{
    public class StateTransitionAction : StateMachineAction, IStateTransitionAction
    {
        public StateTransitionAction(IStateMachine stateMachine, string actionName, StateTransitionPhase targetPhase,
                                     params object[] actionArgs)
            : base(stateMachine,actionName,actionArgs)
        {
            TargetPhase = targetPhase;
        }


        #region IStateTransitionAction Members

        public StateTransitionPhase TargetPhase { get; set; }

        public virtual bool Matches(IStateMachineContext context, StateTransitionPhase phase)
        {
            return TargetPhase.Matches(phase);
        }

        #endregion
    }
}