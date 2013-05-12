namespace Entropy.SimpleStateMachine.TaskManagement
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Interfaces;
	using Utility;

	/// <summary>
	/// Summary description for TaskFactory.
	/// </summary>
	public class TaskFactory
	{
		private static readonly object _statThreadLock = new object();
		private static TaskFactory _default;

		private readonly Dictionary<string, TaskDescription> _availableTasks =
			new Dictionary<string, TaskDescription>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskFactory"/> class.
		/// </summary>
		/// <param name="searchAllLoadedAssemblies">if set to <c>true</c> all loaded assemblies are searched for tasks.</param>
		public TaskFactory(bool searchAllLoadedAssemblies)
			: this(searchAllLoadedAssemblies ? AppDomain.CurrentDomain.GetAssemblies() : null)
		{
		}

		public TaskFactory(IEnumerable<Assembly> taskAssemblies)
		{
			var assemblies = new List<Assembly>();
			if (taskAssemblies != null)
				assemblies.AddRange(taskAssemblies);
			//if (!assemblies.Contains(GetType().Assembly))
			//    assemblies.Add(GetType().Assembly);
			//if (!assemblies.Contains(RuntimeEnvironment.Current.GetType().Assembly))
			//    assemblies.Add(RuntimeEnvironment.Current.GetType().Assembly);

			Initialize(assemblies);
		}

		/// <summary>
		/// Gets default task factory.
		/// </summary>
		/// <value>The default.</value>
		public static TaskFactory Default
		{
			get
			{
				lock (_statThreadLock)
				{
					if (_default == null)
					{
					}
					else
					{
						_default = new TaskFactory(true);
					}
					return _default;
				}
			}
		}

		/// <summary>
		/// Gets the available tasks.
		/// </summary>
		/// <value>The available tasks.</value>
		public TaskDescription[] AvailableTasks
		{
			get
			{
				var output = new TaskDescription[_availableTasks.Count];
				_availableTasks.Values.CopyTo(output, 0);
				return output;
			}
		}

		/// <summary>
		/// Occurs when on an error.
		/// </summary>
		public event EventHandler<ErrorEventArgs> Error;

		/// <summary>
		/// Initializes tasks from the specified assemblies.
		/// </summary>
		/// <param name="taskAssemblies">The task assemblies.</param>
		private void Initialize(IEnumerable<Assembly> taskAssemblies)
		{
			foreach (Assembly assembly in taskAssemblies)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (typeof (ITask).IsAssignableFrom(type))
					{
						TaskDescription description;
						object[] attributes = type.GetCustomAttributes(typeof (TaskDescriptorAttribute), true);

						if (attributes != null && attributes.Length > 0)
						{
							description = ((TaskDescriptorAttribute) attributes[0]).TaskDescription;
							description.TaskType = type;
							AddAvailableTask(description);
						}
						//else
						//{
						//    description = new TaskDescription(type.Name, type);
						//}
					}
				}
			}
		}

		/// <summary>
		/// Gets the available tasks descriptions by attribute.
		/// </summary>
		/// <param name="descriptorType">Type descriptor.</param>
		/// <returns></returns>
		public TaskDescription[] GetAvailableTasksByAttributeType(Type descriptorType)
		{
			var list = new List<TaskDescription>();
			foreach (TaskDescription task in _availableTasks.Values)
			{
				if (task.DescriptorAttributeType == descriptorType)
					list.Add(task);
			}
			return list.ToArray();
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
			return _availableTasks.ContainsKey(taskName);
		}

		/// <summary>
		/// Creates the task.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		public ITask CreateTask(string taskName)
		{
			TaskDescription task;
			_availableTasks.TryGetValue(taskName, out task);
			if (task != null)
				return CreateTask(task);

			throw new InvalidOperationException("The requested task " + taskName + " could not be found.");
		}

		/// <summary>
		/// Creates the task.
		/// </summary>
		/// <param name="taskDescription">The task description.</param>
		/// <returns></returns>
		public ITask CreateTask(TaskDescription taskDescription)
		{
			if (taskDescription != null)
			{
				try
				{
					return ServiceLocator.GetService<IObjectBuilder>().Create<ITask>(taskDescription.TaskType);
				}
				catch (Exception ex)
				{
					OnError(ex);
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		public TaskDescription GetDescription(string taskName)
		{
			return _availableTasks[taskName];
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns></returns>
		public TaskDescription GetDescription(ITask task)
		{
			if (task == null)
				return null;

			Type taskType = task.GetType();

			foreach (TaskDescription description in _availableTasks.Values)
			{
				if (description.TaskType == taskType)
					return description;
			}
			return TaskDescription.GetDescription(task);
		}

		/// <summary>
		/// Adds the available task.
		/// </summary>
		/// <param name="task">The task.</param>
		protected void AddAvailableTask(TaskDescription task)
		{
			if (String.IsNullOrEmpty(task.Name) && task.TaskType != null)
				task.Name = task.TaskType.Name;

			if (_availableTasks.ContainsKey(task.Name))
				throw new InvalidOperationException(
					string.Format("A task with the name {0} already exists in this collection.", task.Name));
			_availableTasks[task.Name] = task;
		}

		/// <summary>
		/// Called when an error has occured.
		/// </summary>
		/// <param name="error">The error.</param>
		protected virtual void OnError(Exception error)
		{
			EventHandler<ErrorEventArgs> evt = Error;
			if (evt != null)
				evt(this, new ErrorEventArgs(error));
		}
	}
}