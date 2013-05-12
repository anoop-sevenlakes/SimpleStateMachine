namespace Entropy.SimpleStateMachine.Tests
{
	using System;
	using Entropy.SimpleStateMachine.Services.Persistence;
	using Interfaces;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using Services;
	using Utility;

	[TestFixture]
	public class PersistenceTestFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_stateMachineService = new StateMachineService(string.Empty,
			                                               new[] {"Entropy.SimpleStateMachine.Tests"},
			                                               new[] {GetType().Assembly});

			_domainContextRepository = MockRepository.GenerateMock<IDomainContextRepository>();
			_workflowPersister = MockRepository.GenerateMock<IWorkflowPersister>();

			_workflowService = new WorkflowService(_stateMachineService,
			                                       _workflowPersister,
			                                       _domainContextRepository);

			_userStateChangedCount = 0;
		}

		[TearDown]
		public void TearDown()
		{
			ServiceLocator.Clear();
		}

		#endregion

		public class User
		{
			private readonly Guid _id = Guid.NewGuid();
			private object _currentStatus;

			public Guid Id
			{
				get { return _id; }
			}

			public object CurrentStatus
			{
				get { return _currentStatus; }
				set
				{
					if (value != _currentStatus)
					{
						_currentStatus = value;
						OnStatusChanged();
					}
				}
			}

			public event EventHandler StatusChanged;

			protected virtual void OnStatusChanged()
			{
				EventHandler evt = StatusChanged;
				if (evt != null)
					evt(this, new EventArgs());
			}
		}

		private IStateMachineService _stateMachineService;
		private IDomainContextRepository _domainContextRepository;
		private IWorkflowService _workflowService;
		private IWorkflowPersister _workflowPersister;
		private int _userStateChangedCount;

		private void user_StatusChanged(object sender, EventArgs e)
		{
			_userStateChangedCount++;
		}

		[Test]
		public void CanCreateUsingStateMachineServiceTest()
		{
			Assert.IsNotNull(_stateMachineService.CreateStateMachine(@"testStateMachine.boo"));
		}

		[Test]
		public void CanCreateUsingWorkflowServiceTest()
		{
			IWorkflowEntity toReturn = new WorkflowEntity();
			_workflowPersister.Expect(p => p.CreateEmptyWorkflowEntity(Guid.Empty)).IgnoreArguments().Return(toReturn);
			IStateMachineContext machineContext = _workflowService.Start(@"testStateMachine.boo");

			Assert.IsNotNull(machineContext);
			Assert.IsTrue(machineContext.IsStarted);
			
			_workflowPersister.VerifyAllExpectations();
		}

		[Test]
		public void CanPersistAndRestore()
		{
			IWorkflowEntity toReturn = new WorkflowEntity();
			_workflowPersister.Expect(p => p.CreateEmptyWorkflowEntity(Guid.Empty)).IgnoreArguments().Return(toReturn);
			_workflowPersister.Expect(p => p.Save(new WorkflowEntity())).IgnoreArguments().Callback<IWorkflowEntity>(x =>
			                                                                                                        	{
																															toReturn = x;
			                                                                                                        		return true;
			                                                                                                        	});
			IStateMachineContext machineContext = _workflowService.Start(@"testStateMachine.boo");

			Assert.IsNotNull(machineContext);
			Assert.IsTrue(machineContext.IsStarted);
			Assert.IsNotNull(toReturn);
			Assert.That(Guid.Empty == toReturn.WorkflowId, Is.Not.True);

			Assert.AreEqual("Initial", machineContext.CurrentState.StateName);
			Assert.AreNotEqual(Guid.Empty, machineContext.Id);

			_workflowPersister.Expect(p => p.Load(new Guid())).IgnoreArguments().Return(toReturn);

			IStateMachineContext restoredMachine = _workflowService.Load(machineContext.Id);

			Assert.AreNotSame(restoredMachine, _workflowService);

			Assert.IsTrue(restoredMachine.IsStarted);
			Assert.AreEqual(machineContext.CurrentState.StateName, restoredMachine.CurrentState.StateName);

			_workflowPersister.VerifyAllExpectations();
		}

		[Test]
		public void CanPersistAndRestoreDomainContext()
		{
			var user = new User();
			user.StatusChanged += user_StatusChanged;

			_domainContextRepository.Expect(x => x.GetId(user)).Return(user.Id);
			_domainContextRepository.Expect(x => x.GetTypeDescription(user)).Return("SimpleStateMachine.User");
			_domainContextRepository.Expect(x => x.Save(user));
			_domainContextRepository.Expect(x => x.Load("SimpleStateMachine.User", user.Id)).Return(new User());

			IWorkflowEntity toReturn = new WorkflowEntity();
			_workflowPersister.Expect(p => p.CreateEmptyWorkflowEntity(Guid.Empty)).IgnoreArguments().Return(toReturn);
			_workflowPersister.Expect(p => p.Save(new WorkflowEntity())).IgnoreArguments().Callback<IWorkflowEntity>(x =>
			{
				toReturn = x;
				return true;
			});
			
			IStateMachineContext machineContext = _workflowService.Start(@"testStateMachine.boo", user);

			Assert.IsNotNull(machineContext);
			Assert.IsTrue(machineContext.IsStarted);

			_workflowPersister.Expect(p => p.Load(new Guid())).IgnoreArguments().Return(toReturn);

			IStateMachineContext restoredMachine = _workflowService.Load(machineContext.Id);

			Assert.IsAssignableFrom(typeof (User), restoredMachine.DomainContext);

			// should be the newly loaded user.
			Assert.AreNotEqual(restoredMachine.DomainContext, user);
			Assert.AreNotEqual(((User) restoredMachine.DomainContext).Id, user.Id);
			// should have never been called
			Assert.AreEqual(0, _userStateChangedCount);

			_domainContextRepository.VerifyAllExpectations();
			_workflowPersister.VerifyAllExpectations();
		}

		[Test]
		public void CanPersistAndRestoreDomainContextWithStatusProperty()
		{
			var user = new User();
			user.StatusChanged += user_StatusChanged;

			_domainContextRepository.Expect(x => x.GetId(user)).Return(user.Id);
			_domainContextRepository.Expect(x => x.GetTypeDescription(user)).Return("SimpleStateMachine.User");
			_domainContextRepository.Expect(x => x.Save(user));
			_domainContextRepository.Expect(x => x.Load("SimpleStateMachine.User", user.Id)).Return(new User());

			IWorkflowEntity toReturn = new WorkflowEntity();
			_workflowPersister.Expect(p => p.CreateEmptyWorkflowEntity(Guid.Empty)).IgnoreArguments().Return(toReturn);

			_workflowPersister.Expect(p => p.Save(new WorkflowEntity())).IgnoreArguments().Callback<IWorkflowEntity>(x =>
			{
				toReturn = x;
				return true;
			});

			IStateMachineContext machineContext = _workflowService.Start(@"testStateMachine.boo", user, "CurrentStatus");

			Assert.IsNotNull(machineContext);
			Assert.IsTrue(machineContext.IsStarted);

			Assert.AreEqual(user.CurrentStatus, machineContext.CurrentState.StateIdentifier);
			Assert.AreEqual(1, _userStateChangedCount);

			_workflowPersister.Expect(p => p.Load(new Guid())).IgnoreArguments().Return(toReturn);

			IStateMachineContext restoredMachine = _workflowService.Load(machineContext.Id);

			Assert.IsAssignableFrom(typeof (ReflectiveDomainContextWrapper), restoredMachine.DomainContext);

			var contextWrapper = (ReflectiveDomainContextWrapper) restoredMachine.DomainContext;

			Assert.IsAssignableFrom(typeof (User), contextWrapper.DomainContext);

			// should be the newly loaded user.
			Assert.AreNotEqual(contextWrapper.DomainContext, user);
			Assert.AreNotEqual(((User) contextWrapper.DomainContext).Id, user.Id);

			// state changed should not have been called!
			// is it up to the domain context repository to restore the state of the domain context.
			Assert.AreEqual(1, _userStateChangedCount);

			_domainContextRepository.VerifyAllExpectations();
			_workflowPersister.VerifyAllExpectations();
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void ThrowWhenNotFound()
		{
			_workflowService.Load(Guid.Empty);
		}

		[Test]
		public void CanCreateSaveUpdateAndComplete() 
		{
			IWorkflowEntity toReturn = new WorkflowEntity();
			_workflowPersister.Expect(p => p.CreateEmptyWorkflowEntity(Guid.Empty)).IgnoreArguments().Repeat.Times(1).Return(toReturn);
			_workflowPersister.Expect(p => p.Load(Guid.Empty)).IgnoreArguments().Repeat.Times(2).Return(toReturn);
			_workflowPersister.Expect(p => p.Save(toReturn));
			_workflowPersister.Expect(p => p.Update(toReturn));
			_workflowPersister.Expect(p => p.Complete(toReturn));

            IStateMachineContext machineContext = _workflowService.Start(@"simplestatemachine.boo");

			Assert.IsNotNull(machineContext);
			Assert.IsTrue(machineContext.IsStarted);

			Assert.AreEqual("Initial", machineContext.CurrentState.StateName);

			// should trigger the Update
			machineContext.HandleEvent(1);

			Assert.AreEqual("First", machineContext.CurrentState.StateName);

			// should trigger the Complete
			machineContext.HandleEvent(3);

			_workflowPersister.VerifyAllExpectations();
		}

		[Test]
		public void IdOfContextMatchesIdOfLoadedMachineTest()
		{
			IWorkflowEntity toReturn = new WorkflowEntity();
			_workflowPersister.Expect(p => p.CreateEmptyWorkflowEntity(Guid.Empty)).IgnoreArguments().Return(toReturn);

			_workflowPersister.Expect(p => p.Save(new WorkflowEntity())).IgnoreArguments().Callback<IWorkflowEntity>(x =>
			{
				toReturn = x;
				return true;
			});

			IStateMachineContext machineContext = _workflowService.Start(@"testStateMachine.boo");

			Assert.IsNotNull(machineContext);
			Assert.IsTrue(machineContext.IsStarted);

			_workflowPersister.Expect(p => p.Load(new Guid())).IgnoreArguments().Return(toReturn);

			IStateMachineContext restoredMachine = _workflowService.Load(machineContext.Id);

			Assert.AreEqual(restoredMachine.Id, machineContext.Id);
		}
	}
}