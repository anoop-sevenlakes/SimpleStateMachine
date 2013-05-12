namespace Entropy.SimpleStateMachine.Services
{
	using System;
	using System.Reflection;
	using Interfaces;
	using Utility;

	/// <summary>
	/// Workflow service.
	/// </summary>
	public class WorkflowService : IWorkflowService
	{
		private const string CurrentStateFieldName = "_currentState";

		private readonly FieldInfo _stateField;

		private readonly IDomainContextRepository _domainContextRepository;
		private readonly IStateMachineService _stateMachineService;
		private readonly IWorkflowPersister _workflowPersister;

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowService"/> class.
		/// </summary>
		/// <param name="stateMachineService">The machine repository.</param>
		/// <param name="workflowPersister">The workflow persister.</param>
		/// <param name="domainContextRepository">The domain context repository.</param>
		public WorkflowService(
			IStateMachineService stateMachineService,
			IWorkflowPersister workflowPersister,
			IDomainContextRepository domainContextRepository)
		{
			if (stateMachineService == null) throw new ArgumentNullException("stateMachineService");
			if (workflowPersister == null) throw new ArgumentNullException("workflowPersister");

			_stateMachineService = stateMachineService;
			_workflowPersister = workflowPersister;
			_domainContextRepository = domainContextRepository;

			_stateField = GetStateMachineContextFieldInfo(CurrentStateFieldName);
		}

		#region IWorkflowService Members

		/// <summary>
		/// Starts the specified state machine.
		/// </summary>
		/// <param name="stateMachineName">Name of the state machine.</param>
		/// <returns>The workflow</returns>
		public IStateMachineContext Start(string stateMachineName)
		{
			return Start(stateMachineName, null);
		}

		/// <summary>
		/// Starts the specified state machine name.
		/// </summary>
		/// <param name="stateMachineName">Name of the state machine.</param>
		/// <param name="domainContext">The domain context.</param>
		/// <returns>The workflow</returns>
		public IStateMachineContext Start(string stateMachineName, object domainContext)
		{
			IStateMachine stateMachine = _stateMachineService.CreateStateMachine(stateMachineName);
			var machineContext = new StateMachineContext(stateMachine, domainContext, this);

			machineContext.Start();
			return machineContext;
		}

		/// <summary>
		/// Starts the specified state machine name.
		/// </summary>
		/// <param name="stateMachineName">Name of the state machine.</param>
		/// <param name="domainContext">The domain context.</param>
		/// <param name="domainContextStateProperty">The domain context state property.</param>
		/// <returns>The workflow</returns>
		public IStateMachineContext Start(string stateMachineName, object domainContext, string domainContextStateProperty)
		{
			IStateMachine stateMachine = _stateMachineService.CreateStateMachine(stateMachineName);
			var machineContext = new StateMachineContext(stateMachine,
			                                             new ReflectiveDomainContextWrapper(domainContext,
			                                                                                domainContextStateProperty), this);

			machineContext.Start();
			return machineContext;
		}

		/// <summary>
		/// Persists the specified work flow.
		/// </summary>
		/// <param name="workFlow">The work flow.</param>
		public void Save(IStateMachineContext workFlow)
		{
			if (workFlow == null) throw new ArgumentNullException("workFlow");

			bool isPersisted = workFlow.Id != Guid.Empty;
			if (isPersisted == false)
			{
				workFlow.Id = GuidCombGenerator.Generate();
			}

			string domainContextType = null;
			object domainContextId = null;
			object domainContext = GetDomainContext(workFlow);

			string domainContextStatusProperty = string.Empty;
			if (workFlow.DomainContext is ReflectiveDomainContextWrapper)
				domainContextStatusProperty = ((ReflectiveDomainContextWrapper) workFlow.DomainContext).StateProperty.Name;

			if (domainContext != null && _domainContextRepository != null)
			{
				_domainContextRepository.Save(domainContext);
				
				domainContextType = _domainContextRepository.GetTypeDescription(domainContext);
				domainContextId = _domainContextRepository.GetId(domainContext);
			}

			IStateMachineState currentState = workFlow.CurrentState;

			IWorkflowEntity workflowEntity = isPersisted
			                                 	? _workflowPersister.Load(workFlow.Id)
			                                 	: _workflowPersister.CreateEmptyWorkflowEntity(workFlow.Id);

			if (workflowEntity == null)
				throw new Exception("The workflow persister returned a null object from the CreateEmptyWorkflowEntity call.");

			workflowEntity.WorkflowId = workFlow.Id;
			workflowEntity.CurrentState = currentState != null ? currentState.StateName : string.Empty;
			workflowEntity.MachineConfiguration = Convert.ToString(workFlow.StateMachine.Tag);
			workflowEntity.DomainContextTypeDescription = domainContextType;
			workflowEntity.DomainContextId = domainContextId;
			workflowEntity.DomainContextStatusProperty = domainContextStatusProperty;

			if (isPersisted && workFlow.IsComplete)
			{
				_workflowPersister.Complete(workflowEntity);
			}
			else if (isPersisted)
			{
				_workflowPersister.Update(workflowEntity);
			}
			else
			{
				_workflowPersister.Save(workflowEntity);
			}
		}

		/// <summary>
		/// Loads work flow with the specified id.
		/// </summary>
		/// <param name="workflowId">The work flow id.</param>
		/// <returns>The work flow.</returns>
		public IStateMachineContext Load(Guid workflowId)
		{
			IWorkflowEntity workflowEntity = _workflowPersister.Load(workflowId);

			if (workflowEntity == null)
				throw new ArgumentNullException(string.Format("The workflow with id {0} can not be loaded", workflowId));

			IStateMachine stateMachine = _stateMachineService.CreateStateMachine(workflowEntity.MachineConfiguration);

			object domainContext = null;
			StateMachineContext machineContext;

			if (_domainContextRepository != null)
			{
				domainContext = _domainContextRepository.Load(workflowEntity.DomainContextTypeDescription,
				                                              workflowEntity.DomainContextId);
			}

			// there is a domain context and we have a status property
			if (domainContext != null && string.IsNullOrEmpty(workflowEntity.DomainContextStatusProperty) == false)
			{
				machineContext = new StateMachineContext(stateMachine,
				                                         new ReflectiveDomainContextWrapper(domainContext,
				                                                                            workflowEntity.
				                                                                            	DomainContextStatusProperty), this);
			}
			else
			{
				machineContext = new StateMachineContext(stateMachine, domainContext, this);
			}

			_stateField.SetValue(machineContext, stateMachine.FindStateByName(workflowEntity.CurrentState));

			machineContext.Id = workflowId;

			return machineContext;
		}

		#endregion

		/// <summary>
		/// Gets the domain context.
		/// </summary>
		/// <param name="workFlow">The work flow.</param>
		/// <returns></returns>
		private static object GetDomainContext(IStateMachineContext workFlow)
		{
			object domainContext;
			if (workFlow.DomainContext is ReflectiveDomainContextWrapper)
			{
				domainContext = ((ReflectiveDomainContextWrapper) workFlow.DomainContext).DomainContext;
			}
			else
			{
				domainContext = workFlow.DomainContext;
			}
			return domainContext;
		}

		/// <summary>
		/// Gets the state machine context field info.
		/// </summary>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns>The field info</returns>
		private static FieldInfo GetStateMachineContextFieldInfo(string fieldName)
		{
			Type contextType = typeof (StateMachineContext);

			FieldInfo fieldInfo = contextType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (fieldInfo == null)
				throw new ArgumentException(CurrentStateFieldName + " is not a field of " + contextType.Name);

			return fieldInfo;
		}
	}
}