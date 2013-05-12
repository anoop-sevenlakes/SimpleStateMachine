using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Entropy.SimpleStateMachine.TaskManagement.Tasks;

namespace Entropy.SimpleStateMachine.TaskManagement
{
    public class CustomExecMethodTaskWrapper : TaskBase
    {
        private readonly string _execMethodName;
        private MethodInfo _execMethodInfo = null;
        private readonly ITask _targetTask;

        public CustomExecMethodTaskWrapper(ITask targetTask,string execMethodName)
        {
            if (targetTask == null)
                throw new ArgumentNullException("targetTask");

            _targetTask = targetTask;

            if (String.IsNullOrEmpty(execMethodName))
                _execMethodName = "Execute";

            _execMethodName = execMethodName;

            GetMethod();
        }

        private void GetMethod()
        {
                _execMethodInfo = _targetTask.GetType().GetMethod(_execMethodName, new[] {typeof (TaskContext)}) ??
                                  _targetTask.GetType().GetMethod(_execMethodName);
               if (_execMethodInfo == null)
                   throw new ArgumentException("No method by the name of " + _execMethodName +
                                               " could be found on provided task type " + _targetTask.GetType());
        }

        protected override object PerformTask(TaskContext context)
        {
            object result = null;
            if (_execMethodInfo.GetParameters().Length == 1)
                result = _execMethodInfo.Invoke(_targetTask, new object[] {context});
            else
            {
                result = _execMethodInfo.Invoke(_targetTask, null);
            }
            return result;
        }
    }
}
