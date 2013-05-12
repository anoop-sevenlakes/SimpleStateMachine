using System;
using Entropy.SimpleStateMachine;
using Entropy.SimpleStateMachine.TaskManagement;
using StateMachineDemo.Domain;

namespace StateMachineDemo.Tasks
{
    [TaskDescriptor("WriteToHistory")]
    public class WriteToHistoryTask : StateMachineTask
    {
        protected override object PerformStateMachineTask(StateMachineActionContext context)
        {
            var wrapper = context.WorkflowContext.DomainContext as ReflectiveDomainContextWrapper;
            if (wrapper != null)
            {
                string prefix = String.Empty;
                if (context.Action.ActionArgs != null && context.Action.ActionArgs.Length > 0 &&
                    context.Action.ActionArgs[0] != null)
                {
                    prefix = context.Action.ActionArgs[0] + ": ";
                }

                var order = wrapper.DomainContext as Order;

                string stateName;

                if (context.WorkflowContext.CurrentState == null)
                    stateName = "<NULL STATE>";
                else
                {
                    stateName = context.WorkflowContext.CurrentState.StateName;
                }

                if (order != null)
                    order.MakeHistory(prefix + context.CurrentPhase + " => " + stateName);
            }
            return true;
        }
    }
}