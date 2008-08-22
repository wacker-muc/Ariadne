namespace SWA.Ariadne.Outlines.Tests
{
    partial class GridTestForm
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.circlesRadioButton = new System.Windows.Forms.RadioButton();
            this.squaresRadioButton = new System.Windows.Forms.RadioButton();
            this.gridShapeHeightNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.gridShapeWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.diameterNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.checkeredCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridShapeHeightNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridShapeWidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.diameterNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkeredCheckBox);
            this.groupBox3.Controls.Add(this.diameterNumericUpDown);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.circlesRadioButton);
            this.groupBox3.Controls.Add(this.squaresRadioButton);
            this.groupBox3.Controls.Add(this.gridShapeHeightNumericUpDown);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.gridShapeWidthNumericUpDown);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(552, 207);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(125, 328);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "GridShape Control";
            // 
            // circlesRadioButton
            // 
            this.circlesRadioButton.AutoSize = true;
            this.circlesRadioButton.Location = new System.Drawing.Point(12, 149);
            this.circlesRadioButton.Name = "circlesRadioButton";
            this.circlesRadioButton.Size = new System.Drawing.Size(56, 17);
            this.circlesRadioButton.TabIndex = 5;
            this.circlesRadioButton.TabStop = true;
            this.circlesRadioButton.Text = "Circles";
            this.circlesRadioButton.UseVisualStyleBackColor = true;
            // 
            // squaresRadioButton
            // 
            this.squaresRadioButton.AutoSize = true;
            this.squaresRadioButton.Checked = true;
            this.squaresRadioButton.Location = new System.Drawing.Point(12, 125);
            this.squaresRadioButton.Name = "squaresRadioButton";
            this.squaresRadioButton.Size = new System.Drawing.Size(64, 17);
            this.squaresRadioButton.TabIndex = 4;
            this.squaresRadioButton.TabStop = true;
            this.squaresRadioButton.Text = "Squares";
            this.squaresRadioButton.UseVisualStyleBackColor = true;
            // 
            // gridShapeHeightNumericUpDown
            // 
            this.gridShapeHeightNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gridShapeHeightNumericUpDown.Location = new System.Drawing.Point(79, 47);
            this.gridShapeHeightNumericUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.gridShapeHeightNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.gridShapeHeightNumericUpDown.Name = "gridShapeHeightNumericUpDown";
            this.gridShapeHeightNumericUpDown.Size = new System.Drawing.Size(40, 20);
            this.gridShapeHeightNumericUpDown.TabIndex = 3;
            this.gridShapeHeightNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.gridShapeHeightNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Height";
            // 
            // gridShapeWidthNumericUpDown
            // 
            this.gridShapeWidthNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gridShapeWidthNumericUpDown.Location = new System.Drawing.Point(79, 21);
            this.gridShapeWidthNumericUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.gridShapeWidthNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.gridShapeWidthNumericUpDown.Name = "gridShapeWidthNumericUpDown";
            this.gridShapeWidthNumericUpDown.Size = new System.Drawing.Size(40, 20);
            this.gridShapeWidthNumericUpDown.TabIndex = 1;
            this.gridShapeWidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.gridShapeWidthNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Width";
            // 
            // diameterNumericUpDown
            // 
            this.diameterNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.diameterNumericUpDown.DecimalPlaces = 1;
            this.diameterNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.diameterNumericUpDown.Location = new System.Drawing.Point(74, 73);
            this.diameterNumericUpDown.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.diameterNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.diameterNumericUpDown.Name = "diameterNumericUpDown";
            this.diameterNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.diameterNumericUpDown.TabIndex = 7;
            this.diameterNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.diameterNumericUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Diameter";
            // 
            // checkeredCheckBox
            // 
            this.checkeredCheckBox.AutoSize = true;
            this.checkeredCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkeredCheckBox.Checked = true;
            this.checkeredCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkeredCheckBox.Location = new System.Drawing.Point(48, 99);
            this.checkeredCheckBox.Name = "checkeredCheckBox";
            this.checkeredCheckBox.Size = new System.Drawing.Size(71, 17);
            this.checkeredCheckBox.TabIndex = 8;
            this.checkeredCheckBox.Text = "checkerd";
            this.checkeredCheckBox.UseVisualStyleBackColor = true;
            // 
            // GridTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 547);
            this.Controls.Add(this.groupBox3);
            this.Name = "GridTestForm";
            this.Text = "GridTestForm";
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridShapeHeightNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridShapeWidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.diameterNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton squaresRadioButton;
        private System.Windows.Forms.NumericUpDown gridShapeHeightNumericUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown gridShapeWidthNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton circlesRadioButton;
        private System.Windows.Forms.NumericUpDown diameterNumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkeredCheckBox;
    }
}