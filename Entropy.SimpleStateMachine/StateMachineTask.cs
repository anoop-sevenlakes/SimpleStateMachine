using Entropy.SimpleStateMachine.TaskManagement;
using Entropy.SimpleStateMachine.TaskManagement.Tasks;

namespace Entropy.SimpleStateMachine
{
    public abstract class StateMachineTask : TaskBase
    {
        protected override object PerformTask(TaskContext context)
        {
            if (context.ContextObject is StateMachineActionContext)
            {
                return PerformStateMachineTask((StateMachineActionContext) context.ContextObject);
            }
            return null;
        }

        protected abstract object PerformStateMachineTask(StateMachineActionContext context);
    }
}