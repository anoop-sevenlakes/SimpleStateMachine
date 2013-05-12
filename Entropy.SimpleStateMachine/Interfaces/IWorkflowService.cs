namespace Entropy.SimpleStateMachine.Interfaces
{
	/// <summary>
	/// Workflow service.
	/// </summary>
	public interface IWorkflowService : IStateMachineContextPersistenceService
	{
		/// <summary>
		/// Starts the specified state machine.
		/// </summary>
		/// <param name="stateMachineName">Name of the state machine.</param>
		/// <returns>The workflow</returns>
		IStateMachineContext Start(string stateMachineName);

		/// <summary>
		/// Starts the specified state machine name.
		/// </summary>
		/// <param name="stateMachineName">Name of the state machine.</param>
		/// <param name="domainContext">The domain context.</param>
		/// <returns>The workflow</returns>
		IStateMachineContext Start(string stateMachineName, object domainContext);

		/// <summary>
		/// Starts the specified state machine name.
		/// </summary>
		/// <param name="stateMachineName">Name of the state machine.</param>
		/// <param name="domainContext">The domain context.</param>
		/// <param name="domainContextStateProperty">The domain context state property.</param>
		/// <returns>The workflow</returns>
		IStateMachineContext Start(string stateMachineName, object domainContext, string domainContextStateProperty);        
	}
}
