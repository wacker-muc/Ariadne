namespace SWA.Ariadne.App
{
    partial class MazeForm
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
            this.mazeUserControl = new SWA.Ariadne.App.MazeUserControl();
            this.SuspendLayout();
            // 
            // mazeUserControl
            // 
            this.mazeUserControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mazeUserControl.BackColor = System.Drawing.Color.Black;
            this.mazeUserControl.Location = new System.Drawing.Point(12, 12);
            this.mazeUserControl.MinimumSize = new System.Drawing.Size(80, 80);
            this.mazeUserControl.Name = "mazeUserControl";
            this.mazeUserControl.Size = new System.Drawing.Size(268, 249);
            this.mazeUserControl.TabIndex = 0;
            // 
            // MazeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.mazeUserControl);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(200, 240);
            this.Name = "MazeForm";
            this.Text = "Ariadne";
            this.ResumeLayout(false);

        }

        #endregion

        private MazeUserControl mazeUserControl;
    }
}

