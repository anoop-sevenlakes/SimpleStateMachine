namespace Entropy.SimpleStateMachine.Services.Persistence
{
	using System;
	using Interfaces;

	/// <summary>
	/// Represents a workflow that can be persisted.
	/// </summary>
	[Serializable]
	public class WorkflowEntity : IWorkflowEntity
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public Guid WorkflowId { get; set; }

		/// <summary>
		/// Gets or sets the machine configuration.
		/// </summary>
		/// <value>The machine configuration.</value>
		public string MachineConfiguration { get; set; }

		/// <summary>
		/// Gets or sets the current state.
		/// </summary>
		/// <value>The current state.</value>
		public string CurrentState { get; set; }

		/// <summary>
		/// Gets or sets the domain context type description.
		/// </summary>
		/// <value>The domain context type description.</value>
		public string DomainContextTypeDescription { get; set; }

		/// <summary>
		/// Gets or sets the domain context id.
		/// </summary>
		/// <value>The domain context id.</value>
		public object DomainContextId { get; set; }

		/// <summary>
		/// Gets or sets the domain context status property.
		/// </summary>
		/// <value>The domain context status property.</value>
		public string DomainContextStatusProperty { get; set; }
	}
}