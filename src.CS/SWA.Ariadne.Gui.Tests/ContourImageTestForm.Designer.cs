namespace SWA.Ariadne.Gui.Tests
{
    partial class ContourImageTestForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.showContourButton = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.timeLabel);
            this.groupBox3.Controls.Add(this.showContourButton);
            this.groupBox3.Location = new System.Drawing.Point(552, 208);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(128, 126);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "groupBox3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(89, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "ms";
            // 
            // timeLabel
            // 
            this.timeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.timeLabel.BackColor = System.Drawing.SystemColors.Control;
            this.timeLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeLabel.Location = new System.Drawing.Point(15, 85);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Padding = new System.Windows.Forms.Padding(2);
            this.timeLabel.Size = new System.Drawing.Size(73, 23);
            this.timeLabel.TabIndex = 3;
            this.timeLabel.Text = "00000";
            this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // showContourButton
            // 
            this.showContourButton.Location = new System.Drawing.Point(15, 19);
            this.showContourButton.Name = "showContourButton";
            this.showContourButton.Size = new System.Drawing.Size(94, 23);
            this.showContourButton.TabIndex = 0;
            this.showContourButton.Text = "Show Contour";
            this.showContourButton.UseVisualStyleBackColor = true;
            this.showContourButton.Click += new System.EventHandler(this.showContourButton_Click);
            // 
            // ContourImageTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 573);
            this.Controls.Add(this.groupBox3);
            this.Name = "ContourImageTestForm";
            this.Text = "ContourImageTestForm";
            this.Controls.SetChildIndex(this.imageButton, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button showContourButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label timeLabel;
    }
}