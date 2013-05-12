namespace Entropy.SimpleStateMachine.Tests
{
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class ReflectiveDomainContextWrapperTestFixture
	{
		public enum State
		{
			First,
			Second
		}

		public class Test
		{
			public State State { get; set; }
		}

		[Test]
		public void EnumStateTest()
		{
			Test context = new Test();
			var wrapper = new ReflectiveDomainContextWrapper(context, "State");

			StateMachine machine = new StateMachine();
			wrapper.AcceptNewState(new StateMachineState(machine, "First", "First"));

			Assert.That(context.State, Is.EqualTo(State.First));
		}
	}
}