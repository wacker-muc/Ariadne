namespace SWA.Ariadne.App
{
    partial class DetailsDialog
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.layoutPage = new System.Windows.Forms.TabPage();
            this.squareWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.dataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pathWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.wallWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.gridWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.autoGridWidthCheckBox = new System.Windows.Forms.CheckBox();
            this.autoWallWidthCheckBox = new System.Windows.Forms.CheckBox();
            this.autoPathWidthCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.autoSquareWidthCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.resultingAreaTextBox2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.squareWidthLabel = new System.Windows.Forms.Label();
            this.capStyleComboBox = new System.Windows.Forms.ComboBox();
            this.setLayoutButton = new System.Windows.Forms.Button();
            this.colorsPage = new System.Windows.Forms.TabPage();
            this.shapePage = new System.Windows.Forms.TabPage();
            this.resultingAreaTextBox1 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.setShapeButton = new System.Windows.Forms.Button();
            this.CodeLabel = new System.Windows.Forms.Label();
            this.codeTextBox = new System.Windows.Forms.TextBox();
            this.autoSeedCheckBox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.seedTextBox = new System.Windows.Forms.TextBox();
            this.autoMazeHeightCheckBox = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.mazeHeightTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.autoMazeWidthCheckBox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.mazeWidthBox = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.layoutPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.squareWidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pathWidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wallWidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridWidthNumericUpDown)).BeginInit();
            this.shapePage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.layoutPage);
            this.tabControl1.Controls.Add(this.colorsPage);
            this.tabControl1.Controls.Add(this.shapePage);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(268, 249);
            this.tabControl1.TabIndex = 0;
            // 
            // layoutPage
            // 
            this.layoutPage.Controls.Add(this.squareWidthNumericUpDown);
            this.layoutPage.Controls.Add(this.pathWidthNumericUpDown);
            this.layoutPage.Controls.Add(this.wallWidthNumericUpDown);
            this.layoutPage.Controls.Add(this.gridWidthNumericUpDown);
            this.layoutPage.Controls.Add(this.autoGridWidthCheckBox);
            this.layoutPage.Controls.Add(this.autoWallWidthCheckBox);
            this.layoutPage.Controls.Add(this.autoPathWidthCheckBox);
            this.layoutPage.Controls.Add(this.label6);
            this.layoutPage.Controls.Add(this.autoSquareWidthCheckbox);
            this.layoutPage.Controls.Add(this.label5);
            this.layoutPage.Controls.Add(this.resultingAreaTextBox2);
            this.layoutPage.Controls.Add(this.label4);
            this.layoutPage.Controls.Add(this.label3);
            this.layoutPage.Controls.Add(this.label2);
            this.layoutPage.Controls.Add(this.label1);
            this.layoutPage.Controls.Add(this.squareWidthLabel);
            this.layoutPage.Controls.Add(this.capStyleComboBox);
            this.layoutPage.Controls.Add(this.setLayoutButton);
            this.layoutPage.Location = new System.Drawing.Point(4, 22);
            this.layoutPage.Name = "layoutPage";
            this.layoutPage.Padding = new System.Windows.Forms.Padding(3);
            this.layoutPage.Size = new System.Drawing.Size(260, 223);
            this.layoutPage.TabIndex = 1;
            this.layoutPage.Text = "Layout";
            this.layoutPage.UseVisualStyleBackColor = true;
            // 
            // squareWidthNumericUpDown
            // 
            this.squareWidthNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "SquareWidth", true));
            this.squareWidthNumericUpDown.Location = new System.Drawing.Point(91, 29);
            this.squareWidthNumericUpDown.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.squareWidthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.squareWidthNumericUpDown.Name = "squareWidthNumericUpDown";
            this.squareWidthNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.squareWidthNumericUpDown.TabIndex = 1;
            this.squareWidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.squareWidthNumericUpDown.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.squareWidthNumericUpDown.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // dataBindingSource
            // 
            this.dataBindingSource.DataSource = typeof(SWA.Ariadne.Settings.AriadneSettingsData);
            this.dataBindingSource.CurrentItemChanged += new System.EventHandler(this.OnDataChanged);
            // 
            // pathWidthNumericUpDown
            // 
            this.pathWidthNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "PathWidth", true));
            this.pathWidthNumericUpDown.Location = new System.Drawing.Point(91, 55);
            this.pathWidthNumericUpDown.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.pathWidthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.pathWidthNumericUpDown.Name = "pathWidthNumericUpDown";
            this.pathWidthNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.pathWidthNumericUpDown.TabIndex = 2;
            this.pathWidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pathWidthNumericUpDown.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.pathWidthNumericUpDown.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // wallWidthNumericUpDown
            // 
            this.wallWidthNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "WallWidth", true));
            this.wallWidthNumericUpDown.Location = new System.Drawing.Point(91, 81);
            this.wallWidthNumericUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.wallWidthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.wallWidthNumericUpDown.Name = "wallWidthNumericUpDown";
            this.wallWidthNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.wallWidthNumericUpDown.TabIndex = 3;
            this.wallWidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.wallWidthNumericUpDown.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.wallWidthNumericUpDown.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // gridWidthNumericUpDown
            // 
            this.gridWidthNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "GridWidth", true));
            this.gridWidthNumericUpDown.Location = new System.Drawing.Point(91, 113);
            this.gridWidthNumericUpDown.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.gridWidthNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.gridWidthNumericUpDown.Name = "gridWidthNumericUpDown";
            this.gridWidthNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.gridWidthNumericUpDown.TabIndex = 4;
            this.gridWidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.gridWidthNumericUpDown.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.gridWidthNumericUpDown.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // autoGridWidthCheckBox
            // 
            this.autoGridWidthCheckBox.AutoSize = true;
            this.autoGridWidthCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoGridWidth", true));
            this.autoGridWidthCheckBox.Location = new System.Drawing.Point(144, 116);
            this.autoGridWidthCheckBox.Name = "autoGridWidthCheckBox";
            this.autoGridWidthCheckBox.Size = new System.Drawing.Size(15, 14);
            this.autoGridWidthCheckBox.TabIndex = 14;
            this.autoGridWidthCheckBox.UseVisualStyleBackColor = true;
            this.autoGridWidthCheckBox.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // autoWallWidthCheckBox
            // 
            this.autoWallWidthCheckBox.AutoSize = true;
            this.autoWallWidthCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoWallWidth", true));
            this.autoWallWidthCheckBox.Location = new System.Drawing.Point(144, 84);
            this.autoWallWidthCheckBox.Name = "autoWallWidthCheckBox";
            this.autoWallWidthCheckBox.Size = new System.Drawing.Size(15, 14);
            this.autoWallWidthCheckBox.TabIndex = 13;
            this.autoWallWidthCheckBox.UseVisualStyleBackColor = true;
            this.autoWallWidthCheckBox.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // autoPathWidthCheckBox
            // 
            this.autoPathWidthCheckBox.AutoSize = true;
            this.autoPathWidthCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoPathWidth", true));
            this.autoPathWidthCheckBox.Location = new System.Drawing.Point(144, 58);
            this.autoPathWidthCheckBox.Name = "autoPathWidthCheckBox";
            this.autoPathWidthCheckBox.Size = new System.Drawing.Size(15, 14);
            this.autoPathWidthCheckBox.TabIndex = 12;
            this.autoPathWidthCheckBox.UseVisualStyleBackColor = true;
            this.autoPathWidthCheckBox.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(141, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Automatic";
            // 
            // autoSquareWidthCheckbox
            // 
            this.autoSquareWidthCheckbox.AutoSize = true;
            this.autoSquareWidthCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoSquareWidth", true));
            this.autoSquareWidthCheckbox.Location = new System.Drawing.Point(144, 32);
            this.autoSquareWidthCheckbox.Name = "autoSquareWidthCheckbox";
            this.autoSquareWidthCheckbox.Size = new System.Drawing.Size(15, 14);
            this.autoSquareWidthCheckbox.TabIndex = 11;
            this.autoSquareWidthCheckbox.UseVisualStyleBackColor = true;
            this.autoSquareWidthCheckbox.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Path cap style";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Visible = false;
            // 
            // resultingAreaTextBox2
            // 
            this.resultingAreaTextBox2.BackColor = System.Drawing.SystemColors.Control;
            this.resultingAreaTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultingAreaTextBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "ResultingArea", true));
            this.resultingAreaTextBox2.Enabled = false;
            this.resultingAreaTextBox2.Location = new System.Drawing.Point(173, 56);
            this.resultingAreaTextBox2.MaxLength = 12;
            this.resultingAreaTextBox2.Name = "resultingAreaTextBox2";
            this.resultingAreaTextBox2.Size = new System.Drawing.Size(72, 20);
            this.resultingAreaTextBox2.TabIndex = 10;
            this.resultingAreaTextBox2.TabStop = false;
            this.resultingAreaTextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(170, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Resulting area";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Grid width";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Wall width";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Path width";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // squareWidthLabel
            // 
            this.squareWidthLabel.AutoSize = true;
            this.squareWidthLabel.Location = new System.Drawing.Point(16, 32);
            this.squareWidthLabel.Name = "squareWidthLabel";
            this.squareWidthLabel.Size = new System.Drawing.Size(69, 13);
            this.squareWidthLabel.TabIndex = 6;
            this.squareWidthLabel.Text = "Square width";
            this.squareWidthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // capStyleComboBox
            // 
            this.capStyleComboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.dataBindingSource, "PathCapStyle", true));
            this.capStyleComboBox.FormattingEnabled = true;
            this.capStyleComboBox.Location = new System.Drawing.Point(91, 146);
            this.capStyleComboBox.Name = "capStyleComboBox";
            this.capStyleComboBox.Size = new System.Drawing.Size(121, 21);
            this.capStyleComboBox.TabIndex = 5;
            this.capStyleComboBox.Visible = false;
            // 
            // setLayoutButton
            // 
            this.setLayoutButton.Location = new System.Drawing.Point(91, 194);
            this.setLayoutButton.Name = "setLayoutButton";
            this.setLayoutButton.Size = new System.Drawing.Size(75, 23);
            this.setLayoutButton.TabIndex = 99;
            this.setLayoutButton.Text = "Set";
            this.setLayoutButton.UseVisualStyleBackColor = true;
            this.setLayoutButton.Click += new System.EventHandler(this.OnSet);
            // 
            // colorsPage
            // 
            this.colorsPage.Location = new System.Drawing.Point(4, 22);
            this.colorsPage.Name = "colorsPage";
            this.colorsPage.Padding = new System.Windows.Forms.Padding(3);
            this.colorsPage.Size = new System.Drawing.Size(260, 223);
            this.colorsPage.TabIndex = 2;
            this.colorsPage.Text = "Colors";
            this.colorsPage.UseVisualStyleBackColor = true;
            // 
            // shapePage
            // 
            this.shapePage.Controls.Add(this.resultingAreaTextBox1);
            this.shapePage.Controls.Add(this.label12);
            this.shapePage.Controls.Add(this.setShapeButton);
            this.shapePage.Controls.Add(this.CodeLabel);
            this.shapePage.Controls.Add(this.codeTextBox);
            this.shapePage.Controls.Add(this.autoSeedCheckBox);
            this.shapePage.Controls.Add(this.label10);
            this.shapePage.Controls.Add(this.seedTextBox);
            this.shapePage.Controls.Add(this.autoMazeHeightCheckBox);
            this.shapePage.Controls.Add(this.label9);
            this.shapePage.Controls.Add(this.mazeHeightTextBox);
            this.shapePage.Controls.Add(this.label7);
            this.shapePage.Controls.Add(this.autoMazeWidthCheckBox);
            this.shapePage.Controls.Add(this.label8);
            this.shapePage.Controls.Add(this.mazeWidthBox);
            this.shapePage.Location = new System.Drawing.Point(4, 22);
            this.shapePage.Name = "shapePage";
            this.shapePage.Padding = new System.Windows.Forms.Padding(3);
            this.shapePage.Size = new System.Drawing.Size(260, 223);
            this.shapePage.TabIndex = 0;
            this.shapePage.Text = "Shape";
            this.shapePage.UseVisualStyleBackColor = true;
            // 
            // resultingAreaTextBox1
            // 
            this.resultingAreaTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.resultingAreaTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultingAreaTextBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "ResultingArea", true));
            this.resultingAreaTextBox1.Enabled = false;
            this.resultingAreaTextBox1.Location = new System.Drawing.Point(173, 56);
            this.resultingAreaTextBox1.MaxLength = 12;
            this.resultingAreaTextBox1.Name = "resultingAreaTextBox1";
            this.resultingAreaTextBox1.Size = new System.Drawing.Size(72, 20);
            this.resultingAreaTextBox1.TabIndex = 102;
            this.resultingAreaTextBox1.TabStop = false;
            this.resultingAreaTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(170, 40);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 13);
            this.label12.TabIndex = 101;
            this.label12.Text = "Resulting area";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // setShapeButton
            // 
            this.setShapeButton.Location = new System.Drawing.Point(91, 194);
            this.setShapeButton.Name = "setShapeButton";
            this.setShapeButton.Size = new System.Drawing.Size(75, 23);
            this.setShapeButton.TabIndex = 100;
            this.setShapeButton.Text = "Set";
            this.setShapeButton.UseVisualStyleBackColor = true;
            this.setShapeButton.Click += new System.EventHandler(this.OnSet);
            // 
            // CodeLabel
            // 
            this.CodeLabel.AutoSize = true;
            this.CodeLabel.Location = new System.Drawing.Point(16, 116);
            this.CodeLabel.Name = "CodeLabel";
            this.CodeLabel.Size = new System.Drawing.Size(32, 13);
            this.CodeLabel.TabIndex = 25;
            this.CodeLabel.Text = "Code";
            this.CodeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CodeLabel.Visible = false;
            // 
            // codeTextBox
            // 
            this.codeTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "Code", true));
            this.codeTextBox.Location = new System.Drawing.Point(91, 113);
            this.codeTextBox.MaxLength = 14;
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Size = new System.Drawing.Size(154, 20);
            this.codeTextBox.TabIndex = 4;
            this.codeTextBox.Text = "WWWW-WWWW-WWWW";
            this.codeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.codeTextBox.Visible = false;
            // 
            // autoSeedCheckBox
            // 
            this.autoSeedCheckBox.AutoSize = true;
            this.autoSeedCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoSeed", true));
            this.autoSeedCheckBox.Location = new System.Drawing.Point(144, 84);
            this.autoSeedCheckBox.Name = "autoSeedCheckBox";
            this.autoSeedCheckBox.Size = new System.Drawing.Size(15, 14);
            this.autoSeedCheckBox.TabIndex = 13;
            this.autoSeedCheckBox.UseVisualStyleBackColor = true;
            this.autoSeedCheckBox.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 84);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Seed";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // seedTextBox
            // 
            this.seedTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "Seed", true));
            this.seedTextBox.Location = new System.Drawing.Point(91, 81);
            this.seedTextBox.MaxLength = 5;
            this.seedTextBox.Name = "seedTextBox";
            this.seedTextBox.Size = new System.Drawing.Size(46, 20);
            this.seedTextBox.TabIndex = 3;
            this.seedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // autoMazeHeightCheckBox
            // 
            this.autoMazeHeightCheckBox.AutoSize = true;
            this.autoMazeHeightCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoMazeHeight", true));
            this.autoMazeHeightCheckBox.Location = new System.Drawing.Point(144, 58);
            this.autoMazeHeightCheckBox.Name = "autoMazeHeightCheckBox";
            this.autoMazeHeightCheckBox.Size = new System.Drawing.Size(15, 14);
            this.autoMazeHeightCheckBox.TabIndex = 12;
            this.autoMazeHeightCheckBox.UseVisualStyleBackColor = true;
            this.autoMazeHeightCheckBox.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Maze height";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mazeHeightTextBox
            // 
            this.mazeHeightTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "MazeHeight", true));
            this.mazeHeightTextBox.Location = new System.Drawing.Point(91, 55);
            this.mazeHeightTextBox.MaxLength = 3;
            this.mazeHeightTextBox.Name = "mazeHeightTextBox";
            this.mazeHeightTextBox.Size = new System.Drawing.Size(46, 20);
            this.mazeHeightTextBox.TabIndex = 2;
            this.mazeHeightTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(141, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Automatic";
            // 
            // autoMazeWidthCheckBox
            // 
            this.autoMazeWidthCheckBox.AutoSize = true;
            this.autoMazeWidthCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoMazeWidth", true));
            this.autoMazeWidthCheckBox.Location = new System.Drawing.Point(144, 32);
            this.autoMazeWidthCheckBox.Name = "autoMazeWidthCheckBox";
            this.autoMazeWidthCheckBox.Size = new System.Drawing.Size(15, 14);
            this.autoMazeWidthCheckBox.TabIndex = 11;
            this.autoMazeWidthCheckBox.UseVisualStyleBackColor = true;
            this.autoMazeWidthCheckBox.Click += new System.EventHandler(this.OnClickImmediateUpdate);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Maze width";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mazeWidthBox
            // 
            this.mazeWidthBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "MazeWidth", true));
            this.mazeWidthBox.Location = new System.Drawing.Point(91, 29);
            this.mazeWidthBox.MaxLength = 3;
            this.mazeWidthBox.Name = "mazeWidthBox";
            this.mazeWidthBox.Size = new System.Drawing.Size(46, 20);
            this.mazeWidthBox.TabIndex = 1;
            this.mazeWidthBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DetailsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 278);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(300, 300);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "DetailsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ariadne Details";
            this.tabControl1.ResumeLayout(false);
            this.layoutPage.ResumeLayout(false);
            this.layoutPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.squareWidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pathWidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wallWidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridWidthNumericUpDown)).EndInit();
            this.shapePage.ResumeLayout(false);
            this.shapePage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage shapePage;
        private System.Windows.Forms.TabPage layoutPage;
        private System.Windows.Forms.Button setLayoutButton;
        private System.Windows.Forms.TabPage colorsPage;
        private System.Windows.Forms.Label squareWidthLabel;
        private System.Windows.Forms.ComboBox capStyleComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox resultingAreaTextBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox autoGridWidthCheckBox;
        private System.Windows.Forms.CheckBox autoWallWidthCheckBox;
        private System.Windows.Forms.CheckBox autoPathWidthCheckBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox autoSquareWidthCheckbox;
        private System.Windows.Forms.BindingSource dataBindingSource;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox autoMazeWidthCheckBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox mazeWidthBox;
        private System.Windows.Forms.Label CodeLabel;
        private System.Windows.Forms.TextBox codeTextBox;
        private System.Windows.Forms.CheckBox autoSeedCheckBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox seedTextBox;
        private System.Windows.Forms.CheckBox autoMazeHeightCheckBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox mazeHeightTextBox;
        private System.Windows.Forms.TextBox resultingAreaTextBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button setShapeButton;
        private System.Windows.Forms.NumericUpDown gridWidthNumericUpDown;
        private System.Windows.Forms.NumericUpDown squareWidthNumericUpDown;
        private System.Windows.Forms.NumericUpDown pathWidthNumericUpDown;
        private System.Windows.Forms.NumericUpDown wallWidthNumericUpDown;
    }
}