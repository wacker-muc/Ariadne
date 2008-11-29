namespace SWA.Ariadne.Gui.Dialogs
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.checkBoxLogSolverStatistics = new System.Windows.Forms.CheckBox();
            this.checkBoxEfficientSolvers = new System.Windows.Forms.CheckBox();
            this.textBoxStepsPerSecond = new System.Windows.Forms.TextBox();
            this.labelStepsPerSecond = new System.Windows.Forms.Label();
            this.checkBoxDetailsBox = new System.Windows.Forms.CheckBox();
            this.checkBoxBlinking = new System.Windows.Forms.CheckBox();
            this.tabPageImages = new System.Windows.Forms.TabPage();
            this.subtractImagesBackgroundCheckBox = new System.Windows.Forms.CheckBox();
            this.imageMinSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelImagesMinSize = new System.Windows.Forms.Label();
            this.imageMaxSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelImagesMaxSize = new System.Windows.Forms.Label();
            this.imageNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelImagesNumber = new System.Windows.Forms.Label();
            this.selectImageFolderButton = new System.Windows.Forms.Button();
            this.imageFolderTextBox = new System.Windows.Forms.TextBox();
            this.tabPageBackground = new System.Windows.Forms.TabPage();
            this.checkBoxDifferentBackgroundImageFolder = new System.Windows.Forms.CheckBox();
            this.selectBackgroundImageFolderButton = new System.Windows.Forms.Button();
            this.backgroundImageFolderTextBox = new System.Windows.Forms.TextBox();
            this.checkBoxBackgroundImage = new System.Windows.Forms.CheckBox();
            this.tabPageExtras = new System.Windows.Forms.TabPage();
            this.checkBoxMultipleMazes = new System.Windows.Forms.CheckBox();
            this.checkBoxPaintAllWalls = new System.Windows.Forms.CheckBox();
            this.checkBoxIrregularMazes = new System.Windows.Forms.CheckBox();
            this.checkBoxOutlineShapes = new System.Windows.Forms.CheckBox();
            this.imageFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageMinSizeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageMaxSizeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageNumberNumericUpDown)).BeginInit();
            this.tabPageBackground.SuspendLayout();
            this.tabPageExtras.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(23, 162);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(114, 162);
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
            this.labelCopyright.Location = new System.Drawing.Point(71, 189);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(66, 13);
            this.labelCopyright.TabIndex = 7;
            this.labelCopyright.Text = "(c) Copyright";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageGeneral);
            this.tabControl1.Controls.Add(this.tabPageImages);
            this.tabControl1.Controls.Add(this.tabPageBackground);
            this.tabControl1.Controls.Add(this.tabPageExtras);
            this.tabControl1.Location = new System.Drawing.Point(0, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(212, 155);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.checkBoxLogSolverStatistics);
            this.tabPageGeneral.Controls.Add(this.checkBoxEfficientSolvers);
            this.tabPageGeneral.Controls.Add(this.textBoxStepsPerSecond);
            this.tabPageGeneral.Controls.Add(this.labelStepsPerSecond);
            this.tabPageGeneral.Controls.Add(this.checkBoxDetailsBox);
            this.tabPageGeneral.Controls.Add(this.checkBoxBlinking);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(204, 129);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // checkBoxLogSolverStatistics
            // 
            this.checkBoxLogSolverStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxLogSolverStatistics.AutoSize = true;
            this.checkBoxLogSolverStatistics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxLogSolverStatistics.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxLogSolverStatistics.Location = new System.Drawing.Point(45, 103);
            this.checkBoxLogSolverStatistics.Name = "checkBoxLogSolverStatistics";
            this.checkBoxLogSolverStatistics.Size = new System.Drawing.Size(122, 17);
            this.checkBoxLogSolverStatistics.TabIndex = 5;
            this.checkBoxLogSolverStatistics.Text = "Log Solver Statistics";
            this.checkBoxLogSolverStatistics.UseVisualStyleBackColor = true;
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
            this.tabPageImages.Controls.Add(this.subtractImagesBackgroundCheckBox);
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
            this.tabPageImages.Size = new System.Drawing.Size(204, 129);
            this.tabPageImages.TabIndex = 1;
            this.tabPageImages.Text = "Images";
            this.tabPageImages.UseVisualStyleBackColor = true;
            // 
            // subtractImagesBackgroundCheckBox
            // 
            this.subtractImagesBackgroundCheckBox.AutoSize = true;
            this.subtractImagesBackgroundCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.subtractImagesBackgroundCheckBox.Location = new System.Drawing.Point(8, 105);
            this.subtractImagesBackgroundCheckBox.Name = "subtractImagesBackgroundCheckBox";
            this.subtractImagesBackgroundCheckBox.Size = new System.Drawing.Size(163, 17);
            this.subtractImagesBackgroundCheckBox.TabIndex = 119;
            this.subtractImagesBackgroundCheckBox.Text = "Subtract uniform background";
            this.subtractImagesBackgroundCheckBox.UseVisualStyleBackColor = true;
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
            // tabPageBackground
            // 
            this.tabPageBackground.Controls.Add(this.checkBoxDifferentBackgroundImageFolder);
            this.tabPageBackground.Controls.Add(this.selectBackgroundImageFolderButton);
            this.tabPageBackground.Controls.Add(this.backgroundImageFolderTextBox);
            this.tabPageBackground.Controls.Add(this.checkBoxBackgroundImage);
            this.tabPageBackground.Location = new System.Drawing.Point(4, 22);
            this.tabPageBackground.Name = "tabPageBackground";
            this.tabPageBackground.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBackground.Size = new System.Drawing.Size(204, 129);
            this.tabPageBackground.TabIndex = 2;
            this.tabPageBackground.Text = "Background";
            this.tabPageBackground.UseVisualStyleBackColor = true;
            // 
            // checkBoxDifferentBackgroundImageFolder
            // 
            this.checkBoxDifferentBackgroundImageFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxDifferentBackgroundImageFolder.AutoSize = true;
            this.checkBoxDifferentBackgroundImageFolder.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDifferentBackgroundImageFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxDifferentBackgroundImageFolder.Location = new System.Drawing.Point(61, 80);
            this.checkBoxDifferentBackgroundImageFolder.Name = "checkBoxDifferentBackgroundImageFolder";
            this.checkBoxDifferentBackgroundImageFolder.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDifferentBackgroundImageFolder.TabIndex = 8;
            this.checkBoxDifferentBackgroundImageFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDifferentBackgroundImageFolder.UseVisualStyleBackColor = true;
            this.checkBoxDifferentBackgroundImageFolder.CheckedChanged += new System.EventHandler(this.checkBoxDifferentBackgroundImageFolder_CheckedChanged);
            // 
            // selectBackgroundImageFolderButton
            // 
            this.selectBackgroundImageFolderButton.Location = new System.Drawing.Point(6, 76);
            this.selectBackgroundImageFolderButton.Name = "selectBackgroundImageFolderButton";
            this.selectBackgroundImageFolderButton.Size = new System.Drawing.Size(49, 23);
            this.selectBackgroundImageFolderButton.TabIndex = 6;
            this.selectBackgroundImageFolderButton.Text = "Folder";
            this.selectBackgroundImageFolderButton.UseVisualStyleBackColor = true;
            this.selectBackgroundImageFolderButton.Click += new System.EventHandler(this.selectBackgroundImageFolderButton_Click);
            // 
            // backgroundImageFolderTextBox
            // 
            this.backgroundImageFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.backgroundImageFolderTextBox.Location = new System.Drawing.Point(87, 77);
            this.backgroundImageFolderTextBox.MaxLength = 14;
            this.backgroundImageFolderTextBox.Name = "backgroundImageFolderTextBox";
            this.backgroundImageFolderTextBox.Size = new System.Drawing.Size(114, 20);
            this.backgroundImageFolderTextBox.TabIndex = 7;
            this.backgroundImageFolderTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.backgroundImageFolderTextBox.WordWrap = false;
            // 
            // checkBoxBackgroundImage
            // 
            this.checkBoxBackgroundImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxBackgroundImage.AutoSize = true;
            this.checkBoxBackgroundImage.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxBackgroundImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxBackgroundImage.Location = new System.Drawing.Point(14, 31);
            this.checkBoxBackgroundImage.Name = "checkBoxBackgroundImage";
            this.checkBoxBackgroundImage.Size = new System.Drawing.Size(153, 17);
            this.checkBoxBackgroundImage.TabIndex = 2;
            this.checkBoxBackgroundImage.Text = "Display Background Image";
            this.checkBoxBackgroundImage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxBackgroundImage.UseVisualStyleBackColor = true;
            // 
            // tabPageExtras
            // 
            this.tabPageExtras.Controls.Add(this.checkBoxMultipleMazes);
            this.tabPageExtras.Controls.Add(this.checkBoxPaintAllWalls);
            this.tabPageExtras.Controls.Add(this.checkBoxIrregularMazes);
            this.tabPageExtras.Controls.Add(this.checkBoxOutlineShapes);
            this.tabPageExtras.Location = new System.Drawing.Point(4, 22);
            this.tabPageExtras.Name = "tabPageExtras";
            this.tabPageExtras.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExtras.Size = new System.Drawing.Size(204, 129);
            this.tabPageExtras.TabIndex = 3;
            this.tabPageExtras.Text = "Extras";
            this.tabPageExtras.UseVisualStyleBackColor = true;
            // 
            // checkBoxMultipleMazes
            // 
            this.checkBoxMultipleMazes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxMultipleMazes.AutoSize = true;
            this.checkBoxMultipleMazes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxMultipleMazes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxMultipleMazes.Location = new System.Drawing.Point(37, 77);
            this.checkBoxMultipleMazes.Name = "checkBoxMultipleMazes";
            this.checkBoxMultipleMazes.Size = new System.Drawing.Size(130, 17);
            this.checkBoxMultipleMazes.TabIndex = 4;
            this.checkBoxMultipleMazes.Text = "Create Multiple Mazes";
            this.checkBoxMultipleMazes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxMultipleMazes.UseVisualStyleBackColor = true;
            // 
            // checkBoxPaintAllWalls
            // 
            this.checkBoxPaintAllWalls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxPaintAllWalls.AutoSize = true;
            this.checkBoxPaintAllWalls.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxPaintAllWalls.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxPaintAllWalls.Location = new System.Drawing.Point(75, 8);
            this.checkBoxPaintAllWalls.Name = "checkBoxPaintAllWalls";
            this.checkBoxPaintAllWalls.Size = new System.Drawing.Size(92, 17);
            this.checkBoxPaintAllWalls.TabIndex = 1;
            this.checkBoxPaintAllWalls.Text = "Paint all Walls";
            this.checkBoxPaintAllWalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxPaintAllWalls.UseVisualStyleBackColor = true;
            // 
            // checkBoxIrregularMazes
            // 
            this.checkBoxIrregularMazes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxIrregularMazes.AutoSize = true;
            this.checkBoxIrregularMazes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxIrregularMazes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkBoxIrregularMazes.Location = new System.Drawing.Point(43, 54);
            this.checkBoxIrregularMazes.Name = "checkBoxIrregularMazes";
            this.checkBoxIrregularMazes.Size = new System.Drawing.Size(124, 17);
            this.checkBoxIrregularMazes.TabIndex = 3;
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
            this.checkBoxOutlineShapes.Location = new System.Drawing.Point(47, 31);
            this.checkBoxOutlineShapes.Name = "checkBoxOutlineShapes";
            this.checkBoxOutlineShapes.Size = new System.Drawing.Size(120, 17);
            this.checkBoxOutlineShapes.TabIndex = 2;
            this.checkBoxOutlineShapes.Text = "Add Outline Shapes";
            this.checkBoxOutlineShapes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxOutlineShapes.UseVisualStyleBackColor = true;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 20000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip.ToolTipTitle = "Ariadne Screen Saver Settings";
            // 
            // OptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(212, 204);
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
            this.tabPageBackground.ResumeLayout(false);
            this.tabPageBackground.PerformLayout();
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
        private System.Windows.Forms.CheckBox checkBoxPaintAllWalls;
        private System.Windows.Forms.CheckBox checkBoxMultipleMazes;
        private System.Windows.Forms.CheckBox subtractImagesBackgroundCheckBox;
        private System.Windows.Forms.TabPage tabPageBackground;
        private System.Windows.Forms.Button selectBackgroundImageFolderButton;
        private System.Windows.Forms.TextBox backgroundImageFolderTextBox;
        private System.Windows.Forms.CheckBox checkBoxBackgroundImage;
        private System.Windows.Forms.CheckBox checkBoxDifferentBackgroundImageFolder;
        private System.Windows.Forms.CheckBox checkBoxLogSolverStatistics;
        private System.Windows.Forms.ToolTip toolTip;

    }
}