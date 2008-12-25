namespace SWA.Ariadne.Gui
{
    partial class ArenaForm
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
            this.arenaItem1 = new SWA.Ariadne.Gui.ArenaItem();
            this.SuspendLayout();
            // 
            // arenaItem1
            // 
            this.arenaItem1.Location = new System.Drawing.Point(6, 6);
            this.arenaItem1.Name = "arenaItem1";
            this.arenaItem1.Size = new System.Drawing.Size(269, 179);
            this.arenaItem1.TabIndex = 2;
            // 
            // ArenaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 273);
            this.Controls.Add(this.arenaItem1);
            this.Name = "ArenaForm";
            this.Controls.SetChildIndex(this.arenaItem1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ArenaItem arenaItem1;
    }
}