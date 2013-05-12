namespace StateMachineDemo.Controls
{
    partial class StateMachineHost
    {
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.availableActionPanel1 = new StateMachineDemo.Controls.AvailableActionPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this._stateMachinePicker = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._currentStateName = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // availableActionPanel1
            // 
            this.availableActionPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availableActionPanel1.Location = new System.Drawing.Point(0, 94);
            this.availableActionPanel1.Name = "availableActionPanel1";
            this.availableActionPanel1.Size = new System.Drawing.Size(449, 172);
            this.availableActionPanel1.TabIndex = 0;
            this.availableActionPanel1.Workflow = null;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._stateMachinePicker);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(449, 33);
            this.panel1.TabIndex = 1;
            // 
            // _stateMachinePicker
            // 
            this._stateMachinePicker.FormattingEnabled = true;
            this._stateMachinePicker.Location = new System.Drawing.Point(133, 6);
            this._stateMachinePicker.Name = "_stateMachinePicker";
            this._stateMachinePicker.Size = new System.Drawing.Size(300, 21);
            this._stateMachinePicker.TabIndex = 1;
            this._stateMachinePicker.SelectedIndexChanged += new System.EventHandler(this._stateMachinePicker_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose a state machine:";
            // 
            // _currentStateName
            // 
            this._currentStateName.BackColor = System.Drawing.Color.Black;
            this._currentStateName.Dock = System.Windows.Forms.DockStyle.Top;
            this._currentStateName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._currentStateName.ForeColor = System.Drawing.Color.Lime;
            this._currentStateName.Location = new System.Drawing.Point(0, 33);
            this._currentStateName.Name = "_currentStateName";
            this._currentStateName.Size = new System.Drawing.Size(449, 61);
            this._currentStateName.TabIndex = 2;
            this._currentStateName.Text = "State Machine Not Started";
            this._currentStateName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StateMachineHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.availableActionPanel1);
            this.Controls.Add(this._currentStateName);
            this.Controls.Add(this.panel1);
            this.Name = "StateMachineHost";
            this.Size = new System.Drawing.Size(449, 266);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private StateMachineDemo.Controls.AvailableActionPanel availableActionPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox _stateMachinePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _currentStateName;

        #endregion
    }
}