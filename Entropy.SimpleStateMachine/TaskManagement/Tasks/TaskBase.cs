using System;
using System.Runtime.Serialization;

namespace Entropy.SimpleStateMachine.TaskManagement.Tasks
{
    internal delegate object ExecuteDelegate(ITaskContext context);

    /// <summary>
    /// Summary description for TaskBase.
    /// </summary>
    [Serializable]
    public abstract class TaskBase : ITask
    {
        [NonSerialized] private TaskDescription _taskDescription;

        protected TaskBase()
        {
        }

        protected TaskBase(SerializationInfo info, StreamingContext context)
        {
        }

        public TaskDescription TaskDescription
        {
            get
            {
                if (_taskDescription == null)
                    _taskDescription = TaskDescription.GetDescription(this);
                return _taskDescription;
            }
        }

        #region virtual public properties that can be overridden

        public virtual string TaskFailureDescription
        {
            get
            {
                if (FailingReason == null)
                    return String.Format("Task {0} execution failed.", GetType().Name);

                return String.Format("Task {0} execution failed. Reason: {1}", GetType().Name, FailingReason);
            }
        }

        #endregion

        #region ITask Members

        public object Execute(ITaskContext context)
        {
            try
            {
                //argh, this sucks - WHY I SAY - why not use ITaskContext as the signature? 
                //backwards compatibility compels me not to refactor - but I want to so bad...
                object output = PerformTask(context as TaskContext);
                return output;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion

        #region protected methods/properties

        protected string FailingReason { get; set; }

        protected abstract object PerformTask(TaskContext context);

        #endregion
    }
}