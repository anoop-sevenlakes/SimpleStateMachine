namespace Entropy.SimpleStateMachine.Interfaces
{
	using System.Reflection;

	public interface IStateMachineBuilder : IStateMachine, IStateTransitionActionManager, IStateEventActionManager
	{
		/// <summary>
		/// Sets the name.
		/// </summary>
		/// <param name="name">The name.</param>
		void SetName(string name);

		/// <summary>
		/// Adds the new state.
		/// </summary>
		/// <param name="stateName">Name of the state.</param>
		/// <param name="stateIdentifier">The state identifier.</param>
		/// <returns></returns>
		IStateMachineStateBuilder AddNewState(string stateName, object stateIdentifier);

		/// <summary>
		/// Adds the new event.
		/// </summary>
		/// <param name="stateEvent">The state event.</param>
		/// <param name="eventIdentifier">The event identifier.</param>
		/// <returns></returns>
		IStateMachineEvent AddNewEvent(string stateEvent, object eventIdentifier);

		/// <summary>
		/// Registers a task assembly.
		/// </summary>
		/// <param name="taskAssembly">The task assembly.</param>
		void RegisterTaskAssembly(Assembly taskAssembly);
	}
}