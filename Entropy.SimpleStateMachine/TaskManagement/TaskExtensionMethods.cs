using System;

namespace Entropy.SimpleStateMachine.TaskManagement
{
    public static class TaskExtensionMethods
    {
        public static ITaskContext CreateTaskContext(this ITaskExecutionService service)
        {
            return service.CreateTaskContext(null, null);
        }

        public static ITaskContext CreateTaskContext(this ITaskExecutionService service, object contextObject)
        {
            return service.CreateTaskContext(contextObject, null);
        }

        public static object ExecuteTask(this ITaskExecutionService service, string taskName)
        {
            return service.ExecuteTask(taskName, null, null);
        }

        public static object ExecuteTask(this ITaskExecutionService service, string taskName, object contextObject)
        {
            return service.ExecuteTask(taskName, contextObject, null);
        }

        public static object ExecuteTask(this ITaskExecutionService service, string taskName, object contextObject,
                                         params object[] contextArgs)
        {
			if (service.HasTask(taskName))
			{
				TaskDescription description = service.GetTaskDescription(taskName);
				if (description != null)
				{
					ITask task = service.CreateTask(description);
					return service.ExecuteTask(task, contextObject, contextArgs);
				}
			}

			throw new InvalidOperationException(
				string.Format("A task with the TaskName of '{0}' could not be found.", taskName));
        }

        public static object ExecuteTask(this ITaskExecutionService service, ITask task)
        {
            return service.ExecuteTask(task, null);
        }

        public static object ExecuteTask(this ITaskExecutionService service, ITask task, object contextObject)
        {
            return service.ExecuteTask(task, contextObject, null);
        }

        public static object ExecuteTask(this ITaskExecutionService service, ITask task, object contextObject,
                                         params object[] contextArgs)
        {
            TaskDescription description = service.GetTaskDescription(task);
            ITaskContext context;

            if (description == null)
                context = service.CreateTaskContext(contextObject, contextArgs);
            else
                context = description.CreateTaskContext(contextObject, contextArgs);

            return service.ExecuteTask(task, context);
        }

        public static object ExecuteTask(this ITaskExecutionService service, ITask task, ITaskContext context)
        {
            return service.ExecuteTask(task, context, null);
        }

        public static object ExecuteTask<T>(this ITaskExecutionService service)
        {
            return service.ExecuteTask<T>(null);
        }

        public static object ExecuteTask<T>(this ITaskExecutionService service, object contextObject)
        {
            return service.ExecuteTask<T>(contextObject, new object[] {});
        }

        public static object ExecuteTask<T>(this ITaskExecutionService service, object contextObject,
                                            params object[] contextArgs)
        {
            TaskDescriptorAttribute taskDesc = TaskDescriptorAttribute.Find(typeof (T));
            if (taskDesc != null)
            {
                return service.ExecuteTask(taskDesc.TaskName, contextObject, contextArgs);
            }
            throw new ArgumentException(
                "ExecuteTask<T> cannot be defined using a type unless it has a TaskDescriptorAttribute defined.");
        }
    }
}
