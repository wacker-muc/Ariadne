namespace SWA.Ariadne.Gui
{
    partial class AboutDetailsForm
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
            this.textBoxFeatureLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxFeatureLog
            // 
            this.textBoxFeatureLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFeatureLog.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxFeatureLog.Location = new System.Drawing.Point(12, 12);
            this.textBoxFeatureLog.Multiline = true;
            this.textBoxFeatureLog.Name = "textBoxFeatureLog";
            this.textBoxFeatureLog.ReadOnly = true;
            this.textBoxFeatureLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxFeatureLog.Size = new System.Drawing.Size(268, 249);
            this.textBoxFeatureLog.TabIndex = 0;
            this.textBoxFeatureLog.TabStop = false;
            // 
            // AboutDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.textBoxFeatureLog);
            this.Name = "AboutDetailsForm";
            this.Text = "Ariadne Info";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFeatureLog;
    }
}