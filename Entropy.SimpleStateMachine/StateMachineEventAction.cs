using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateMachineEventAction : StateMachineAction, IStateMachineEventAction
    {
        public StateMachineEventAction(IStateMachine stateMachine, string actionName, IStateMachineEvent triggerEvent, params object[] actionArgs)
            : base(stateMachine, actionName, actionArgs)
        {
            TriggerEvent = triggerEvent;
        }

        public IStateMachineEvent TriggerEvent { get; set; }

        public bool Matches(IStateMachineContext context, object eventIdentifier)
        {
            return TriggerEvent.Matches(eventIdentifier);
        }
    }
}
