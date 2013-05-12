namespace Entropy.SimpleStateMachine
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Interfaces;
	using TaskManagement;

	public class StateMachine : IStateMachineBuilder
	{
		private ITaskExecutionService _actionTaskManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateMachine"/> class.
		/// </summary>
		public StateMachine()
		{
			StateList = new List<IStateMachineState>();
			EventList = new List<IStateMachineEvent>();
			StateTransitionActionList = new List<IStateTransitionAction>();
		    EventActionList = new List<IStateMachineEventAction>();
		}

		protected List<IStateMachineState> StateList { get; private set; }
		protected List<IStateMachineEvent> EventList { get; private set; }
		protected List<IStateTransitionAction> StateTransitionActionList { get; private set; }
        protected List<IStateMachineEventAction> EventActionList { get; private set; }
		protected List<Assembly> TaskAssemblies { get; set; }

		#region IStateMachineBuilder Members

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public object Tag { get; set; }

		/// <summary>
		/// Gets the events.
		/// </summary>
		/// <value>The events.</value>
		public IStateMachineEvent[] Events
		{
			get { return EventList.ToArray(); }
		}

		/// <summary>
		/// Gets the states.
		/// </summary>
		/// <value>The states.</value>
		public IStateMachineState[] States
		{
			get { return StateList.ToArray(); }
		}

		/// <summary>
		/// Gets the state transition actions.
		/// </summary>
		/// <value>The state transition actions.</value>
		public IStateTransitionAction[] StateTransitionActions
		{
			get { return StateTransitionActionList.ToArray(); }
		}

	    public IStateMachineEventAction[] EventActions
	    {
            get { return EventActionList.ToArray(); }
	    }

		/// <summary>
		/// Gets or sets the action task manager.
		/// </summary>
		/// <value>The action task manager.</value>
		public ITaskExecutionService ActionTaskManager
		{
			get
			{
				if (_actionTaskManager == null)
					_actionTaskManager = CreateTaskExecutionService();

				return _actionTaskManager;
			}
			set { _actionTaskManager = value; }
		}

		/// <summary>
		/// Adds the new event.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="eventIdentifier">The event identifier.</param>
		/// <returns></returns>
		public IStateMachineEvent AddNewEvent(string eventName, object eventIdentifier)
		{
			var newEvent = new StateMachineEvent(this, eventName, eventIdentifier);
			EventList.Add(newEvent);
			return newEvent;
		}

		/// <summary>
		/// Adds the new state.
		/// </summary>
		/// <param name="stateName">Name of the state.</param>
		/// <param name="stateIdentifier">The state identifier.</param>
		/// <returns></returns>
		public virtual IStateMachineStateBuilder AddNewState(string stateName, object stateIdentifier)
		{
			var newState = new StateMachineState(this, stateName, stateIdentifier);
			StateList.Add(newState);
			return newState;
		}

		/// <summary>
		/// Adds the new transition action.
		/// </summary>
		/// <param name="actionTaskName">Name of the action task.</param>
		/// <param name="targetPhase">The target phase.</param>
		/// <param name="actionArgs">The action args.</param>
		/// <returns></returns>
		public virtual IStateTransitionAction AddNewTransitionAction(string actionTaskName,
		                                                             StateTransitionPhase targetPhase,
		                                                             params object[] actionArgs)
		{
			var action = new StateTransitionAction(this, actionTaskName, targetPhase, actionArgs);
			StateTransitionActionList.Add(action);
			return action;
		}

	    public virtual IStateMachineEventAction AddNewEventAction(string actionTaskName,
	                                                              IStateMachineEvent eventIdentifier,
	                                                              params object[] actionArgs)
	    {
	        var action = new StateMachineEventAction(this, actionTaskName, eventIdentifier, actionArgs);
	        EventActionList.Add(action);
	        return action;
	    }

		/// <summary>
		/// Sets the name of the state machine.
		/// </summary>
		/// <param name="name">The name.</param>
		public void SetName(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			Name = name;
		}

		/// <summary>
		/// Registers a task assembly.
		/// </summary>
		/// <param name="taskAssembly">The task assembly.</param>
		public void RegisterTaskAssembly(Assembly taskAssembly)
		{
			if (StateList.Count > 0)
				throw new InvalidOperationException(
					"ITask assemblies can only be registered prior to adding any States. If you are programatically registering task assemblies, please make sure you register these assemblies prior to loading the state machine from from the DSL factory.");

			if (TaskAssemblies == null)
				TaskAssemblies = new List<Assembly>();

			if (!TaskAssemblies.Contains(taskAssembly))
				TaskAssemblies.Add(taskAssembly);
		}

		#endregion

		/// <summary>
		/// Adds the new state.
		/// </summary>
		/// <param name="stateName">Name of the state.</param>
		/// <returns></returns>
		public IStateMachineStateBuilder AddNewState(string stateName)
		{
			return AddNewState(stateName, stateName);
		}

		/// <summary>
		/// Creates the task execution service.
		/// </summary>
		/// <returns></returns>
		protected virtual ITaskExecutionService CreateTaskExecutionService()
		{
			var assemblies = TaskAssemblies ?? (IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies();
			return new TaskExecutionService(assemblies);
		}
	}
}