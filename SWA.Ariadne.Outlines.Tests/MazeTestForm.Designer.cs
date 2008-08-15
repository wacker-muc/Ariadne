namespace SWA.Ariadne.Outlines.Tests
{
    partial class MazeTestForm
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
            this.newButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.notOkButton = new System.Windows.Forms.Button();
            this.gridWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.mazeUserControl = new SWA.Ariadne.Gui.Mazes.MazeUserControl();
            ((System.ComponentModel.ISupportInitialize)(this.gridWidthNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // newButton
            // 
            this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newButton.Location = new System.Drawing.Point(12, 247);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(75, 23);
            this.newButton.TabIndex = 1;
            this.newButton.Text = "New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(223, 247);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // notOkButton
            // 
            this.notOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.notOkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.notOkButton.Location = new System.Drawing.Point(304, 247);
            this.notOkButton.Name = "notOkButton";
            this.notOkButton.Size = new System.Drawing.Size(75, 23);
            this.notOkButton.TabIndex = 3;
            this.notOkButton.Text = "Not OK";
            this.notOkButton.UseVisualStyleBackColor = true;
            // 
            // gridWidthNumericUpDown
            // 
            this.gridWidthNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gridWidthNumericUpDown.Location = new System.Drawing.Point(161, 249);
            this.gridWidthNumericUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.gridWidthNumericUpDown.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.gridWidthNumericUpDown.Name = "gridWidthNumericUpDown";
            this.gridWidthNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.gridWidthNumericUpDown.TabIndex = 4;
            this.gridWidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.gridWidthNumericUpDown.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 252);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Grid Width";
            // 
            // mazeUserControl
            // 
            this.mazeUserControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mazeUserControl.Location = new System.Drawing.Point(12, 12);
            this.mazeUserControl.MazeForm = null;
            this.mazeUserControl.Name = "mazeUserControl";
            this.mazeUserControl.Size = new System.Drawing.Size(368, 220);
            this.mazeUserControl.TabIndex = 0;
            this.mazeUserControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mazeUserControl_MouseClick);
            // 
            // MazeTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 273);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gridWidthNumericUpDown);
            this.Controls.Add(this.notOkButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.mazeUserControl);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "MazeTestForm";
            this.Text = "MazeTestForm";
            ((System.ComponentModel.ISupportInitialize)(this.gridWidthNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SWA.Ariadne.Gui.Mazes.MazeUserControl mazeUserControl;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button notOkButton;
        private System.Windows.Forms.NumericUpDown gridWidthNumericUpDown;
        private System.Windows.Forms.Label label1;
    }
}