using System;

namespace Entropy.SimpleStateMachine.TaskManagement
{
    [Serializable]
    public class TaskDescription
    {
        internal TaskDescription()
        {
        }

        public TaskDescription(string name, Type taskType) : this(name, name, taskType)
        {
        }

        public TaskDescription(string name, string displayName, Type taskType)
            : this(name, displayName, displayName, null, taskType)
        {
        }

        public TaskDescription(string name, string displayName, string description, Type taskContextType, Type taskType)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
            TaskContextType = taskContextType;
            TaskType = taskType;
        }

        public string Name { get; internal set; }

        public string DisplayName { get; internal set; }

        public string Description { get; internal set; }

        public Type TaskContextType { get; internal set; }

        public Type TaskType { get; internal set; }

        public Type DescriptorAttributeType { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }

        public TaskContext CreateTaskContext()
        {
            if (TaskContextType == null)
                return new TaskContext();

            if (typeof (TaskContext).IsAssignableFrom(TaskContextType))
                return (TaskContext) Activator.CreateInstance(TaskContextType);

            throw new Exception(
                string.Format(
                    "The TaskContextType for this task description({0} does not inherit from TaskContext.", Name));
        }

        public TaskContext CreateTaskContext(object contextObject)
        {
            return CreateTaskContext(contextObject, null);
        }

        public TaskContext CreateTaskContext(object contextObject, params object[] args)
        {
            if (TaskContextType == null)
                return new TaskContext(contextObject, args);

            if (args == null)
                args = new object[] {};

            var taskArgs = new object[args.Length + 1];
            taskArgs[0] = contextObject;
            if (args.Length > 0)
                args.CopyTo(taskArgs, 1);


            if (typeof (TaskContext).IsAssignableFrom(TaskContextType))
            {
                return (TaskContext) Activator.CreateInstance(TaskContextType, taskArgs);
            }

            throw new Exception(
                string.Format(
                    "The TaskContextType for this task description({0} does not inherit from TaskContext.", Name));
        }

        public static TaskDescription GetDescription(ITask task)
        {
            if (task == null)
                return null;
            Type taskType = task.GetType();

            object[] attributes = taskType.GetCustomAttributes(typeof (TaskDescriptorAttribute), true);
            if (attributes != null && attributes.Length > 0)
                return ((TaskDescriptorAttribute) attributes[0]).TaskDescription;
            return null;
        }
    }
}