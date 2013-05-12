namespace Entropy.SimpleStateMachine.Tests
{
	using Interfaces;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using Utility;

	[TestFixture]
	public class ServiceLocatorTestFixture
	{
		[SetUp]
		public void Setup()
		{
			ServiceLocator.Clear();
		}

		[TearDown]
		public void TearDown()
		{
			ServiceLocator.Clear();
		}

		[Test]
		public void CanRegisterInstanceTest()
		{
			ServiceLocator.RegisterService<IObjectBuilder>(MockRepository.GenerateStub<IObjectBuilder>());

			Assert.That(ServiceLocator.HasService<IObjectBuilder>(), Is.True);
			Assert.That(ServiceLocator.GetService<IObjectBuilder>().GetType(),
			            SyntaxHelper.Not.EqualTo(typeof (DefaultObjectBuilder)));
		}

		[Test]
		public void ClearAndRegisterMissingHasServiceTest()
		{
			Assert.That(ServiceLocator.HasService<IObjectBuilder>(), Is.True);
			ServiceLocator.Clear();
			Assert.That(ServiceLocator.HasService<IObjectBuilder>(), Is.True);
		}

		[Test]
		public void ClearAndRegisterMissingGetServiceTest()
		{
			Assert.That(ServiceLocator.GetService<IObjectBuilder>(), Is.Not.Null);
			ServiceLocator.Clear();
			Assert.That(ServiceLocator.GetService<IObjectBuilder>(), Is.Not.Null);
		}
	}
}