using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entropy.SimpleStateMachine.TaskManagement;
using Entropy.SimpleStateMachine.TaskManagement.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Entropy.SimpleStateMachine.Tests
{
    [TestFixture]
    public class CustomExecMethodTaskWrapperFixture
    {
        [Test]
        public void ShouldExecAlternativeMethodWhenInvoked()
        {
            //Given
            CustomExecMethodTaskWrapper wrapper = new CustomExecMethodTaskWrapper(new WrappedTask(), "SmokeTest");
            
            //When
            object result = wrapper.Execute(new TaskContext(1));
        
            //Then
            Assert.That(result,Is.EqualTo(1));
        }

        [Test]
        public void ShouldPreferOverloadWithTaskContextParam()
        {
            //Given
            CustomExecMethodTaskWrapper wrapper = new CustomExecMethodTaskWrapper(new WrappedTask(), "TestMethodOne");

            //When
            object result = wrapper.Execute(new TaskContext("hi"));

            //Then
            Assert.That(result, Is.EqualTo("hi"));
        }
        
        [Test]
        public void WillBindToParamlessMethod()
        {
            //Given
            CustomExecMethodTaskWrapper wrapper = new CustomExecMethodTaskWrapper(new WrappedTask(), "TaskMethodTwo");

            //When
            object result = wrapper.Execute(new TaskContext("hi"));

            //Then
            Assert.AreEqual(result, "Task Method Two - at your service");
        }

        [Test]
        public void WillExecuteVoidTaskMethod()
        {
            //Given
            CustomExecMethodTaskWrapper wrapper = new CustomExecMethodTaskWrapper(new WrappedTask(), "TaskMethodThree");
            TaskContext context = new TaskContext("a");

            //When
            object result = wrapper.Execute(context);

            //Then
            Assert.IsNull(result);
            Assert.AreEqual(context.ContextObject, "And now for something new");
        }

        [Test][ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowExceptionWhenUsedWithUnknownMethodName()
        {
            //Given
            CustomExecMethodTaskWrapper wrapper = new CustomExecMethodTaskWrapper(new WrappedTask(), "NoNoNo");
            Exception expected = null;
            
            //When
            try
            {
                wrapper.Execute(new TaskContext("a"));
            }
            catch (Exception ex)
            {
                expected = ex;
            }

            //Then
            Assert.IsInstanceOfType(typeof(ArgumentException),expected);
        }
    }

    public class WrappedTask:TaskBase
    {
        protected override object PerformTask(TaskContext context)
        {
            throw new Exception("This should not be happening! WHY ME?!?!?!");
        }

        public object SmokeTest(TaskContext context)
        {
            return context.ContextObject;
        }

        public string TestMethodOne()
        {
            return "Don't pick me!";
        }

        public object TestMethodOne(TaskContext taskContext)
        {
            return taskContext.ContextObject;
        }

        public string TaskMethodTwo()
        {
            return "Task Method Two - at your service";
        }

        public void TaskMethodThree(TaskContext taskContext)
        {
            taskContext.ContextObject = "And now for something new";
        }
    }
}
