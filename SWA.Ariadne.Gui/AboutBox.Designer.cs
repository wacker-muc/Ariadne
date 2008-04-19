namespace SWA.Ariadne.App
{
    partial class AboutBox
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
            this.okButton = new System.Windows.Forms.Button();
            this.outerAboutPanel = new System.Windows.Forms.Panel();
            this.innerAboutPanel = new System.Windows.Forms.Panel();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelProductName = new System.Windows.Forms.Label();
            this.mazeUserControl = new SWA.Ariadne.App.MazeUserControl();
            this.outerAboutPanel.SuspendLayout();
            this.innerAboutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(288, 141);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(59, 24);
            this.okButton.TabIndex = 25;
            this.okButton.Text = "&OK";
            // 
            // outerAboutPanel
            // 
            this.outerAboutPanel.Controls.Add(this.innerAboutPanel);
            this.outerAboutPanel.Location = new System.Drawing.Point(43, 49);
            this.outerAboutPanel.Name = "outerAboutPanel";
            this.outerAboutPanel.Padding = new System.Windows.Forms.Padding(3);
            this.outerAboutPanel.Size = new System.Drawing.Size(193, 60);
            this.outerAboutPanel.TabIndex = 29;
            // 
            // innerAboutPanel
            // 
            this.innerAboutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.innerAboutPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.innerAboutPanel.Controls.Add(this.labelCopyright);
            this.innerAboutPanel.Controls.Add(this.labelVersion);
            this.innerAboutPanel.Controls.Add(this.labelProductName);
            this.innerAboutPanel.Location = new System.Drawing.Point(3, 3);
            this.innerAboutPanel.Name = "innerAboutPanel";
            this.innerAboutPanel.Padding = new System.Windows.Forms.Padding(2);
            this.innerAboutPanel.Size = new System.Drawing.Size(187, 54);
            this.innerAboutPanel.TabIndex = 27;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCopyright.Location = new System.Drawing.Point(2, 33);
            this.labelCopyright.Margin = new System.Windows.Forms.Padding(0);
            this.labelCopyright.MaximumSize = new System.Drawing.Size(186, 17);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(179, 17);
            this.labelCopyright.TabIndex = 22;
            this.labelCopyright.Text = "Copyright";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.Location = new System.Drawing.Point(2, 17);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(186, 17);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(179, 17);
            this.labelVersion.TabIndex = 21;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelProductName
            // 
            this.labelProductName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProductName.Location = new System.Drawing.Point(2, 1);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(186, 17);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(179, 17);
            this.labelProductName.TabIndex = 20;
            this.labelProductName.Text = "Product Name";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mazeUserControl
            // 
            this.mazeUserControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mazeUserControl.BackColor = System.Drawing.Color.Black;
            this.mazeUserControl.Location = new System.Drawing.Point(12, 12);
            this.mazeUserControl.Name = "mazeUserControl";
            this.mazeUserControl.Size = new System.Drawing.Size(339, 157);
            this.mazeUserControl.TabIndex = 0;
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 181);
            this.ControlBox = false;
            this.Controls.Add(this.outerAboutPanel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.mazeUserControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowInTaskbar = false;
            this.Text = "AboutBox";
            this.outerAboutPanel.ResumeLayout(false);
            this.innerAboutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MazeUserControl mazeUserControl;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel outerAboutPanel;
        private System.Windows.Forms.Panel innerAboutPanel;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelProductName;
    }
}