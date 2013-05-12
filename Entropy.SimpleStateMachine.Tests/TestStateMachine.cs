using System.Collections.Generic;
using System.Reflection;

namespace Entropy.SimpleStateMachine.Tests
{
    public class TestStateMachine : StateMachine
    {
        public List<Assembly> RegisteredTaskAssemblies
        {
            get { return TaskAssemblies ?? new List<Assembly>(); }
        }
    }
}