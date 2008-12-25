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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.visibleCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gridWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.newButton = new System.Windows.Forms.Button();
            this.mazeUserControl = new SWA.Ariadne.Gui.Mazes.MazeUserControl();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridWidthNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.visibleCheckBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.gridWidthNumericUpDown);
            this.groupBox2.Controls.Add(this.newButton);
            this.groupBox2.Location = new System.Drawing.Point(552, 101);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(125, 100);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Maze Control";
            // 
            // visibleCheckBox
            // 
            this.visibleCheckBox.AutoSize = true;
            this.visibleCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.visibleCheckBox.Checked = true;
            this.visibleCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.visibleCheckBox.Location = new System.Drawing.Point(64, 40);
            this.visibleCheckBox.Name = "visibleCheckBox";
            this.visibleCheckBox.Size = new System.Drawing.Size(55, 17);
            this.visibleCheckBox.TabIndex = 8;
            this.visibleCheckBox.Text = "visible";
            this.visibleCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Grid Width";
            // 
            // gridWidthNumericUpDown
            // 
            this.gridWidthNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gridWidthNumericUpDown.Location = new System.Drawing.Point(79, 14);
            this.gridWidthNumericUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.gridWidthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.gridWidthNumericUpDown.Name = "gridWidthNumericUpDown";
            this.gridWidthNumericUpDown.Size = new System.Drawing.Size(40, 20);
            this.gridWidthNumericUpDown.TabIndex = 6;
            this.gridWidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.gridWidthNumericUpDown.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // newButton
            // 
            this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newButton.Location = new System.Drawing.Point(25, 71);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(73, 23);
            this.newButton.TabIndex = 2;
            this.newButton.Text = "New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // mazeUserControl
            // 
            this.mazeUserControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mazeUserControl.Location = new System.Drawing.Point(12, 12);
            this.mazeUserControl.MazeForm = null;
            this.mazeUserControl.Name = "mazeUserControl";
            this.mazeUserControl.Size = new System.Drawing.Size(523, 523);
            this.mazeUserControl.TabIndex = 0;
            this.mazeUserControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mazeUserControl_MouseClick);
            // 
            // MazeTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 547);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.mazeUserControl);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "MazeTestForm";
            this.Text = "MazeTestForm";
            this.Load += new System.EventHandler(this.MazeTestForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridWidthNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SWA.Ariadne.Gui.Mazes.MazeUserControl mazeUserControl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown gridWidthNumericUpDown;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.CheckBox visibleCheckBox;
    }
}