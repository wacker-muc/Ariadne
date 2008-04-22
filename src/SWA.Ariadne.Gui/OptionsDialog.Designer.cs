namespace SWA.Ariadne.Gui
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxEfficientSolvers = new System.Windows.Forms.CheckBox();
            this.textBoxStepsPerSecond = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxDetailsBox = new System.Windows.Forms.CheckBox();
            this.checkBoxBlinking = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.imageMinSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.imageMaxSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.imageNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.selectImageFolderButton = new System.Windows.Forms.Button();
            this.imageFolderTextBox = new System.Windows.Forms.TextBox();
            this.imageFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageMinSizeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageMaxSizeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(23, 139);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(114, 139);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelCopyright
            // 
            this.labelCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCopyright.AutoSize = true;
            this.labelCopyright.Location = new System.Drawing.Point(71, 166);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(66, 13);
            this.labelCopyright.TabIndex = 7;
            this.labelCopyright.Text = "(c) Copyright";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(212, 132);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxEfficientSolvers);
            this.tabPage1.Controls.Add(this.textBoxStepsPerSecond);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.checkBoxDetailsBox);
            this.tabPage1.Controls.Add(this.checkBoxBlinking);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(204, 106);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBoxEfficientSolvers
            // 
            this.checkBoxEfficientSolvers.AutoSize = true;
            this.checkBoxEfficientSolvers.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxEfficientSolvers.Location = new System.Drawing.Point(43, 54);
            this.checkBoxEfficientSolvers.Name = "checkBoxEfficientSolvers";
            this.checkBoxEfficientSolvers.Size = new System.Drawing.Size(124, 17);
            this.checkBoxEfficientSolvers.TabIndex = 13;
            this.checkBoxEfficientSolvers.Text = "Use Efficient Solvers";
            this.checkBoxEfficientSolvers.UseVisualStyleBackColor = true;
            // 
            // textBoxStepsPerSecond
            // 
            this.textBoxStepsPerSecond.Location = new System.Drawing.Point(153, 77);
            this.textBoxStepsPerSecond.Name = "textBoxStepsPerSecond";
            this.textBoxStepsPerSecond.Size = new System.Drawing.Size(46, 20);
            this.textBoxStepsPerSecond.TabIndex = 12;
            this.textBoxStepsPerSecond.Text = "200";
            this.textBoxStepsPerSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Steps per Second";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxDetailsBox
            // 
            this.checkBoxDetailsBox.AutoSize = true;
            this.checkBoxDetailsBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDetailsBox.Location = new System.Drawing.Point(58, 8);
            this.checkBoxDetailsBox.Name = "checkBoxDetailsBox";
            this.checkBoxDetailsBox.Size = new System.Drawing.Size(109, 17);
            this.checkBoxDetailsBox.TabIndex = 10;
            this.checkBoxDetailsBox.Text = "Show Details Box";
            this.checkBoxDetailsBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDetailsBox.UseVisualStyleBackColor = true;
            // 
            // checkBoxBlinking
            // 
            this.checkBoxBlinking.AutoSize = true;
            this.checkBoxBlinking.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxBlinking.Location = new System.Drawing.Point(6, 31);
            this.checkBoxBlinking.Name = "checkBoxBlinking";
            this.checkBoxBlinking.Size = new System.Drawing.Size(161, 17);
            this.checkBoxBlinking.TabIndex = 9;
            this.checkBoxBlinking.Text = "Paint Blinking Target Square";
            this.checkBoxBlinking.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.imageMinSizeNumericUpDown);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.imageMaxSizeNumericUpDown);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.imageNumberNumericUpDown);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.selectImageFolderButton);
            this.tabPage2.Controls.Add(this.imageFolderTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(204, 106);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Images";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // imageMinSizeNumericUpDown
            // 
            this.imageMinSizeNumericUpDown.Location = new System.Drawing.Point(87, 31);
            this.imageMinSizeNumericUpDown.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.imageMinSizeNumericUpDown.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.imageMinSizeNumericUpDown.Name = "imageMinSizeNumericUpDown";
            this.imageMinSizeNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.imageMinSizeNumericUpDown.TabIndex = 119;
            this.imageMinSizeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.imageMinSizeNumericUpDown.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(12, 34);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(48, 13);
            this.label18.TabIndex = 118;
            this.label18.Text = "Min. size";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // imageMaxSizeNumericUpDown
            // 
            this.imageMaxSizeNumericUpDown.Location = new System.Drawing.Point(87, 54);
            this.imageMaxSizeNumericUpDown.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.imageMaxSizeNumericUpDown.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.imageMaxSizeNumericUpDown.Name = "imageMaxSizeNumericUpDown";
            this.imageMaxSizeNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.imageMaxSizeNumericUpDown.TabIndex = 117;
            this.imageMaxSizeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.imageMaxSizeNumericUpDown.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 57);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(51, 13);
            this.label16.TabIndex = 116;
            this.label16.Text = "Max. size";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // imageNumberNumericUpDown
            // 
            this.imageNumberNumericUpDown.Location = new System.Drawing.Point(87, 8);
            this.imageNumberNumericUpDown.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.imageNumberNumericUpDown.Name = "imageNumberNumericUpDown";
            this.imageNumberNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.imageNumberNumericUpDown.TabIndex = 114;
            this.imageNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 11);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(75, 13);
            this.label17.TabIndex = 115;
            this.label17.Text = "Number (max.)";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // selectImageFolderButton
            // 
            this.selectImageFolderButton.Location = new System.Drawing.Point(6, 76);
            this.selectImageFolderButton.Name = "selectImageFolderButton";
            this.selectImageFolderButton.Size = new System.Drawing.Size(49, 23);
            this.selectImageFolderButton.TabIndex = 113;
            this.selectImageFolderButton.Text = "Folder";
            this.selectImageFolderButton.UseVisualStyleBackColor = true;
            this.selectImageFolderButton.Click += new System.EventHandler(this.selectImageFolderButton_Click);
            // 
            // imageFolderTextBox
            // 
            this.imageFolderTextBox.Location = new System.Drawing.Point(87, 77);
            this.imageFolderTextBox.MaxLength = 14;
            this.imageFolderTextBox.Name = "imageFolderTextBox";
            this.imageFolderTextBox.Size = new System.Drawing.Size(114, 20);
            this.imageFolderTextBox.TabIndex = 112;
            this.imageFolderTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.imageFolderTextBox.WordWrap = false;
            // 
            // OptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(212, 181);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "OptionsDialog";
            this.ShowInTaskbar = false;
            this.Text = "Ariadne Settings";
            this.Load += new System.EventHandler(this.OptionsDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageMinSizeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageMaxSizeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox checkBoxEfficientSolvers;
        private System.Windows.Forms.TextBox textBoxStepsPerSecond;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxDetailsBox;
        private System.Windows.Forms.CheckBox checkBoxBlinking;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown imageMinSizeNumericUpDown;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.NumericUpDown imageMaxSizeNumericUpDown;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown imageNumberNumericUpDown;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button selectImageFolderButton;
        private System.Windows.Forms.TextBox imageFolderTextBox;
        private System.Windows.Forms.FolderBrowserDialog imageFolderBrowserDialog;

    }
}