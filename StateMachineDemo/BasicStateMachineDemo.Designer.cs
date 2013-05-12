using StateMachineHost=StateMachineDemo.Controls.StateMachineHost;

namespace StateMachineDemo
{
    partial class BasicStateMachineDemo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stateMachineHost1 = new StateMachineHost();
            this.SuspendLayout();
            // 
            // stateMachineHost1
            // 
            this.stateMachineHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateMachineHost1.Location = new System.Drawing.Point(0, 0);
            this.stateMachineHost1.Name = "stateMachineHost1";
            this.stateMachineHost1.Size = new System.Drawing.Size(451, 266);
            this.stateMachineHost1.TabIndex = 0;
            this.stateMachineHost1.WorkflowDirectory = null;
            // 
            // BasicStateMachineDemo
            // 
            this.ClientSize = new System.Drawing.Size(451, 266);
            this.Controls.Add(this.stateMachineHost1);
            this.Name = "BasicStateMachineDemo";
            this.Text = "Basic State Machine Demo";
            this.ResumeLayout(false);

        }

        #endregion

        private StateMachineHost stateMachineHost1;

       
    }
}