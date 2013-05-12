namespace Entropy.SimpleStateMachine.Interfaces
{
	using System;

	public interface IStateMachineContextPersistenceService
	{
		/// <summary>
		/// Persists the specified work flow.
		/// </summary>
		/// <param name="workFlow">The work flow.</param>
		void Save(IStateMachineContext workFlow);

		/// <summary>
		/// Loads work flow with the specified id.
		/// </summary>
		/// <param name="workflowId">The work flow id.</param>
		/// <returns>The work flow.</returns>
		IStateMachineContext Load(Guid workflowId);
	}
}