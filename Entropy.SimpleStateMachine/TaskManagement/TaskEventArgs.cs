using System;

namespace Entropy.SimpleStateMachine.TaskManagement
{
    public delegate void TaskEventHandler(object sender, TaskEventArgs args);

    /// <summary>
    /// Summary description for TaskEventArgs.
    /// </summary>
    public class TaskEventArgs
    {
        private readonly ITaskContext _context;
        private readonly ITask _task;
        private readonly object _taskResult;

        public TaskEventArgs(ITask task, object result)
        {
            _task = task;
            _taskResult = result;
        }

        public TaskEventArgs(ITask task, ITaskContext context, object result)
        {
            _task = task;
            _context = context;
            _taskResult = result;
        }

        public ITask Task
        {
            get { return _task; }
        }

        public object TaskResult
        {
            get { return _taskResult; }
        }

        public ITaskContext TaskExecutionContext
        {
            get { return _context; }
        }

        public bool ResultIsException
        {
            get { return _taskResult != null && _taskResult is Exception; }
        }
    }
}