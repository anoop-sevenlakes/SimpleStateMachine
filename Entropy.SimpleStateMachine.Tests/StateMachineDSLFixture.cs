using System;
using Entropy.SimpleStateMachine.Configuration;
using NUnit.Framework;
using Rhino.DSL;

namespace Entropy.SimpleStateMachine.Tests
{
    public enum MyCustomEnum
    {
        One,
        Chicken,
        Freedom
    }

    [TestFixture]
    public class StateMachineDSLFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            factory = new DslFactory
                          {
                              BaseDirectory = AppDomain.CurrentDomain.BaseDirectory
                          };

            var engine = new StateMachineDSLEngine();
            engine.ImportedNamespaces.Add("Entropy.SimpleStateMachine.Tests");
            engine.ReferencedAssemblies.Add(GetType().Assembly);
            factory.Register<StateMachineBuilder>(engine);
        }

        #endregion

        private DslFactory factory;

        [Test]
        public void CanCompileFile()
        {
            Assert.IsNotNull(factory.Create<StateMachineBuilder>(@"testStateMachine.boo"));
        }

        [Test]
        public void CanCreateInstanceOfScheduler()
        {
            object instance = factory.Create<StateMachineBuilder>(@"testStateMachine.boo");
            Assert.IsNotNull(instance);
        }

        [Test]
        public void CanGetAllDslInstancesInDirectory()
        {
            StateMachineBuilder[] all = factory.CreateAll<StateMachineBuilder>("");
            Assert.AreEqual(3, all.Length);
        }

        [Test]
        public void PreparingInstanceWillCauseValuesToFillFromDSL()
        {
            var instance = factory.Create<StateMachineBuilder>(@"testStateMachine.boo");

            var target = new TestStateMachine();
            instance.BuildStateMachine(target);

            Assert.AreEqual(1, target.RegisteredTaskAssemblies.Count);
            Assert.AreSame(GetType().Assembly, target.RegisteredTaskAssemblies[0]);

            Assert.AreEqual(6, target.States.Length);
            Assert.AreEqual(3, target.Events.Length);

            Assert.AreEqual("Event1", target.Events[0].EventName);
            Assert.AreEqual("Event2", target.Events[1].EventName);
            Assert.AreEqual("Event3", target.Events[2].EventName);

            Assert.IsTrue(target.Events[0].Matches("My/Event/Path"));
            Assert.IsTrue(target.Events[2].Matches(TypeCode.String));
            Assert.IsTrue(target.Events[1].Matches("Event2"));

            Assert.AreEqual("Initial", target.States[0].StateName);
            Assert.AreEqual("haHaInitialState", target.States[0].StateIdentifier);

            Assert.AreEqual("First", target.States[1].StateName);
            Assert.AreEqual(typeof (string), target.States[1].StateIdentifier);

            Assert.AreEqual("NoTransitions", target.States[3].StateName);
            Assert.AreEqual("NoTransitions", target.States[3].StateIdentifier);

            Assert.AreEqual("NoTransitions2", target.States[4].StateName);
            Assert.AreEqual(MyCustomEnum.Chicken, target.States[4].StateIdentifier);

            Assert.AreEqual("Second", target.States[2].StateName);

            Assert.AreEqual(2, target.States[0].Transitions.Length);
            Assert.AreEqual("Event2", target.States[0].Transitions[1].TriggerEvent.EventName);
            Assert.AreEqual("Second", target.States[0].Transitions[1].TargetState.StateName);

            Assert.AreEqual(3, target.StateTransitionActions.Length);
            Assert.AreEqual("TestyTheTest", target.StateTransitionActions[0].ActionName);
            Assert.AreEqual(StateTransitionPhase.StateMachineStarting, target.StateTransitionActions[0].TargetPhase);
            Assert.AreEqual(StateTransitionPhase.StateMachineComplete, target.StateTransitionActions[1].TargetPhase);
            Assert.AreEqual(StateTransitionPhase.StateInitialization, target.StateTransitionActions[2].TargetPhase);

            Assert.AreEqual(1, target.States[0].StateTransitionActions.Length);
            Assert.AreEqual("TestyTheTest", target.States[0].StateTransitionActions[0].ActionName);
            Assert.AreEqual(StateTransitionPhase.StateFinalization,
                            target.States[0].StateTransitionActions[0].TargetPhase);

            Assert.AreEqual(0, target.States[1].StateTransitionActions.Length);
            Assert.AreEqual(0, target.States[2].StateTransitionActions.Length);

            Assert.AreEqual("Chicken", target.States[5].StateName);
            Assert.AreEqual(MyCustomEnum.Chicken, target.States[5].StateIdentifier);

            //Event Actions
            Assert.AreEqual(1, target.EventActions.Length);
            Assert.AreEqual("TestyTheTest", target.EventActions[0].ActionName);
            Assert.AreEqual("Event2", target.EventActions[0].TriggerEvent.EventName);

            Assert.AreEqual(1, target.States[0].EventActions.Length);
            Assert.AreEqual("TestyTheTest", target.States[0].EventActions[0].ActionName);
            Assert.AreEqual("Event1", target.States[0].EventActions[0].TriggerEvent.EventName);
        }
    }
}