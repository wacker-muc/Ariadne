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
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.checkBoxEfficientSolvers = new System.Windows.Forms.CheckBox();
            this.textBoxStepsPerSecond = new System.Windows.Forms.TextBox();
            this.labelStepsPerSecond = new System.Windows.Forms.Label();
            this.checkBoxDetailsBox = new System.Windows.Forms.CheckBox();
            this.checkBoxBlinking = new System.Windows.Forms.CheckBox();
            this.tabPageImages = new System.Windows.Forms.TabPage();
            this.imageMinSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelImagesMinSize = new System.Windows.Forms.Label();
            this.imageMaxSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelImagesMaxSize = new System.Windows.Forms.Label();
            this.imageNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelImagesNumber = new System.Windows.Forms.Label();
            this.selectImageFolderButton = new System.Windows.Forms.Button();
            this.imageFolderTextBox = new System.Windows.Forms.TextBox();
            this.tabPageExtras = new System.Windows.Forms.TabPage();
            this.checkBoxIrregularMazes = new System.Windows.Forms.CheckBox();
            this.checkBoxOutlineShapes = new System.Windows.Forms.CheckBox();
            this.imageFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageMinSizeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageMaxSizeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageNumberNumericUpDown)).BeginInit();
            this.tabPageExtras.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tabPageGeneral);
            this.tabControl1.Controls.Add(this.tabPageImages);
            this.tabControl1.Controls.Add(this.tabPageExtras);
            this.tabControl1.Location = new System.Drawing.Point(0, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(212, 132);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.checkBoxEfficientSolvers);
            this.tabPageGeneral.Controls.Add(this.textBoxStepsPerSecond);
            this.tabPageGeneral.Controls.Add(this.labelStepsPerSecond);
            this.tabPageGeneral.Controls.Add(this.checkBoxDetailsBox);
            this.tabPageGeneral.Controls.Add(this.checkBoxBlinking);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(204, 106);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // checkBoxEfficientSolvers
            // 
            this.checkBoxEfficientSolvers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxEfficientSolvers.AutoSize = true;
            this.checkBoxEfficientSolvers.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxEfficientSolvers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxEfficientSolvers.Location = new System.Drawing.Point(43, 54);
            this.checkBoxEfficientSolvers.Name = "checkBoxEfficientSolvers";
            this.checkBoxEfficientSolvers.Size = new System.Drawing.Size(124, 17);
            this.checkBoxEfficientSolvers.TabIndex = 3;
            this.checkBoxEfficientSolvers.Text = "Use Efficient Solvers";
            this.checkBoxEfficientSolvers.UseVisualStyleBackColor = true;
            // 
            // textBoxStepsPerSecond
            // 
            this.textBoxStepsPerSecond.Location = new System.Drawing.Point(153, 77);
            this.textBoxStepsPerSecond.Name = "textBoxStepsPerSecond";
            this.textBoxStepsPerSecond.Size = new System.Drawing.Size(46, 20);
            this.textBoxStepsPerSecond.TabIndex = 4;
            this.textBoxStepsPerSecond.Text = "200";
            this.textBoxStepsPerSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelStepsPerSecond
            // 
            this.labelStepsPerSecond.AutoSize = true;
            this.labelStepsPerSecond.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.labelStepsPerSecond.Location = new System.Drawing.Point(58, 80);
            this.labelStepsPerSecond.Name = "labelStepsPerSecond";
            this.labelStepsPerSecond.Size = new System.Drawing.Size(92, 13);
            this.labelStepsPerSecond.TabIndex = 11;
            this.labelStepsPerSecond.Text = "Steps per Second";
            this.labelStepsPerSecond.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxDetailsBox
            // 
            this.checkBoxDetailsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxDetailsBox.AutoSize = true;
            this.checkBoxDetailsBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDetailsBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxDetailsBox.Location = new System.Drawing.Point(58, 8);
            this.checkBoxDetailsBox.Name = "checkBoxDetailsBox";
            this.checkBoxDetailsBox.Size = new System.Drawing.Size(109, 17);
            this.checkBoxDetailsBox.TabIndex = 1;
            this.checkBoxDetailsBox.Text = "Show Details Box";
            this.checkBoxDetailsBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDetailsBox.UseVisualStyleBackColor = true;
            // 
            // checkBoxBlinking
            // 
            this.checkBoxBlinking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxBlinking.AutoSize = true;
            this.checkBoxBlinking.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxBlinking.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxBlinking.Location = new System.Drawing.Point(6, 31);
            this.checkBoxBlinking.Name = "checkBoxBlinking";
            this.checkBoxBlinking.Size = new System.Drawing.Size(161, 17);
            this.checkBoxBlinking.TabIndex = 2;
            this.checkBoxBlinking.Text = "Paint Blinking Target Square";
            this.checkBoxBlinking.UseVisualStyleBackColor = true;
            // 
            // tabPageImages
            // 
            this.tabPageImages.Controls.Add(this.imageMinSizeNumericUpDown);
            this.tabPageImages.Controls.Add(this.labelImagesMinSize);
            this.tabPageImages.Controls.Add(this.imageMaxSizeNumericUpDown);
            this.tabPageImages.Controls.Add(this.labelImagesMaxSize);
            this.tabPageImages.Controls.Add(this.imageNumberNumericUpDown);
            this.tabPageImages.Controls.Add(this.labelImagesNumber);
            this.tabPageImages.Controls.Add(this.selectImageFolderButton);
            this.tabPageImages.Controls.Add(this.imageFolderTextBox);
            this.tabPageImages.Location = new System.Drawing.Point(4, 22);
            this.tabPageImages.Name = "tabPageImages";
            this.tabPageImages.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageImages.Size = new System.Drawing.Size(204, 106);
            this.tabPageImages.TabIndex = 1;
            this.tabPageImages.Text = "Images";
            this.tabPageImages.UseVisualStyleBackColor = true;
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
            this.imageMinSizeNumericUpDown.TabIndex = 2;
            this.imageMinSizeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.imageMinSizeNumericUpDown.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // labelImagesMinSize
            // 
            this.labelImagesMinSize.AutoSize = true;
            this.labelImagesMinSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.labelImagesMinSize.Location = new System.Drawing.Point(10, 34);
            this.labelImagesMinSize.Name = "labelImagesMinSize";
            this.labelImagesMinSize.Size = new System.Drawing.Size(48, 13);
            this.labelImagesMinSize.TabIndex = 118;
            this.labelImagesMinSize.Text = "Min. size";
            this.labelImagesMinSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.imageMaxSizeNumericUpDown.TabIndex = 3;
            this.imageMaxSizeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.imageMaxSizeNumericUpDown.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            // 
            // labelImagesMaxSize
            // 
            this.labelImagesMaxSize.AutoSize = true;
            this.labelImagesMaxSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.labelImagesMaxSize.Location = new System.Drawing.Point(10, 57);
            this.labelImagesMaxSize.Name = "labelImagesMaxSize";
            this.labelImagesMaxSize.Size = new System.Drawing.Size(51, 13);
            this.labelImagesMaxSize.TabIndex = 116;
            this.labelImagesMaxSize.Text = "Max. size";
            this.labelImagesMaxSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.imageNumberNumericUpDown.TabIndex = 1;
            this.imageNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelImagesNumber
            // 
            this.labelImagesNumber.AutoSize = true;
            this.labelImagesNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.labelImagesNumber.Location = new System.Drawing.Point(10, 11);
            this.labelImagesNumber.Name = "labelImagesNumber";
            this.labelImagesNumber.Size = new System.Drawing.Size(75, 13);
            this.labelImagesNumber.TabIndex = 115;
            this.labelImagesNumber.Text = "Number (max.)";
            this.labelImagesNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // selectImageFolderButton
            // 
            this.selectImageFolderButton.Location = new System.Drawing.Point(6, 76);
            this.selectImageFolderButton.Name = "selectImageFolderButton";
            this.selectImageFolderButton.Size = new System.Drawing.Size(49, 23);
            this.selectImageFolderButton.TabIndex = 4;
            this.selectImageFolderButton.Text = "Folder";
            this.selectImageFolderButton.UseVisualStyleBackColor = true;
            this.selectImageFolderButton.Click += new System.EventHandler(this.selectImageFolderButton_Click);
            // 
            // imageFolderTextBox
            // 
            this.imageFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.imageFolderTextBox.Location = new System.Drawing.Point(87, 77);
            this.imageFolderTextBox.MaxLength = 14;
            this.imageFolderTextBox.Name = "imageFolderTextBox";
            this.imageFolderTextBox.Size = new System.Drawing.Size(114, 20);
            this.imageFolderTextBox.TabIndex = 5;
            this.imageFolderTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.imageFolderTextBox.WordWrap = false;
            // 
            // tabPageExtras
            // 
            this.tabPageExtras.Controls.Add(this.checkBoxIrregularMazes);
            this.tabPageExtras.Controls.Add(this.checkBoxOutlineShapes);
            this.tabPageExtras.Location = new System.Drawing.Point(4, 22);
            this.tabPageExtras.Name = "tabPageExtras";
            this.tabPageExtras.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExtras.Size = new System.Drawing.Size(204, 106);
            this.tabPageExtras.TabIndex = 2;
            this.tabPageExtras.Text = "Extras";
            this.tabPageExtras.UseVisualStyleBackColor = true;
            // 
            // checkBoxIrregularMazes
            // 
            this.checkBoxIrregularMazes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxIrregularMazes.AutoSize = true;
            this.checkBoxIrregularMazes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxIrregularMazes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxIrregularMazes.Location = new System.Drawing.Point(43, 31);
            this.checkBoxIrregularMazes.Name = "checkBoxIrregularMazes";
            this.checkBoxIrregularMazes.Size = new System.Drawing.Size(124, 17);
            this.checkBoxIrregularMazes.TabIndex = 2;
            this.checkBoxIrregularMazes.Text = "Build Irregular Mazes";
            this.checkBoxIrregularMazes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxIrregularMazes.UseVisualStyleBackColor = true;
            // 
            // checkBoxOutlineShapes
            // 
            this.checkBoxOutlineShapes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxOutlineShapes.AutoSize = true;
            this.checkBoxOutlineShapes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxOutlineShapes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxOutlineShapes.Location = new System.Drawing.Point(47, 8);
            this.checkBoxOutlineShapes.Name = "checkBoxOutlineShapes";
            this.checkBoxOutlineShapes.Size = new System.Drawing.Size(120, 17);
            this.checkBoxOutlineShapes.TabIndex = 1;
            this.checkBoxOutlineShapes.Text = "Add Outline Shapes";
            this.checkBoxOutlineShapes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxOutlineShapes.UseVisualStyleBackColor = true;
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
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.tabPageImages.ResumeLayout(false);
            this.tabPageImages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageMinSizeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageMaxSizeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageNumberNumericUpDown)).EndInit();
            this.tabPageExtras.ResumeLayout(false);
            this.tabPageExtras.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.CheckBox checkBoxEfficientSolvers;
        private System.Windows.Forms.TextBox textBoxStepsPerSecond;
        private System.Windows.Forms.Label labelStepsPerSecond;
        private System.Windows.Forms.CheckBox checkBoxDetailsBox;
        private System.Windows.Forms.CheckBox checkBoxBlinking;
        private System.Windows.Forms.TabPage tabPageImages;
        private System.Windows.Forms.NumericUpDown imageMinSizeNumericUpDown;
        private System.Windows.Forms.Label labelImagesMinSize;
        private System.Windows.Forms.NumericUpDown imageMaxSizeNumericUpDown;
        private System.Windows.Forms.Label labelImagesMaxSize;
        private System.Windows.Forms.NumericUpDown imageNumberNumericUpDown;
        private System.Windows.Forms.Label labelImagesNumber;
        private System.Windows.Forms.Button selectImageFolderButton;
        private System.Windows.Forms.TextBox imageFolderTextBox;
        private System.Windows.Forms.FolderBrowserDialog imageFolderBrowserDialog;
        private System.Windows.Forms.TabPage tabPageExtras;
        private System.Windows.Forms.CheckBox checkBoxOutlineShapes;
        private System.Windows.Forms.CheckBox checkBoxIrregularMazes;

    }
}