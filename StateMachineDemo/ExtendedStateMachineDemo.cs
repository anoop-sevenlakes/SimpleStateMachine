using System;
using System.Windows.Forms;
using Entropy.SimpleStateMachine;
using Entropy.SimpleStateMachine.Interfaces;
using StateMachineDemo.Domain;

namespace StateMachineDemo
{
    public partial class ExtendedStateMachineDemo : Form
    {
        private readonly Order _currentOrder;

        public ExtendedStateMachineDemo()
        {
            InitializeComponent();

            _currentOrder = new Order();

            _currentOrder.StatusChanged += _currentOrder_StatusChanged;
            _currentOrder.HistoryChanged += _currentOrder_HistoryChanged;

            stateMachineHost1.WorkflowDirectory = @"WorkflowDefinitions\Extended";
            stateMachineHost1.DomainContext = new ReflectiveDomainContextWrapper(_currentOrder, "CurrentStatus");


            RefreshDisplay();

            _currentOrder.MakeHistory("Extended Demo Started");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BuildStateTree();
        }


        private void _currentOrder_HistoryChanged(object sender, EventArgs e)
        {
            if (!InvokeRequired)
            {
                _output.Text = _currentOrder.History;
                _output.Refresh();
                _output.SelectionStart = _output.Text.Length - 1;
            }
            else
            {
                EventHandler marshal = _currentOrder_HistoryChanged;
                Invoke(marshal, sender, e);
            }
        }

        private void _currentOrder_StatusChanged(object sender, EventArgs e)
        {
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            _orderProps.SelectedObject = _currentOrder;

            if (_stateMachineTree.Nodes.Count < 1)
                return;

            foreach (TreeNode node in _stateMachineTree.Nodes[0].Nodes)
            {
                if (node.Tag == stateMachineHost1.CurrentWorkflow.CurrentState)
                {
                    node.ImageIndex = 1;
                    node.Expand();
                }
                else
                {
                    node.ImageIndex = 0;
                    node.Collapse();
                }
            }
        }

        private void BuildStateTree()
        {
            _stateMachineTree.Nodes.Clear();

            if (stateMachineHost1.CurrentWorkflow == null)
                return;

            var machine = new TreeNode(stateMachineHost1.CurrentWorkflow.StateMachine.Name);
            _stateMachineTree.Nodes.Add(machine);
            foreach (IStateMachineState state in stateMachineHost1.CurrentWorkflow.StateMachine.States)
            {
                var stateNode = new TreeNode(state.StateName) {Tag = state, ImageIndex = 0};

                machine.Nodes.Add(stateNode);

                foreach (IStateTransition transition in state.Transitions)
                {
                    var transitionNode = new TreeNode(transition.ToString()) {ImageIndex = 2};
                    stateNode.Nodes.Add(transitionNode);
                }
            }
            machine.Nodes[0].ImageIndex = 1;
            machine.Nodes[0].Expand();

            machine.Expand();
        }
    }
}