namespace SWA.Ariadne.Outlines.Tests
{
    partial class PolygonTestForm
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
            this.distortedCheckBox = new System.Windows.Forms.CheckBox();
            this.slantNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.windingsNnumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.cornersNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slantNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.windingsNnumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cornersNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.distortedCheckBox);
            this.groupBox3.Controls.Add(this.slantNumericUpDown);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.windingsNnumericUpDown);
            this.groupBox3.Controls.Add(this.cornersNumericUpDown);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(552, 207);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(125, 120);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Polygon Control";
            // 
            // distortedCheckBox
            // 
            this.distortedCheckBox.AutoSize = true;
            this.distortedCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.distortedCheckBox.Location = new System.Drawing.Point(48, 95);
            this.distortedCheckBox.Name = "distortedCheckBox";
            this.distortedCheckBox.Size = new System.Drawing.Size(66, 17);
            this.distortedCheckBox.TabIndex = 12;
            this.distortedCheckBox.Text = "distorted";
            this.distortedCheckBox.UseVisualStyleBackColor = true;
            // 
            // slantNumericUpDown
            // 
            this.slantNumericUpDown.DecimalPlaces = 1;
            this.slantNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.slantNumericUpDown.Location = new System.Drawing.Point(69, 69);
            this.slantNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.slantNumericUpDown.Name = "slantNumericUpDown";
            this.slantNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.slantNumericUpDown.TabIndex = 11;
            this.slantNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Slant";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Winding";
            // 
            // windingsNnumericUpDown
            // 
            this.windingsNnumericUpDown.Location = new System.Drawing.Point(74, 43);
            this.windingsNnumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.windingsNnumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.windingsNnumericUpDown.Name = "windingsNnumericUpDown";
            this.windingsNnumericUpDown.Size = new System.Drawing.Size(40, 20);
            this.windingsNnumericUpDown.TabIndex = 8;
            this.windingsNnumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.windingsNnumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cornersNumericUpDown
            // 
            this.cornersNumericUpDown.Location = new System.Drawing.Point(74, 17);
            this.cornersNumericUpDown.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.cornersNumericUpDown.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.cornersNumericUpDown.Name = "cornersNumericUpDown";
            this.cornersNumericUpDown.Size = new System.Drawing.Size(40, 20);
            this.cornersNumericUpDown.TabIndex = 3;
            this.cornersNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cornersNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Corners";
            // 
            // PolygonTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 573);
            this.Controls.Add(this.groupBox3);
            this.Name = "PolygonTestForm";
            this.Text = "PolygonTestForm";
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slantNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.windingsNnumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cornersNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown windingsNnumericUpDown;
        private System.Windows.Forms.NumericUpDown cornersNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox distortedCheckBox;
        private System.Windows.Forms.NumericUpDown slantNumericUpDown;
        private System.Windows.Forms.Label label3;
    }
}