using System;
using Entropy.SimpleStateMachine.Configuration;
using Entropy.SimpleStateMachine.Interfaces;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.DSL;

namespace Entropy.SimpleStateMachine.Tests
{
    [TestFixture]
    public class StateMachineTestFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _factory = new DslFactory
                           {
                               BaseDirectory = AppDomain.CurrentDomain.BaseDirectory
                           };
            _factory.Register<StateMachineBuilder>(new StateMachineDSLEngine());
        }

        #endregion

        private bool _cancel;
        private int _beforeCount;
        private int _afterCount;
        private DslFactory _factory;

        private void context_AfterStateChanged(object sender, StateChangedEventArgs e)
        {
            _afterCount++;
        }

        private void context_BeforeStateChanged(object sender, StateChangingEventArgs e)
        {
            _beforeCount++;
            if (_cancel)
                e.Cancel = _cancel;
        }

        [Test]
        public void ContextEventTest()
        {
            //Use just this assembly to load tasks from
            var stateMachine = new StateMachine();

            IStateMachineEvent e1 = stateMachine.AddNewEvent("Event1");
            IStateMachineEvent e2 = stateMachine.AddNewEvent("Event2");

            var initial = stateMachine.AddNewState("Initial") as StateMachineState;
            var first = stateMachine.AddNewState("First") as StateMachineState;

            initial.AddNewTransition(e1, first);

            first.AddNewTransition(e1, initial);
            first.AddNewTransition(e2, first);

            var context = new StateMachineContext(stateMachine, null, null);
            context.Start();

            _beforeCount = 0;
            _afterCount = 0;
            _cancel = true;

            context.BeforeStateChanged += context_BeforeStateChanged;
            context.AfterStateChanged += context_AfterStateChanged;

            context.HandleEvent(e1);
            Assert.That(_beforeCount, Is.EqualTo(1));
            Assert.That(_afterCount, Is.EqualTo(0));
            Assert.That(context.CurrentState, Is.SameAs(initial));

            _beforeCount = 0;
            _afterCount = 0;
            _cancel = false;

            context.HandleEvent("Event1");

            Assert.That(_beforeCount, Is.EqualTo(1));
            Assert.That(_afterCount, Is.EqualTo(1));
            Assert.That(context.CurrentState, Is.SameAs(first));

            context.HandleEvent(e2);

            Assert.That(_beforeCount, Is.EqualTo(2));
            Assert.That(_afterCount, Is.EqualTo(2));
            Assert.That(context.CurrentState, Is.SameAs(first));
        }

        [Test]
        public void CreateAndRunSimpleStateMachineTest()
        {
            var stateMachine = new StateMachine();

            IStateMachineEvent e1 = stateMachine.AddNewEvent("Event1");
            IStateMachineEvent e2 = stateMachine.AddNewEvent("Event2");

            var initial = stateMachine.AddNewState("Initial") as StateMachineState;
            var first = stateMachine.AddNewState("First") as StateMachineState;
            var second = stateMachine.AddNewState("Second") as StateMachineState;

            initial.AddNewTransition(e1, first);
            initial.AddNewTransition(e2, second);

            first.AddNewTransition(e1, initial);
            first.AddNewTransition(e2, second);

            second.AddNewTransition(e1, initial);
            second.AddNewTransition(e2, first);

            var context = new StateMachineContext(stateMachine, "Initial", null);
            context.Start();

            context.HandleEvent(e1);
            Assert.That(context.CurrentState, Is.SameAs(first));

            context.HandleEvent(e1);
            Assert.That(context.CurrentState, Is.SameAs(initial));

            context.HandleEvent(e2);
            Assert.That(context.CurrentState, Is.SameAs(second));

            context.HandleEvent(e2);
            Assert.That(context.CurrentState, Is.SameAs(first));

            context.HandleEvent(e2);
            Assert.That(context.CurrentState, Is.SameAs(second));

            context.HandleEvent(e1);
            Assert.That(context.CurrentState, Is.SameAs(initial));
        }

        [Test]
        public void StateTransitionedToSameStateTest()
        {
            var stateMachine = new StateMachine();

            IStateMachineEvent e1 = stateMachine.AddNewEvent("Event1");
            IStateMachineEvent e2 = stateMachine.AddNewEvent("Event2");

            var initial = stateMachine.AddNewState("Initial") as StateMachineState;
            var first = stateMachine.AddNewState("First") as StateMachineState;

            initial.AddNewTransition(e1, first);

            first.AddNewTransition(e1, initial);
            first.AddNewTransition(e2, first);

            var context = new StateMachineContext(stateMachine, initial, null);
            context.Start();

            context.HandleEvent(e2);
            Assert.That(context.CurrentState, Is.SameAs(initial));

            context.HandleEvent(e1);
            Assert.That(context.CurrentState, Is.SameAs(first));

            context.HandleEvent(e2);
            Assert.That(context.CurrentState, Is.SameAs(first));
        }

        [Test]
        public void TransitionActionTest()
        {
            var stateMachine = new StateMachine();
			
            stateMachine.RegisterTaskAssembly(GetType().Assembly);

            stateMachine.AddStateInitializationAction("TestyTheTest");

            IStateMachineEvent e1 = stateMachine.AddNewEvent("Event1");
            IStateMachineEvent e2 = stateMachine.AddNewEvent("Event2");


            var initial = stateMachine.AddNewState("Initial") as StateMachineState;
            var first = stateMachine.AddNewState("First") as StateMachineState;

            initial.AddNewTransition(e1, first);

            first.AddNewTransition(e1, initial);
            first.AddNewTransition(e2, first);

            first.AddStateFinalizationAction("TestyTheTest::AlternateMethod");

            var context = new StateMachineContext(stateMachine, null, null);
            context.Start();

            TestStateMachineTask.Clear();

            context.HandleEvent(e1);

            Assert.That(TestStateMachineTask.PhaseHistory.Count, Is.EqualTo(1));

            context.HandleEvent(e2);

            Assert.That(TestStateMachineTask.PhaseHistory.Count, Is.EqualTo(3));

            //Global init, Local Finalize, Global init
            Assert.That(TestStateMachineTask.PhaseHistory[0], Is.EqualTo(StateTransitionPhase.StateInitialization));
            Assert.That(TestStateMachineTask.PhaseHistory[1], Is.EqualTo(StateTransitionPhase.StateFinalization));
            Assert.That(TestStateMachineTask.PhaseHistory[2], Is.EqualTo(StateTransitionPhase.StateInitialization));

            //first inits, first finalizes, first inits
            Assert.That(TestStateMachineTask.StateHistory[0], Is.SameAs(first));
            Assert.That(TestStateMachineTask.StateHistory[1], Is.SameAs(first));
            Assert.That(TestStateMachineTask.StateHistory[1], Is.SameAs(first));

            context.HandleEvent(e1);

            //first finalizes, initial inits
            Assert.That(TestStateMachineTask.StateHistory[3], Is.SameAs(first));
            Assert.That(TestStateMachineTask.StateHistory[4], Is.SameAs(initial));

            Assert.That(TestStateMachineTask.PhaseHistory[3], Is.EqualTo(StateTransitionPhase.StateFinalization));
            Assert.That(TestStateMachineTask.PhaseHistory[4], Is.EqualTo(StateTransitionPhase.StateInitialization));
        }

        [Test]
        public void TransitionActionTestFromDSL()
        {
            var stateMachine = new StateMachine();

            var builder = _factory.Create<StateMachineBuilder>("SimpleStateMachine.boo");
            builder.BuildStateMachine(stateMachine);

            var context = new StateMachineContext(stateMachine, null, null);
            context.Start();

            TestStateMachineTask.Clear();

            context.HandleEvent(1);

            Assert.That(TestStateMachineTask.PhaseHistory.Count, Is.EqualTo(1));

            context.HandleEvent(2);

            Assert.That(TestStateMachineTask.PhaseHistory.Count, Is.EqualTo(3));

            //Global init, Local Finalize, Global init
            Assert.That(TestStateMachineTask.PhaseHistory[0], Is.EqualTo(StateTransitionPhase.StateInitialization));
            Assert.That(TestStateMachineTask.PhaseHistory[1], Is.EqualTo(StateTransitionPhase.StateFinalization));
            Assert.That(TestStateMachineTask.PhaseHistory[2], Is.EqualTo(StateTransitionPhase.StateInitialization));

            //first inits, first finalizes, first inits
            Assert.That(TestStateMachineTask.StateHistory[0], Is.SameAs(stateMachine.States[1]));
            Assert.That(TestStateMachineTask.StateHistory[1], Is.SameAs(stateMachine.States[1]));
            Assert.That(TestStateMachineTask.StateHistory[1], Is.SameAs(stateMachine.States[1]));

            context.HandleEvent(1);

            //first finalizes, initial inits
            Assert.That(TestStateMachineTask.StateHistory[3], Is.SameAs(stateMachine.States[1]));
            Assert.That(TestStateMachineTask.StateHistory[4], Is.SameAs(stateMachine.States[0]));

            Assert.That(TestStateMachineTask.PhaseHistory[3], Is.EqualTo(StateTransitionPhase.StateFinalization));
            Assert.That(TestStateMachineTask.PhaseHistory[4], Is.EqualTo(StateTransitionPhase.StateInitialization));
        }

        [Test]
        public void ShouldExecuteAssignedActionWhenEventIsReceived()
        {
            var stateMachine = new StateMachine();

            stateMachine.RegisterTaskAssembly(GetType().Assembly);

            IStateMachineEvent e1 = stateMachine.AddNewEvent("Event1");//.Action("TestyTheTest");
            IStateMachineEvent e2 = stateMachine.AddNewEvent("Event2");

            stateMachine.AddNewEventAction("TestyTheTest", e1);

            var initial = stateMachine.AddNewState("Initial") as StateMachineState;
            var first = stateMachine.AddNewState("First") as StateMachineState;

            initial.AddNewEventAction("TestyTheTest", e1);


            initial.AddNewTransition(e1, first);

            first.AddNewTransition(e1, initial);
            first.AddNewTransition(e2, first);

            var context = new StateMachineContext(stateMachine, null, null);
            context.Start();

            TestStateMachineTask.Clear();

            context.HandleEvent(e1);

            Assert.That(TestStateMachineTask.PhaseHistory.Count, Is.EqualTo(2));

            context.HandleEvent(e2);

            Assert.That(TestStateMachineTask.PhaseHistory.Count, Is.EqualTo(2));

            Assert.That(TestStateMachineTask.EventHistory[0], Is.SameAs(e1));

            context.HandleEvent(e1);

            Assert.That(TestStateMachineTask.EventHistory.Count, Is.EqualTo(3));
            Assert.That(TestStateMachineTask.EventHistory[1], Is.SameAs(e1));
        }

        [Test]
        public void ShouldExecuteEventActionsWhenLoadedFromDSL()
        {
            var stateMachine = new StateMachine();

            var builder = _factory.Create<StateMachineBuilder>("EventActionTestMachine.boo");
            builder.BuildStateMachine(stateMachine);

            var context = new StateMachineContext(stateMachine, null, null);
            context.Start();

            TestStateMachineTask.Clear();

            context.HandleEvent(1);

            Assert.That(TestStateMachineTask.EventHistory.Count, Is.EqualTo(1));

            context.HandleEvent(2);

            Assert.That(TestStateMachineTask.EventHistory.Count, Is.EqualTo(2));


            context.HandleEvent(1);

            Assert.That(TestStateMachineTask.EventHistory.Count, Is.EqualTo(3));
        }

    }
}