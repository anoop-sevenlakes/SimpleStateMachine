using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entropy.SimpleStateMachine.Interfaces
{
    public interface IStateMachineEventAction : IStateMachineAction
    {
        IStateMachineEvent TriggerEvent { get;  }
        bool Matches(IStateMachineContext context, object eventIdentifier);
    }
}
