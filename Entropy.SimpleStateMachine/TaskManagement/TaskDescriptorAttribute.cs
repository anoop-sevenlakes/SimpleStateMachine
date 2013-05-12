using System;

namespace Entropy.SimpleStateMachine.TaskManagement
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TaskDescriptorAttribute : Attribute
    {
        private readonly TaskDescription _taskDescription = new TaskDescription();

        public TaskDescriptorAttribute()
        {
            _taskDescription.DescriptorAttributeType = GetType();
        }

        public TaskDescriptorAttribute(string taskName) : this(taskName, taskName, String.Empty)
        {
        }

        public TaskDescriptorAttribute(string taskName, string displayName) : this(taskName, displayName, String.Empty)
        {
        }

        public TaskDescriptorAttribute(string taskName, string displayName, string description)
        {
            TaskName = taskName;
            DisplayName = displayName;
            Description = description;
        }

        public TaskDescription TaskDescription
        {
            get { return _taskDescription; }
        }

        public string TaskName
        {
            get { return _taskDescription.Name; }
            set { _taskDescription.Name = value; }
        }

        public string DisplayName
        {
            get { return _taskDescription.DisplayName; }
            set { _taskDescription.DisplayName = value; }
        }

        public string Description
        {
            get { return _taskDescription.Description; }
            set { _taskDescription.Description = value; }
        }

        public Type TaskContextType
        {
            get { return _taskDescription.TaskContextType; }
            set { _taskDescription.TaskContextType = value; }
        }

        public static TaskDescriptorAttribute Find(Type type)
        {
            object[] found = type.GetCustomAttributes(true);
            foreach (Attribute a in found)
            {
                if (typeof (TaskDescriptorAttribute).IsAssignableFrom(a.GetType()))
                    return (TaskDescriptorAttribute) a;
            }
            return null;
        }
    }
}