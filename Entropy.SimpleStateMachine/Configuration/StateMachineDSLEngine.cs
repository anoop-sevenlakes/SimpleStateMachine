using System.Collections.Generic;
using System.Reflection;
using Boo.Lang.Compiler;
using Rhino.DSL;

namespace Entropy.SimpleStateMachine.Configuration
{
    public class StateMachineDSLEngine : DslEngine
    {
        public StateMachineDSLEngine()
        {
            ImportedNamespaces = new List<string>();
            ReferencedAssemblies = new List<Assembly>();
        }

        public List<string> ImportedNamespaces { get; private set; }
        public List<Assembly> ReferencedAssemblies { get; private set; }


        protected override void CustomizeCompiler(BooCompiler compiler, CompilerPipeline pipeline, string[] urls)
        {
            foreach (Assembly asm in ReferencedAssemblies)
                compiler.Parameters.AddAssembly(asm);

            pipeline.Insert(1,new ImplicitBaseClassCompilerStep(typeof (StateMachineBuilder), "Prepare",
                                                               ImportedNamespaces.ToArray()));
            pipeline.Insert(2, new UseSymbolsStep());
        }
    }
}