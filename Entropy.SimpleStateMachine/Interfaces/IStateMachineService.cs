namespace Entropy.SimpleStateMachine.Interfaces
{
	/// <summary>
	/// State machine repository
	/// </summary>
	public interface IStateMachineService
	{
		/// <summary>
		/// Creates the state machine.
		/// </summary>
		/// <param name="stateMachineName">Name of the state machine.</param>
		/// <returns>The state machine</returns>
		IStateMachine CreateStateMachine(string stateMachineName);
	}
}
