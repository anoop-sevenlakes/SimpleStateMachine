using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entropy.SimpleStateMachine.Interfaces
{
    public interface IStateEventActionManager
    {
        IStateMachineEventAction AddNewEventAction(string actionTaskName, IStateMachineEvent triggerEvent,
                                                   params object[] args);
    }
}
