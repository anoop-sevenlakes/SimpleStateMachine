namespace Entropy.SimpleStateMachine.Services.Persistence
{
	using System;
	using System.IO;
	using System.Xml.Serialization;
	using Interfaces;

	public class FilePersister : IWorkflowPersister
	{
		private readonly string _workflowStore;
		private readonly XmlSerializer _xmlSerializer;

		/// <summary>
		/// Initializes a new instance of the <see cref="FilePersister"/> class.
		/// </summary>
		/// <param name="workflowStore">The workflow store.</param>
		public FilePersister(string workflowStore)
		{
			_workflowStore = workflowStore;

			_xmlSerializer = new XmlSerializer(typeof(WorkflowEntity));
		}

		/// <summary>
		/// Saves the specified workflow entity.
		/// </summary>
		/// <param name="workflowEntity">The workflow entity.</param>
		public void Save(IWorkflowEntity workflowEntity)
		{
			string fileName = GetFileName(workflowEntity.WorkflowId);

			using (var stream = new FileStream(fileName, FileMode.Create)) {
				_xmlSerializer.Serialize(stream, workflowEntity);
			}		
		}

		/// <summary>
		/// Updates the specified workflow entity.
		/// </summary>
		/// <param name="workflowEntity">The workflow entity.</param>
		public void Update(IWorkflowEntity workflowEntity)
		{
			Save(workflowEntity);
		}

		/// <summary>
		/// Completes the specified workflow entity.
		/// </summary>
		/// <param name="workflowEntity">The workflow entity.</param>
		public void Complete(IWorkflowEntity workflowEntity)
		{
			string fileName = GetFileName(workflowEntity.WorkflowId);
			if(File.Exists(fileName))
				File.Delete(fileName);
		}

		/// <summary>
		/// Gets the specified workflow.
		/// </summary>
		/// <param name="workflowId">The workflow id.</param>
		/// <returns></returns>
		public IWorkflowEntity Load(Guid workflowId)
		{
			string fileName = GetFileName(workflowId);

			using (var stream = new FileStream(fileName, FileMode.Open))
			{
				return (WorkflowEntity) _xmlSerializer.Deserialize(stream);
			}
		}

		/// <summary>
		/// Creates the empty workflow entity.
		/// </summary>
		/// <returns></returns>
		public IWorkflowEntity CreateEmptyWorkflowEntity(Guid workflowId)
		{
			return new WorkflowEntity{WorkflowId = workflowId};
		}

		/// <summary>
		/// Gets the name of the persisted workflow file.
		/// </summary>
		/// <param name="workFlowId">The work flow id.</param>
		/// <returns>The filename.</returns>
		private string GetFileName(Guid workFlowId)
		{
			return Path.Combine(_workflowStore, string.Format("{0}.xml", workFlowId));
		}
	}
}