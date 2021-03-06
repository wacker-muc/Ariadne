﻿namespace SWA.Ariadne.Gui
{
    partial class AriadneFormBase
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
            this.menuButton = new System.Windows.Forms.ToolStripSplitButton();
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openArenaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.strategyComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.stepsPerSecTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.resetLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.startLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.pauseLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.stepLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.repeatLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.visitedProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.openScreenSaverMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openScreenSaverOptionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuButton
            // 
            this.menuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.menuButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.detailsMenuItem,
            this.resetMenuItem,
            this.startMenuItem,
            this.aboutMenuItem,
            this.openArenaMenuItem,
            this.openScreenSaverMenuItem,
            this.openScreenSaverOptionsMenuItem,
            this.strategyComboBox,
            this.stepsPerSecTextBox});
            this.menuButton.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonDetails;
            this.menuButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menuButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuButton.Name = "menuButton";
            this.menuButton.Size = new System.Drawing.Size(32, 20);
            this.menuButton.Text = "toolStripSplitButton1";
            this.menuButton.ToolTipText = "Click for menu [M]";
            this.menuButton.Click += new System.EventHandler(this.OnShowDropDownMenu);
            // 
            // newMenuItem
            // 
            this.newMenuItem.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonNew;
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.Size = new System.Drawing.Size(260, 22);
            this.newMenuItem.Text = "New [N]";
            this.newMenuItem.ToolTipText = "Builds a new maze";
            this.newMenuItem.Click += new System.EventHandler(this.OnNew);
            // 
            // detailsMenuItem
            // 
            this.detailsMenuItem.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonDetails;
            this.detailsMenuItem.Name = "detailsMenuItem";
            this.detailsMenuItem.Size = new System.Drawing.Size(260, 22);
            this.detailsMenuItem.Text = "Details";
            this.detailsMenuItem.ToolTipText = "Opens a window with detailled controls";
            this.detailsMenuItem.Click += new System.EventHandler(this.OnDetails);
            // 
            // resetMenuItem
            // 
            this.resetMenuItem.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonReset;
            this.resetMenuItem.Name = "resetMenuItem";
            this.resetMenuItem.Size = new System.Drawing.Size(260, 22);
            this.resetMenuItem.Text = "Reset [ESC]";
            this.resetMenuItem.ToolTipText = "Clears the Maze, stops the Solver";
            this.resetMenuItem.Click += new System.EventHandler(this.OnReset);
            // 
            // startMenuItem
            // 
            this.startMenuItem.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonStart;
            this.startMenuItem.Name = "startMenuItem";
            this.startMenuItem.Size = new System.Drawing.Size(260, 22);
            this.startMenuItem.Text = "Start [Enter]";
            this.startMenuItem.ToolTipText = "Starts the Maze Solver";
            this.startMenuItem.Click += new System.EventHandler(this.OnStart);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonAbout;
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(260, 22);
            this.aboutMenuItem.Text = "About";
            this.aboutMenuItem.ToolTipText = "Displays information about the application";
            this.aboutMenuItem.Click += new System.EventHandler(this.OnAbout);
            // 
            // openArenaMenuItem
            // 
            this.openArenaMenuItem.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonArena;
            this.openArenaMenuItem.Name = "openArenaMenuItem";
            this.openArenaMenuItem.Size = new System.Drawing.Size(260, 22);
            this.openArenaMenuItem.Text = "Open Arena";
            this.openArenaMenuItem.Click += new System.EventHandler(this.OnOpenArena);
            // 
            // openScreenSaverMenuItem
            // 
            this.openScreenSaverMenuItem.Name = "openScreenSaverMenuItem";
            this.openScreenSaverMenuItem.Size = new System.Drawing.Size(260, 22);
            this.openScreenSaverMenuItem.Text = "Open Screen Saver";
            this.openScreenSaverMenuItem.Click += new System.EventHandler(this.OnOpenScreenSaver);
            // 
            // strategyComboBox
            // 
            this.strategyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.strategyComboBox.Items.AddRange(new object[] {
            "foo",
            "bar"});
            this.strategyComboBox.Name = "strategyComboBox";
            this.strategyComboBox.Size = new System.Drawing.Size(200, 21);
            this.strategyComboBox.ToolTipText = "Selects the Solver strategy";
            this.strategyComboBox.SelectedIndexChanged += new System.EventHandler(this.strategy_SelectedIndexChanged);
            // 
            // stepsPerSecTextBox
            // 
            this.stepsPerSecTextBox.MaxLength = 5;
            this.stepsPerSecTextBox.Name = "stepsPerSecTextBox";
            this.stepsPerSecTextBox.Size = new System.Drawing.Size(60, 21);
            this.stepsPerSecTextBox.ToolTipText = "Sets the desired steps per second";
            this.stepsPerSecTextBox.TextChanged += new System.EventHandler(this.stepsPerSec_TextChanged);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLabel.Size = new System.Drawing.Size(415, 17);
            this.statusLabel.Spring = true;
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
            this.repeatLabel,
            this.statusLabel,
            this.visitedProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 251);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.ShowItemToolTips = true;
            this.statusStrip.Size = new System.Drawing.Size(675, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // resetLabel
            // 
            this.resetLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.resetLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
            this.resetLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resetLabel.Font = new System.Drawing.Font("Wingdings 2", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.resetLabel.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonReset;
            this.resetLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.resetLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.resetLabel.Name = "resetLabel";
            this.resetLabel.Size = new System.Drawing.Size(20, 20);
            this.resetLabel.Tag = "";
            this.resetLabel.Text = "";
            this.resetLabel.ToolTipText = "Reset [ESC]";
            this.resetLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.resetLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.resetLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.resetLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.resetLabel.Click += new System.EventHandler(this.OnReset);
            // 
            // startLabel
            // 
            this.startLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides) ((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.startLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
            this.startLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.startLabel.Font = new System.Drawing.Font("Wingdings 3", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.startLabel.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonStart;
            this.startLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.startLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.startLabel.Name = "startLabel";
            this.startLabel.Size = new System.Drawing.Size(20, 20);
            this.startLabel.Tag = "";
            this.startLabel.Text = "";
            this.startLabel.ToolTipText = "Start [Enter]";
            this.startLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.startLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.startLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.startLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.startLabel.Click += new System.EventHandler(this.OnStart);
            // 
            // pauseLabel
            // 
            this.pauseLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides) ((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.pauseLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
            this.pauseLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.pauseLabel.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonPause;
            this.pauseLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.pauseLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.pauseLabel.Name = "pauseLabel";
            this.pauseLabel.Size = new System.Drawing.Size(20, 20);
            this.pauseLabel.Tag = "";
            this.pauseLabel.Text = "||";
            this.pauseLabel.ToolTipText = "Pause [P]";
            this.pauseLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.pauseLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.pauseLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.pauseLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.pauseLabel.Click += new System.EventHandler(this.OnPause);
            // 
            // stepLabel
            // 
            this.stepLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides) ((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.stepLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
            this.stepLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stepLabel.Font = new System.Drawing.Font("Wingdings 3", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.stepLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.stepLabel.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonStep;
            this.stepLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.stepLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.stepLabel.Name = "stepLabel";
            this.stepLabel.Size = new System.Drawing.Size(20, 20);
            this.stepLabel.Tag = "";
            this.stepLabel.Text = "";
            this.stepLabel.ToolTipText = "Single Step [.]";
            this.stepLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.stepLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.stepLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.stepLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.stepLabel.Click += new System.EventHandler(this.OnStep);
            // 
            // repeatLabel
            // 
            this.repeatLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides) ((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.repeatLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
            this.repeatLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.repeatLabel.Font = new System.Drawing.Font("Wingdings 3", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.repeatLabel.Image = global::SWA.Ariadne.Gui.Properties.Resources.ButtonRepeat;
            this.repeatLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.repeatLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.repeatLabel.Name = "repeatLabel";
            this.repeatLabel.Size = new System.Drawing.Size(20, 20);
            this.repeatLabel.Text = "";
            this.repeatLabel.ToolTipText = "Repeat [+]";
            this.repeatLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.repeatLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.repeatLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.repeatLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.repeatLabel.Click += new System.EventHandler(this.OnRepeat);
            // 
            // visitedProgressBar
            // 
            this.visitedProgressBar.Name = "visitedProgressBar";
            this.visitedProgressBar.Size = new System.Drawing.Size(80, 16);
            this.visitedProgressBar.Step = 1;
            this.visitedProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.visitedProgressBar.ToolTipText = "Percentage of visited area";
            // 
            // openScreenSaverOptionsMenuItem
            // 
            this.openScreenSaverOptionsMenuItem.Name = "openScreenSaverOptionsMenuItem";
            this.openScreenSaverOptionsMenuItem.Size = new System.Drawing.Size(260, 22);
            this.openScreenSaverOptionsMenuItem.Text = "Open Screen Saver Options";
            this.openScreenSaverOptionsMenuItem.Click += new System.EventHandler(this.OnOpenScreenSaverOptions);
            // 
            // AriadneFormBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 273);
            this.Controls.Add(this.statusStrip);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(200, 240);
            this.Name = "AriadneFormBase";
            this.Text = "Ariadne";
            this.Load += new System.EventHandler(this.AriadneFormBase_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AriadneFormBase_FormClosing);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripSplitButton menuButton;
        private System.Windows.Forms.ToolStripMenuItem resetMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        protected System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem newMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel stepLabel;
        private System.Windows.Forms.ToolStripStatusLabel startLabel;
        private System.Windows.Forms.ToolStripStatusLabel pauseLabel;
        private System.Windows.Forms.ToolStripStatusLabel resetLabel;
        protected System.Windows.Forms.ToolStripComboBox strategyComboBox;
        private System.Windows.Forms.ToolStripTextBox stepsPerSecTextBox;
        private System.Windows.Forms.ToolStripMenuItem detailsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        protected System.Windows.Forms.ToolStripProgressBar visitedProgressBar;
        private System.Windows.Forms.ToolStripMenuItem openArenaMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel repeatLabel;
        private System.Windows.Forms.ToolStripMenuItem openScreenSaverMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openScreenSaverOptionsMenuItem;
    }
}

