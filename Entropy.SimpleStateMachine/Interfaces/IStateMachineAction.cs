using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entropy.SimpleStateMachine.Interfaces
{
    public interface IStateMachineAction : IStateMachineComponent
    {
        string ActionName { get; }
        object[] ActionArgs { get; }
        object Execute(StateMachineActionContext context);
    }
}
