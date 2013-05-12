namespace Entropy.SimpleStateMachine.Interfaces
{
	using System;

	public interface IWorkflowPersister
	{
		/// <summary>
		/// Saves the specified workflow entity.
		/// </summary>
		/// <param name="workflowEntity">The workflow entity.</param>
		void Save(IWorkflowEntity workflowEntity);

		/// <summary>
		/// Updates the specified workflow entity.
		/// </summary>
		/// <param name="workflowEntity">The workflow entity.</param>
		void Update(IWorkflowEntity workflowEntity);

		/// <summary>
		/// Completes the specified workflow entity.
		/// </summary>
		/// <param name="workflowEntity">The workflow entity.</param>
		void Complete(IWorkflowEntity workflowEntity);

		/// <summary>
		/// Gets the specified workflow.
		/// </summary>
		/// <param name="workflowId">The workflow id.</param>
		/// <returns></returns>
		IWorkflowEntity Load(Guid workflowId);

		/// <summary>
		/// Creates the empty workflow entity.
		/// </summary>
		/// <param name="workflowId">The workflow id.</param>
		/// <returns></returns>
		IWorkflowEntity CreateEmptyWorkflowEntity(Guid workflowId);
	}
}