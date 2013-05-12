using System;
using System.Windows.Forms;

namespace StateMachineDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var demo = new BasicStateMachineDemo();
            demo.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var demo = new ExtendedStateMachineDemo();
            demo.Show();
        }
    }
}