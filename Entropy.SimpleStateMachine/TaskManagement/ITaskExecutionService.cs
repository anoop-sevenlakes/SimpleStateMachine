namespace Entropy.SimpleStateMachine.TaskManagement
{
	using System;

	public interface ITaskExecutionService
	{
		/// <summary>
		/// Gets the available tasks.
		/// </summary>
		/// <value>The available tasks.</value>
		TaskDescription[] AvailableTasks { get; }

		/// <summary>
		/// Occurs when a task completes.
		/// </summary>
		event TaskEventHandler TaskComplete;

		/// <summary>
		/// Occurs when there is a task error.
		/// </summary>
		event TaskEventHandler TaskError;

		/// <summary>
		/// Determines whether the specified task name has been registered.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns>
		/// 	<c>true</c> if the specified task name has been registered; otherwise, <c>false</c>.
		/// </returns>
		bool HasTask(string taskName);

		/// <summary>
		/// Creates the task.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		ITask CreateTask(string taskName);

		/// <summary>
		/// Creates the task.
		/// </summary>
		/// <param name="taskDescription">The task description.</param>
		/// <returns></returns>
		ITask CreateTask(TaskDescription taskDescription);

		/// <summary>
		/// Creates the task context.
		/// </summary>
		/// <param name="contextObject">The context object.</param>
		/// <param name="taskArgs">The task args.</param>
		/// <returns></returns>
		ITaskContext CreateTaskContext(object contextObject, object[] taskArgs);

		/// <summary>
		/// Executes the task.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="context">The context.</param>
		/// <param name="callback">The callback.</param>
		/// <returns></returns>
		object ExecuteTask(ITask task, ITaskContext context, TaskEventHandler callback);

		/// <summary>
		/// Begins the execute task.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="context">The context.</param>
		/// <param name="callback">The callback.</param>
		void BeginExecuteTask(ITask task, ITaskContext context, TaskEventHandler callback);

		/// <summary>
		/// Gets the available tasks descriptions by attribute.
		/// </summary>
		/// <param name="descriptorType">Type descriptor.</param>
		/// <returns></returns>
		TaskDescription[] GetAvailableTasksByAttributeType(Type descriptorType);

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns></returns>
		TaskDescription GetTaskDescription(ITask task);

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		TaskDescription GetTaskDescription(string taskName);
	}
}