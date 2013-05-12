using Entropy.SimpleStateMachine.TaskManagement;

namespace Entropy.SimpleStateMachine.Interfaces
{
	public interface IStateMachine
    {
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
        string Name { get; }

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		object Tag { get; set; }

		/// <summary>
		/// Gets the states.
		/// </summary>
		/// <value>The states.</value>
        IStateMachineState[] States { get; }

		/// <summary>
		/// Gets the events.
		/// </summary>
		/// <value>The events.</value>
        IStateMachineEvent[] Events { get; }

		/// <summary>
		/// Gets the state transition actions.
		/// </summary>
		/// <value>The state transition actions.</value>
        IStateTransitionAction[] StateTransitionActions { get; }

        /// <summary>
        /// Gets the list of actions to be fired when certain events occur
        /// </summary>
        IStateMachineEventAction[] EventActions { get; }

		/// <summary>
		/// Gets the action task manager.
		/// </summary>
		/// <value>The action task manager.</value>
        ITaskExecutionService ActionTaskManager { get; }
    }
}
