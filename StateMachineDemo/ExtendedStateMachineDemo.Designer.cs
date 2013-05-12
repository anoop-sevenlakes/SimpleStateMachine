using StateMachineHost=StateMachineDemo.Controls.StateMachineHost;

namespace StateMachineDemo
{
    partial class ExtendedStateMachineDemo
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtendedStateMachineDemo));
            this.stateMachineHost1 = new StateMachineDemo.Controls.StateMachineHost();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this._orderProps = new System.Windows.Forms.PropertyGrid();
            this.panel2 = new System.Windows.Forms.Panel();
            this._stateMachineTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this._output = new System.Windows.Forms.TextBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stateMachineHost1
            // 
            this.stateMachineHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateMachineHost1.Location = new System.Drawing.Point(0, 3);
            this.stateMachineHost1.Name = "stateMachineHost1";
            this.stateMachineHost1.Size = new System.Drawing.Size(438, 363);
            this.stateMachineHost1.TabIndex = 0;
            this.stateMachineHost1.WorkflowDirectory = "";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.stateMachineHost1);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this._orderProps);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(438, 489);
            this.panel1.TabIndex = 1;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(438, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // _orderProps
            // 
            this._orderProps.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._orderProps.HelpVisible = false;
            this._orderProps.Location = new System.Drawing.Point(0, 366);
            this._orderProps.Name = "_orderProps";
            this._orderProps.Size = new System.Drawing.Size(438, 123);
            this._orderProps.TabIndex = 0;
            this._orderProps.ToolbarVisible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitter2);
            this.panel2.Controls.Add(this._stateMachineTree);
            this.panel2.Controls.Add(this._output);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(438, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(437, 489);
            this.panel2.TabIndex = 2;
            // 
            // _stateMachineTree
            // 
            this._stateMachineTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._stateMachineTree.ImageIndex = 0;
            this._stateMachineTree.ImageList = this.imageList1;
            this._stateMachineTree.Location = new System.Drawing.Point(0, 0);
            this._stateMachineTree.Name = "_stateMachineTree";
            this._stateMachineTree.SelectedImageIndex = 0;
            this._stateMachineTree.Size = new System.Drawing.Size(437, 293);
            this._stateMachineTree.TabIndex = 1;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "refresh_24.png");
            this.imageList1.Images.SetKeyName(1, "play_24.png");
            this.imageList1.Images.SetKeyName(2, "reload_config_24.png");
            // 
            // _output
            // 
            this._output.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._output.Location = new System.Drawing.Point(0, 293);
            this._output.Multiline = true;
            this._output.Name = "_output";
            this._output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._output.Size = new System.Drawing.Size(437, 196);
            this._output.TabIndex = 0;
            this._output.Text = "abc";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 290);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(437, 3);
            this.splitter2.TabIndex = 2;
            this.splitter2.TabStop = false;
            // 
            // ExtendedStateMachineDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 489);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ExtendedStateMachineDemo";
            this.Text = "Exteneded State Machine Demo";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private StateMachineHost stateMachineHost1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.PropertyGrid _orderProps;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.TreeView _stateMachineTree;
        private System.Windows.Forms.TextBox _output;
        private System.Windows.Forms.ImageList imageList1;
    }
}