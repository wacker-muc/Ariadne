namespace SWA.Ariadne.Gui
{
    partial class ArenaItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusLabel = new System.Windows.Forms.Label();
            this.visitedProgressBar = new System.Windows.Forms.ProgressBar();
            this.mazeUserControl = new SWA.Ariadne.Gui.MazeUserControl();
            this.strategyComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.Location = new System.Drawing.Point(127, 158);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(55, 21);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "Status";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // visitedProgressBar
            // 
            this.visitedProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.visitedProgressBar.Location = new System.Drawing.Point(188, 158);
            this.visitedProgressBar.Name = "visitedProgressBar";
            this.visitedProgressBar.Size = new System.Drawing.Size(80, 21);
            this.visitedProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.visitedProgressBar.TabIndex = 2;
            // 
            // mazeUserControl
            // 
            this.mazeUserControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mazeUserControl.BackColor = System.Drawing.Color.Black;
            this.mazeUserControl.Location = new System.Drawing.Point(0, 0);
            this.mazeUserControl.Name = "mazeUserControl";
            this.mazeUserControl.Size = new System.Drawing.Size(268, 152);
            this.mazeUserControl.TabIndex = 0;
            // 
            // strategyComboBox
            // 
            this.strategyComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.strategyComboBox.FormattingEnabled = true;
            this.strategyComboBox.Location = new System.Drawing.Point(0, 158);
            this.strategyComboBox.Name = "strategyComboBox";
            this.strategyComboBox.Size = new System.Drawing.Size(121, 21);
            this.strategyComboBox.TabIndex = 3;
            // 
            // ArenaItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.strategyComboBox);
            this.Controls.Add(this.visitedProgressBar);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.mazeUserControl);
            this.Name = "ArenaItem";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(268, 179);
            this.ResumeLayout(false);

        }

        #endregion

        private MazeUserControl mazeUserControl;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar visitedProgressBar;
        private System.Windows.Forms.ComboBox strategyComboBox;
    }
}
