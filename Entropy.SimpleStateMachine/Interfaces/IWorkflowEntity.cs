namespace Entropy.SimpleStateMachine.Interfaces
{
	using System;

	public interface IWorkflowEntity
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		Guid WorkflowId { get; set; }

		/// <summary>
		/// Gets or sets the machine configuration.
		/// </summary>
		/// <value>The machine configuration.</value>
		string MachineConfiguration { get; set; }

		/// <summary>
		/// Gets or sets the current state.
		/// </summary>
		/// <value>The current state.</value>
		string CurrentState { get; set; }

		/// <summary>
		/// Gets or sets the domain context type description.
		/// </summary>
		/// <value>The domain context type description.</value>
		string DomainContextTypeDescription { get; set; }

		/// <summary>
		/// Gets or sets the domain context id.
		/// </summary>
		/// <value>The domain context id.</value>
		object DomainContextId { get; set; }

		/// <summary>
		/// Gets or sets the domain context status property.
		/// </summary>
		/// <value>The domain context status property.</value>
		string DomainContextStatusProperty { get; set; }
	}
}