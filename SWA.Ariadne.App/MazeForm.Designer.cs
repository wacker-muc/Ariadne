namespace SWA.Ariadne.App
{
    partial class MazeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MazeForm));
            this.menuButton = new System.Windows.Forms.ToolStripSplitButton();
            this.resetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.resetLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.startLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.pauseLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.stepLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mazeUserControl = new SWA.Ariadne.App.MazeUserControl();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuButton
            // 
            this.menuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.menuButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetMenuItem,
            this.newMenuItem,
            this.startMenuItem});
            this.menuButton.Image = ((System.Drawing.Image)(resources.GetObject("menuButton.Image")));
            this.menuButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuButton.Name = "menuButton";
            this.menuButton.Size = new System.Drawing.Size(32, 20);
            this.menuButton.Text = "toolStripSplitButton1";
            this.menuButton.ToolTipText = "Click for menu";
            // 
            // resetMenuItem
            // 
            this.resetMenuItem.Name = "resetMenuItem";
            this.resetMenuItem.Size = new System.Drawing.Size(113, 22);
            this.resetMenuItem.Text = "Reset";
            this.resetMenuItem.ToolTipText = "Restart the Maze Solver";
            this.resetMenuItem.Click += new System.EventHandler(this.OnReset);
            // 
            // newMenuItem
            // 
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.Size = new System.Drawing.Size(113, 22);
            this.newMenuItem.Text = "New";
            this.newMenuItem.ToolTipText = "Build a new maze";
            this.newMenuItem.Click += new System.EventHandler(this.OnNew);
            // 
            // startMenuItem
            // 
            this.startMenuItem.Name = "startMenuItem";
            this.startMenuItem.Size = new System.Drawing.Size(113, 22);
            this.startMenuItem.Text = "Start";
            this.startMenuItem.Click += new System.EventHandler(this.OnStart);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLabel.Size = new System.Drawing.Size(38, 17);
            this.statusLabel.Text = "Status";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuButton,
            this.resetLabel,
            this.startLabel,
            this.pauseLabel,
            this.stepLabel,
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 251);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.ShowItemToolTips = true;
            this.statusStrip.Size = new System.Drawing.Size(613, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // resetLabel
            // 
            this.resetLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.resetLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
            this.resetLabel.Font = new System.Drawing.Font("Wingdings 2", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.resetLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.resetLabel.Name = "resetLabel";
            this.resetLabel.Size = new System.Drawing.Size(23, 20);
            this.resetLabel.Tag = "";
            this.resetLabel.Text = "";
            this.resetLabel.ToolTipText = "Reset";
            this.resetLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.resetLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.resetLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.resetLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.resetLabel.Click += new System.EventHandler(this.OnReset);
            // 
            // startLabel
            // 
            this.startLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.startLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
            this.startLabel.Font = new System.Drawing.Font("Wingdings 3", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.startLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.startLabel.Name = "startLabel";
            this.startLabel.Size = new System.Drawing.Size(22, 20);
            this.startLabel.Tag = "";
            this.startLabel.Text = "";
            this.startLabel.ToolTipText = "Start";
            this.startLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.startLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.startLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.startLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.startLabel.Click += new System.EventHandler(this.OnStart);
            // 
            // pauseLabel
            // 
            this.pauseLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.pauseLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
            this.pauseLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pauseLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.pauseLabel.Name = "pauseLabel";
            this.pauseLabel.Size = new System.Drawing.Size(25, 20);
            this.pauseLabel.Tag = "";
            this.pauseLabel.Text = "||";
            this.pauseLabel.ToolTipText = "Pause";
            this.pauseLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.pauseLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.pauseLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.pauseLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.pauseLabel.Click += new System.EventHandler(this.OnPause);
            // 
            // stepLabel
            // 
            this.stepLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.stepLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
            this.stepLabel.Font = new System.Drawing.Font("Wingdings 3", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.stepLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.stepLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.stepLabel.Name = "stepLabel";
            this.stepLabel.Size = new System.Drawing.Size(24, 20);
            this.stepLabel.Tag = "";
            this.stepLabel.Text = "";
            this.stepLabel.ToolTipText = "Single Step";
            this.stepLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.stepLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.stepLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.stepLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.stepLabel.Click += new System.EventHandler(this.OnStep);
            // 
            // mazeUserControl
            // 
            this.mazeUserControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mazeUserControl.BackColor = System.Drawing.Color.Black;
            this.mazeUserControl.Location = new System.Drawing.Point(12, 12);
            this.mazeUserControl.MinimumSize = new System.Drawing.Size(80, 80);
            this.mazeUserControl.Name = "mazeUserControl";
            this.mazeUserControl.Size = new System.Drawing.Size(589, 236);
            this.mazeUserControl.TabIndex = 0;
            // 
            // MazeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 273);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mazeUserControl);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(200, 240);
            this.Name = "MazeForm";
            this.Text = "Ariadne";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MazeUserControl mazeUserControl;
        private System.Windows.Forms.ToolStripSplitButton menuButton;
        private System.Windows.Forms.ToolStripMenuItem resetMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem newMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel stepLabel;
        private System.Windows.Forms.ToolStripStatusLabel startLabel;
        private System.Windows.Forms.ToolStripStatusLabel pauseLabel;
        private System.Windows.Forms.ToolStripStatusLabel resetLabel;
    }
}

