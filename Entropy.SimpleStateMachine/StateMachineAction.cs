using System;
using Entropy.SimpleStateMachine.Interfaces;
using Entropy.SimpleStateMachine.TaskManagement;

namespace Entropy.SimpleStateMachine
{
    public class StateMachineAction : StateMachineComponent, IStateMachineAction
    {
         public StateMachineAction(IStateMachine stateMachine, string actionName, params object[] actionArgs)
            : base(stateMachine)
        {
            if (string.IsNullOrEmpty(actionName))
                throw new ArgumentNullException("actionName");

            ActionName = actionName;
            ActionArgs = actionArgs;
        }

        public TaskFactory TaskFactory { get; set; }

        #region IStateTransitionAction Members

        public string ActionName { get; set; }
        public object[] ActionArgs { get; set; }

      public virtual object Execute(StateMachineActionContext context)
        {
            if (!ActionName.Contains("::"))
                return StateMachine.ActionTaskManager.ExecuteTask(ActionName, context);

            //An alternate task execute syntax is being supported now that will
            //allow any method on a named task to be invoked using the ::, following the syntax
            //TaskName::MethodName

            string taskName = ActionName.Substring(0, ActionName.IndexOf("::"));
            string methodName = ActionName.Substring(ActionName.IndexOf("::") + 2);

            ITask task = StateMachine.ActionTaskManager.CreateTask(taskName);
            if (task != null)
            {
                var wrapper = new CustomExecMethodTaskWrapper(task, methodName);
                return wrapper.Execute(StateMachine.ActionTaskManager.CreateTaskContext(context));
            }

            throw new ArgumentException("A task with the name " + taskName + " could not be found.");
        }

        #endregion
    }
}
