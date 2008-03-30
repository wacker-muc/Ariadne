namespace SWA.Ariadne.App
{
    partial class OptionsDialog
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
            this.checkBoxBlinking = new System.Windows.Forms.CheckBox();
            this.checkBoxDetailsBox = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxStepsPerSecond = new System.Windows.Forms.TextBox();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkBoxBlinking
            // 
            this.checkBoxBlinking.AutoSize = true;
            this.checkBoxBlinking.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxBlinking.Location = new System.Drawing.Point(23, 36);
            this.checkBoxBlinking.Name = "checkBoxBlinking";
            this.checkBoxBlinking.Size = new System.Drawing.Size(134, 17);
            this.checkBoxBlinking.TabIndex = 0;
            this.checkBoxBlinking.Text = "Blinking Target Square";
            this.checkBoxBlinking.UseVisualStyleBackColor = true;
            // 
            // checkBoxDetailsBox
            // 
            this.checkBoxDetailsBox.AutoSize = true;
            this.checkBoxDetailsBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDetailsBox.Location = new System.Drawing.Point(48, 13);
            this.checkBoxDetailsBox.Name = "checkBoxDetailsBox";
            this.checkBoxDetailsBox.Size = new System.Drawing.Size(109, 17);
            this.checkBoxDetailsBox.TabIndex = 2;
            this.checkBoxDetailsBox.Text = "Show Details Box";
            this.checkBoxDetailsBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDetailsBox.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(23, 96);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(114, 96);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Steps per Second";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxStepsPerSecond
            // 
            this.textBoxStepsPerSecond.Location = new System.Drawing.Point(143, 59);
            this.textBoxStepsPerSecond.Name = "textBoxStepsPerSecond";
            this.textBoxStepsPerSecond.Size = new System.Drawing.Size(46, 20);
            this.textBoxStepsPerSecond.TabIndex = 6;
            this.textBoxStepsPerSecond.Text = "200";
            this.textBoxStepsPerSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCopyright.AutoSize = true;
            this.labelCopyright.Location = new System.Drawing.Point(71, 133);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(66, 13);
            this.labelCopyright.TabIndex = 7;
            this.labelCopyright.Text = "(c) Copyright";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(212, 148);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.textBoxStepsPerSecond);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxDetailsBox);
            this.Controls.Add(this.checkBoxBlinking);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "OptionsDialog";
            this.ShowInTaskbar = false;
            this.Text = "Ariadne Settings";
            this.Load += new System.EventHandler(this.OptionsDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxBlinking;
        private System.Windows.Forms.CheckBox checkBoxDetailsBox;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStepsPerSecond;
        private System.Windows.Forms.Label labelCopyright;
    }
}