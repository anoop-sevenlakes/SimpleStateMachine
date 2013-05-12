namespace Entropy.SimpleStateMachine.TaskManagement
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Tasks;

	internal delegate object ExecuteTaskDelegate(ITask task, ITaskContext context);

	/// <summary>
	/// Summary description for TaskManager.
	/// </summary>
	public class TaskExecutionService : ITaskExecutionService
	{
		private readonly TaskFactory _factory;
		private readonly RetriableTaskContainer _retriableTasks;

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskExecutionService"/> class.
		/// </summary>
		/// <param name="taskAssemblies">The task assemblies.</param>
		public TaskExecutionService(IEnumerable<Assembly> taskAssemblies)
		{
			_factory = new TaskFactory(taskAssemblies);
			_retriableTasks = new RetriableTaskContainer();

			_retriableTasks.TaskCompleted += RetriableTask_TaskCompleted;
			_retriableTasks.TaskCompletedWithError += RetriableTask_TaskCompletedWithError;
		}

		#region ITaskExecutionService Members

		/// <summary>
		/// Occurs when a task completes.
		/// </summary>
		public event TaskEventHandler TaskComplete;

		/// <summary>
		/// Occurs when there is a task error.
		/// </summary>
		public event TaskEventHandler TaskError;

		/// <summary>
		/// Creates the task context.
		/// </summary>
		/// <param name="contextObject">The context object.</param>
		/// <param name="taskArgs">The task args.</param>
		/// <returns></returns>
		public ITaskContext CreateTaskContext(object contextObject, object[] taskArgs)
		{
			return new TaskContext(contextObject, taskArgs);
		}

		/// <summary>
		/// Executes the task.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="context">The context.</param>
		/// <param name="callback">The callback.</param>
		/// <returns></returns>
		public object ExecuteTask(ITask task, ITaskContext context, TaskEventHandler callback)
		{
			RetriableTaskCheck(task, context);
			object taskResult = SafeExecuteTask(task, context);

			var args = new TaskEventArgs(task, context, taskResult);

			if (callback != null)
			{
				//callback.BeginInvoke(this,args,null,null);
				callback(this, args);
			}

			if (args.ResultIsException)
				OnTaskError(args);
			else
				OnTaskComplete(args);

			return taskResult;
		}


		/// <summary>
		/// Begins the execute task.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="context">The context.</param>
		/// <param name="callback">The callback.</param>
		public void BeginExecuteTask(ITask task, ITaskContext context, TaskEventHandler callback)
		{
			RetriableTaskCheck(task, context);
			ExecuteTaskDelegate del = SafeExecuteTask;
			del.BeginInvoke(task, context, EndExecuteTask, new object[] {del, task, context, callback});
		}

		/// <summary>
		/// Determines whether the specified task name has been registered.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns>
		/// 	<c>true</c> if the specified task name has been registered; otherwise, <c>false</c>.
		/// </returns>
		public bool HasTask(string taskName)
		{
			return _factory.HasTask(taskName);
		}

		/// <summary>
		/// Creates the task.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		public ITask CreateTask(string taskName)
		{
			return _factory.CreateTask(taskName);
		}

		/// <summary>
		/// Creates the task.
		/// </summary>
		/// <param name="taskDescription">The task description.</param>
		/// <returns></returns>
		public ITask CreateTask(TaskDescription taskDescription)
		{
			return _factory.CreateTask(taskDescription);
		}

		/// <summary>
		/// Gets the available tasks.
		/// </summary>
		/// <value>The available tasks.</value>
		public TaskDescription[] AvailableTasks
		{
			get { return _factory.AvailableTasks; }
		}

		/// <summary>
		/// Gets the available tasks descriptions by attribute.
		/// </summary>
		/// <param name="descriptorType">Type descriptor.</param>
		/// <returns></returns>
		public TaskDescription[] GetAvailableTasksByAttributeType(Type descriptorType)
		{
			return _factory.GetAvailableTasksByAttributeType(descriptorType);
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		public TaskDescription GetTaskDescription(string taskName)
		{
			return _factory.GetDescription(taskName);
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns></returns>
		public TaskDescription GetTaskDescription(ITask task)
		{
			return _factory.GetDescription(task);
		}

		#endregion

		private void RetriableTaskCheck(ITask task, ITaskContext context)
		{
			if (task is RetriableTask)
			{
				_retriableTasks.RegisterTask(task as RetriableTask, context);
			}
		}

		private void RetriableTask_TaskCompleted(object sender, TaskEventArgs args)
		{
			OnTaskComplete(args);
		}

		private void RetriableTask_TaskCompletedWithError(object sender, TaskEventArgs args)
		{
			OnTaskError(args);
		}

		private void EndExecuteTask(IAsyncResult result)
		{
			var state = result.AsyncState as object[];
			if (state != null && state.Length != 4)
				return;

			if (state != null)
			{
				var del = (ExecuteTaskDelegate) state[0];
				var task = (ITask) state[1];
				var context = (ITaskContext) state[2];
				var callback = state[3] as TaskEventHandler;
				object taskResult = del.EndInvoke(result);

				var args = new TaskEventArgs(task, context, taskResult);

				if (callback != null)
				{
					callback(this, args);
				}

				if (args.ResultIsException)
					OnTaskError(args);
				else
					OnTaskComplete(args);
			}
		}

		private static object SafeExecuteTask(ITask task, ITaskContext context)
		{
			try
			{
				return task.Execute(context);
			}
			catch (Exception ex)
			{
				return ex;
			}
		}

		protected virtual void OnTaskComplete(TaskEventArgs args)
		{
			if (TaskComplete != null)
			{
				TaskComplete(this, args);
			}
		}

		protected virtual void OnTaskError(TaskEventArgs args)
		{
			//RuntimeEnvironment.Current.ServiceManager.SystemLogService.Log(LogEntryCategory.TaskExecutionError,
			//                                                               "Error Executing Task: " +
			//                                                               args.Task.GetType().Name,
			//                                                               new Exception(
			//                                                                   "Error executing task " +
			//                                                                   args.Task.GetType().Name,
			//                                                                   args.TaskResult as Exception));
			if (TaskError != null)
			{
				TaskError(this, args);
			}
		}
	}
}