namespace SWA.Ariadne.Gui.Tests
{
    partial class PreviewTestForm
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
            this.previewControl = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // previewControl
            // 
            this.previewControl.Location = new System.Drawing.Point(12, 12);
            this.previewControl.Name = "previewControl";
            this.previewControl.Size = new System.Drawing.Size(268, 249);
            this.previewControl.TabIndex = 0;
            this.previewControl.Text = "button1";
            this.previewControl.UseVisualStyleBackColor = true;
            // 
            // PreviewTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.previewControl);
            this.Name = "PreviewTestForm";
            this.Text = "PreviewTestForm";
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button previewControl;
    }
}