namespace Entropy.SimpleStateMachine.TaskManagement
{
    /// <summary>
    /// Summary description for DefaultTaskContext.
    /// </summary>
    public class TaskContext : ITaskContext
    {
        public TaskContext()
        {
        }

        public TaskContext(object contextObject) : this(contextObject, null)
        {
        }

        public TaskContext(object contextObject, params object[] taskArgs)
        {
            ContextObject = contextObject;
            TaskArgs = taskArgs;
        }

        #region ITaskContext Members

        public object ContextObject { get; set; }

        public object[] TaskArgs { get; set; }

        #endregion
    }
}