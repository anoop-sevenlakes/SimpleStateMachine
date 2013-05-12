namespace Entropy.SimpleStateMachine.Tests
{
	using System;
	using System.IO;
	using Entropy.SimpleStateMachine.Services.Persistence;
	using NUnit.Framework;

	[TestFixture]
	public class FilePersisterTestFixture
	{
		[Test]
		public void SaveAndLoad()
		{
			var filePersister = new FilePersister(string.Empty);
			
			WorkflowEntity workflowEntity = new WorkflowEntity
			                                	{
			                                		CurrentState = "CurrentState",
			                                		DomainContextId = Guid.NewGuid(),
			                                		DomainContextStatusProperty = "DomainContextStatusProperty",
			                                		DomainContextTypeDescription = "DomainContextTypeDescription",
			                                		MachineConfiguration = "MachineConfiguration",
			                                		WorkflowId = Guid.NewGuid()
			                                	};

			filePersister.Save(workflowEntity);

			var path = string.Format("{0}.xml", workflowEntity.WorkflowId);
			Assert.That(File.Exists(path));

			var workflowEntity1 = filePersister.Load(workflowEntity.WorkflowId);

			Assert.AreEqual(workflowEntity1.CurrentState, workflowEntity.CurrentState);
			Assert.AreEqual(workflowEntity1.DomainContextId, workflowEntity.DomainContextId);
			Assert.AreEqual(workflowEntity1.DomainContextStatusProperty, workflowEntity.DomainContextStatusProperty);
			Assert.AreEqual(workflowEntity1.DomainContextTypeDescription, workflowEntity.DomainContextTypeDescription);
			Assert.AreEqual(workflowEntity1.MachineConfiguration, workflowEntity.MachineConfiguration);
			Assert.AreEqual(workflowEntity1.WorkflowId, workflowEntity.WorkflowId);
			Assert.AreEqual(workflowEntity1.WorkflowId, workflowEntity.WorkflowId);

			File.Delete(path);
		}
	}
}