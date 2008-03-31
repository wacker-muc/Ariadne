namespace SWA.Ariadne.App
{
    partial class ScreenSaverForm
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
            this.outerInfoPanel = new System.Windows.Forms.Panel();
            this.innerInfoPanel = new System.Windows.Forms.Panel();
            this.infoLabelStatus = new System.Windows.Forms.Label();
            this.infoLabelCaption = new System.Windows.Forms.Label();
            this.outerInfoPanel.SuspendLayout();
            this.innerInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mazeUserControl
            // 
            this.mazeUserControl.Size = new System.Drawing.Size(609, 213);
            // 
            // outerInfoPanel
            // 
            this.outerInfoPanel.Controls.Add(this.innerInfoPanel);
            this.outerInfoPanel.Location = new System.Drawing.Point(151, 231);
            this.outerInfoPanel.Name = "outerInfoPanel";
            this.outerInfoPanel.Padding = new System.Windows.Forms.Padding(3);
            this.outerInfoPanel.Size = new System.Drawing.Size(425, 44);
            this.outerInfoPanel.TabIndex = 30;
            // 
            // innerInfoPanel
            // 
            this.innerInfoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.innerInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.innerInfoPanel.Controls.Add(this.infoLabelStatus);
            this.innerInfoPanel.Controls.Add(this.infoLabelCaption);
            this.innerInfoPanel.Location = new System.Drawing.Point(3, 3);
            this.innerInfoPanel.Name = "innerInfoPanel";
            this.innerInfoPanel.Padding = new System.Windows.Forms.Padding(2);
            this.innerInfoPanel.Size = new System.Drawing.Size(419, 38);
            this.innerInfoPanel.TabIndex = 27;
            // 
            // infoLabelStatus
            // 
            this.infoLabelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabelStatus.Location = new System.Drawing.Point(2, 17);
            this.infoLabelStatus.Margin = new System.Windows.Forms.Padding(0);
            this.infoLabelStatus.Name = "infoLabelStatus";
            this.infoLabelStatus.Size = new System.Drawing.Size(411, 17);
            this.infoLabelStatus.TabIndex = 22;
            this.infoLabelStatus.Text = "Status";
            this.infoLabelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // infoLabelCaption
            // 
            this.infoLabelCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabelCaption.Location = new System.Drawing.Point(2, 1);
            this.infoLabelCaption.Margin = new System.Windows.Forms.Padding(0);
            this.infoLabelCaption.Name = "infoLabelCaption";
            this.infoLabelCaption.Size = new System.Drawing.Size(411, 16);
            this.infoLabelCaption.TabIndex = 21;
            this.infoLabelCaption.Text = "Caption";
            this.infoLabelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ScreenSaverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 300);
            this.Controls.Add(this.outerInfoPanel);
            this.Name = "ScreenSaverForm";
            this.Text = "ScreenSaverForm";
            this.Load += new System.EventHandler(this.ScreenSaverForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScreenSaverForm_MouseDown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScreenSaverForm_KeyDown);
            this.Controls.SetChildIndex(this.outerInfoPanel, 0);
            this.Controls.SetChildIndex(this.mazeUserControl, 0);
            this.outerInfoPanel.ResumeLayout(false);
            this.innerInfoPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel outerInfoPanel;
        private System.Windows.Forms.Panel innerInfoPanel;
        private System.Windows.Forms.Label infoLabelStatus;
        private System.Windows.Forms.Label infoLabelCaption;

    }
}