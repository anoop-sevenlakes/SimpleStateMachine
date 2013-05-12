using System;
using System.Drawing;
using System.Windows.Forms;
using Entropy.SimpleStateMachine;
using Entropy.SimpleStateMachine.Interfaces;

namespace StateMachineDemo.Controls
{
    public partial class AvailableActionPanel : UserControl
    {
        private IStateMachineContext _workflow;

        public AvailableActionPanel()
        {
            InitializeComponent();
        }

        public IStateMachineContext Workflow
        {
            get { return _workflow; }
            set
            {
                if (_workflow != null)
                    UnBindFromWorkflow();
                BindToWorkflow(value);
            }
        }

        private void BindToWorkflow(IStateMachineContext workflow)
        {
            _workflow = workflow;
            if (_workflow != null)
            {
                _workflow.StateMachineStarted += _workflow_StateMachineStarted;
                _workflow.StateMachineComplete += _workflow_StateMachineComplete;
                _workflow.StateInitialized += _workflow_StateInitialized;

                RefreshButtonAvailability();
            }
        }

        private void UnBindFromWorkflow()
        {
            if (_workflow != null)
            {
                _workflow.StateMachineStarted -= _workflow_StateMachineStarted;
                _workflow.StateMachineComplete -= _workflow_StateMachineComplete;
                _workflow.StateInitialized -= _workflow_StateInitialized;

                _workflow = null;

                _pnlButtonContainer.Controls.Clear();
            }
        }

        private void RefreshButtonAvailability()
        {
            _pnlButtonContainer.Controls.Clear();

            if (_workflow != null)
            {
                if (_workflow.IsStarted == false)
                {
                    var startButton = new Button
                                          {
                                              Text = "Start State Machine",
                                              Width = (_pnlButtonContainer.Width - 5)
                                          };
                    startButton.Click += startButton_Click;

                    _pnlButtonContainer.Controls.Add(startButton);
                }
                else if (_workflow.IsComplete)
                {
                    var restartButton = new Button {Text = "Restart State Machine"};
                    restartButton.Click += restartButton_Click;
                    restartButton.Width = _pnlButtonContainer.Width - 5;
                    _pnlButtonContainer.Controls.Add(restartButton);

                    var label = new Label
                                    {
                                        AutoSize = false,
                                        Width = (_pnlButtonContainer.Width - 5),
                                        Text =
                                            ("Workflow Has Completed. Final State:" +
                                             _workflow.CurrentState.StateName.ExpandAtCaseTransitions()),
                                        TextAlign = ContentAlignment.MiddleCenter,
                                        BackColor = Color.YellowGreen
                                    };
                    _pnlButtonContainer.Controls.Add(label);
                }
                else
                {
                    foreach (IStateTransition transition in _workflow.CurrentState.Transitions)
                    {
                        var button = new Button
                                         {
                                             Text = transition.TriggerEvent.EventName.ExpandAtCaseTransitions(),
                                             Tag = transition.TriggerEvent.EventIdentifier,
                                             Width = (_pnlButtonContainer.Width - 5)
                                         };
                        button.Click += button_Click;
                        _pnlButtonContainer.Controls.Add(button);
                    }
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            var btn = (Button) sender;
            _workflow.HandleEvent(btn.Tag);
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            _workflow.SetState(_workflow.StateMachine.States[0]);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            _workflow.Start();
        }

        private void _workflow_StateInitialized(object sender, StateTransitionEventArgs e)
        {
            RefreshButtonAvailability();
        }

        private void _workflow_StateMachineComplete(object sender, StateTransitionEventArgs e)
        {
            RefreshButtonAvailability();
        }

        private void _workflow_StateMachineStarted(object sender, StateTransitionEventArgs e)
        {
            RefreshButtonAvailability();
        }
    }
}