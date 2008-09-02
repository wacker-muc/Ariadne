namespace SWA.Ariadne.Gui.Tests
{
    partial class ImageTestForm
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
            this.newImageButton = new System.Windows.Forms.Button();
            this.imageButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.newImageButton);
            this.groupBox2.Location = new System.Drawing.Point(552, 102);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(125, 100);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image Control";
            // 
            // newImageButton
            // 
            this.newImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newImageButton.Location = new System.Drawing.Point(25, 71);
            this.newImageButton.Name = "newImageButton";
            this.newImageButton.Size = new System.Drawing.Size(75, 23);
            this.newImageButton.TabIndex = 0;
            this.newImageButton.Text = "New";
            this.newImageButton.UseVisualStyleBackColor = true;
            this.newImageButton.Click += new System.EventHandler(this.newImageButton_Click);
            // 
            // imageButton
            // 
            this.imageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.imageButton.Location = new System.Drawing.Point(13, 13);
            this.imageButton.Name = "imageButton";
            this.imageButton.Size = new System.Drawing.Size(524, 522);
            this.imageButton.TabIndex = 8;
            this.imageButton.Text = "(image)";
            this.imageButton.UseVisualStyleBackColor = true;
            // 
            // ImageTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 547);
            this.Controls.Add(this.imageButton);
            this.Controls.Add(this.groupBox2);
            this.Name = "ImageTestForm";
            this.Text = "ImageTestForm";
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.imageButton, 0);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button newImageButton;
        protected System.Windows.Forms.Button imageButton;
    }
}