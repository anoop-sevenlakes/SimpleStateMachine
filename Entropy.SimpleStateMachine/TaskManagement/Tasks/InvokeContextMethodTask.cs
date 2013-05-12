using System;
using System.Reflection;

namespace Entropy.SimpleStateMachine.TaskManagement.Tasks
{
    [TaskDescriptor("InvokeMethod", "Invoke Method",
        "Invokes the method named in the first argument on the object passed in as the task context. This task requires both a ContextObject to be present, and that a string is passed in as the first Task Argument that matches a public instance or static method on the Context Object."
        )]
    public class InvokeContextMethodTask : TaskBase
    {
        protected override object PerformTask(TaskContext context)
        {
            if (context != null)
            {
                if (context.ContextObject == null)
                    throw new Exception("No ContextObject was found to invoke a method on.");
                if (context.TaskArgs == null || context.TaskArgs.Length < 1 || context.TaskArgs[0] == null)
                    throw new Exception("No method name was provided.");

                string methodName = context.TaskArgs[0].ToString();
                if (String.IsNullOrEmpty(methodName))
                    throw new Exception("No method name was provided.");

                Type contextType = context.ContextObject.GetType();
                MethodInfo method = contextType.GetMethod(methodName,
                                                          BindingFlags.IgnoreCase | BindingFlags.Public |
                                                          BindingFlags.Instance | BindingFlags.Static);
                if (method != null)
                {
                    return method.Invoke(context.ContextObject, null);
                }

                throw new Exception("The method " + methodName + " was not found on the provided object of type " +
                                    contextType.Name);
            }
            return null;
        }
    }
}