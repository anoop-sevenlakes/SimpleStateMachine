using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Entropy.SimpleStateMachine;
using Entropy.SimpleStateMachine.Configuration;
using Entropy.SimpleStateMachine.Interfaces;
using Rhino.DSL;
using StateMachineDemo.Domain;

namespace StateMachineDemo.Controls
{
    public partial class StateMachineHost : UserControl
    {
        private readonly Dictionary<string, IStateMachine> _machines = new Dictionary<string, IStateMachine>();
        private IStateMachineContext _currentWorkflow;

        public StateMachineHost()
        {
            InitializeComponent();
        }

        public IStateMachineContext CurrentWorkflow
        {
            get { return _currentWorkflow; }
        }

        public string WorkflowDirectory { get; set; }

        public IStateMachineDomainContext DomainContext { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode)
                return;


            base.OnLoad(e);
            var factory = new DslFactory
                              {
                                  BaseDirectory = AppDomain.CurrentDomain.BaseDirectory
                              };

            var engine = new StateMachineDSLEngine();

            engine.ImportedNamespaces.Add("StateMachineDemo.Domain");
            engine.ReferencedAssemblies.Add(typeof (Order).Assembly);

            factory.Register<StateMachineBuilder>(engine);

            if (String.IsNullOrEmpty(WorkflowDirectory))
                WorkflowDirectory = @"WorkflowDefinitions\Basic";

            StateMachineBuilder[] machines = factory.CreateAll<StateMachineBuilder>(WorkflowDirectory);
            foreach (StateMachineBuilder builder in machines)
            {
                var newMachine = new StateMachine();
                
                //As an alternative to using the task_assembly "assemblyname" 
                //feature of the DSL, you can manually
                //register any assemblites that contain StateTransitionTasks. 
                //This step can be skipped, but then all loaded assemblies
                //will be checked for tasks and if your project has a lot
                //of references, this will be a lot of wasted CPU cycles.
                
                //In this case, we are using the DSL to register the assembly
                //newMachine.RegisterTaskAssembly(GetType().Assembly);

                builder.BuildStateMachine(newMachine);
                _machines.Add(newMachine.Name, newMachine);
                _stateMachinePicker.Items.Add(newMachine.Name);
            }
            if (_stateMachinePicker.Items.Count > 0)
                _stateMachinePicker.SelectedIndex = 0;
        }

        private void BindToStateMachine(IStateMachine stateMachine)
        {
            if (_currentWorkflow != null)
            {
                if (_currentWorkflow.StateMachine == stateMachine)
                    return;

                UnBindFromWorkflow();
            }

            if (stateMachine != null)
            {
                var newContext = new StateMachineContext(stateMachine, (object) DomainContext ?? (object) this);
                BindToWorkflow(newContext);
            }
        }

        private void BindToWorkflow(IStateMachineContext workflow)
        {
            _currentWorkflow = workflow;
            if (_currentWorkflow != null)
            {
                availableActionPanel1.Workflow = _currentWorkflow;
                _currentWorkflow.StateInitialized += _currentWorkflow_StateInitialized;
            }
        }

        private void UnBindFromWorkflow()
        {
            if (_currentWorkflow == null)
                return;

            availableActionPanel1.Workflow = null;
            _currentWorkflow.StateInitialized -= _currentWorkflow_StateInitialized;
            _currentWorkflow = null;
        }

        private void _currentWorkflow_StateInitialized(object sender, StateTransitionEventArgs e)
        {
            if (_currentWorkflow.CurrentState != null)
                _currentStateName.Text = _currentWorkflow.CurrentState.StateName.ExpandAtCaseTransitions();
            else
                _currentStateName.Text = "State Machine Not Started";
        }

        private void _stateMachinePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_stateMachinePicker.SelectedItem != null)
            {
                _currentStateName.Text = "State Machine Not Started";
                IStateMachine machine = _machines[(string) _stateMachinePicker.SelectedItem];
                BindToStateMachine(machine);
            }
        }
    }
}