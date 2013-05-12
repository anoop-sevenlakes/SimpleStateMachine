namespace Entropy.SimpleStateMachine.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using Configuration;
	using Interfaces;
	using Rhino.DSL;

	/// <summary>
	/// State machine file repository
	/// </summary>
	public class StateMachineService : IStateMachineService
	{
		private readonly DslFactory _factory;

		private readonly IDictionary<string, StateMachineBuilder> _machineBuilderCache =
			new Dictionary<string, StateMachineBuilder>();

		private readonly string _workflowDirectory;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateMachineService"/> class.
		/// </summary>
		/// <param name="workflowDirectory">The workflow directory.</param>
		/// <param name="importedNamespaces">The imported namespaces.</param>
		/// <param name="referencedAssemblies">The referenced assemblies.</param>
		public StateMachineService(string workflowDirectory, IEnumerable<string> importedNamespaces,
		                           IEnumerable<Assembly> referencedAssemblies)
		{
			if (workflowDirectory == null) throw new ArgumentNullException("workflowDirectory");
			_workflowDirectory = workflowDirectory;

			_factory = new DslFactory
			           	{
			           		BaseDirectory = AppDomain.CurrentDomain.BaseDirectory
			           	};

			var engine = new StateMachineDSLEngine();

			engine.ImportedNamespaces.AddRange(importedNamespaces);
			engine.ReferencedAssemblies.AddRange(referencedAssemblies);

			_factory.Register<StateMachineBuilder>(engine);
		}

		#region IStateMachineService Members

		/// <summary>
		/// Creates the state machine.
		/// </summary>
		/// <param name="stateMachineName">Name of the state machine.</param>
		/// <returns>The state machine</returns>
		public IStateMachine CreateStateMachine(string stateMachineName)
		{
			if (string.IsNullOrEmpty(stateMachineName)) throw new ArgumentNullException("stateMachineName");

			string fileName = UnifyFileName(stateMachineName);

			if (_machineBuilderCache.ContainsKey(fileName) == false)
			{
				_machineBuilderCache[fileName] = _factory.Create<StateMachineBuilder>(fileName);
			}

			StateMachineBuilder stateMachineBuilder = _machineBuilderCache[fileName];

			var machine = new StateMachine {Tag = stateMachineName};
			stateMachineBuilder.BuildStateMachine(machine);

			return machine;
		}

		#endregion

		/// <summary>
		/// Unifies the name of the file.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>the unified name.</returns>
		private string UnifyFileName(string name)
		{
			if (name.EndsWith(".boo") == false)
				name += ".boo";

			return Path.Combine(_workflowDirectory, name);
		}
	}
}