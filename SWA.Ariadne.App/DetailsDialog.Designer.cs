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
            this.shapePage = new System.Windows.Forms.TabPage();
            this.layoutPage = new System.Windows.Forms.TabPage();
            this.autoGridWidthCheckBox = new System.Windows.Forms.CheckBox();
            this.autoWallWidthCheckBox = new System.Windows.Forms.CheckBox();
            this.autoPathWidthCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.autoSquareWidthCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.resultingAreaTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.squareWidthLabel = new System.Windows.Forms.Label();
            this.capStyleComboBox = new System.Windows.Forms.ComboBox();
            this.grdWidthTextBox = new System.Windows.Forms.TextBox();
            this.wallWidthTextBox = new System.Windows.Forms.TextBox();
            this.pathWidthTextBox = new System.Windows.Forms.TextBox();
            this.squareWidthTextBox = new System.Windows.Forms.TextBox();
            this.setLayoutButton = new System.Windows.Forms.Button();
            this.colorsPage = new System.Windows.Forms.TabPage();
            this.dataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tabControl1.SuspendLayout();
            this.layoutPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.shapePage);
            this.tabControl1.Controls.Add(this.layoutPage);
            this.tabControl1.Controls.Add(this.colorsPage);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(268, 249);
            this.tabControl1.TabIndex = 0;
            // 
            // shapePage
            // 
            this.shapePage.Location = new System.Drawing.Point(4, 22);
            this.shapePage.Name = "shapePage";
            this.shapePage.Padding = new System.Windows.Forms.Padding(3);
            this.shapePage.Size = new System.Drawing.Size(260, 223);
            this.shapePage.TabIndex = 0;
            this.shapePage.Text = "Shape";
            this.shapePage.UseVisualStyleBackColor = true;
            // 
            // layoutPage
            // 
            this.layoutPage.Controls.Add(this.autoGridWidthCheckBox);
            this.layoutPage.Controls.Add(this.autoWallWidthCheckBox);
            this.layoutPage.Controls.Add(this.autoPathWidthCheckBox);
            this.layoutPage.Controls.Add(this.label6);
            this.layoutPage.Controls.Add(this.autoSquareWidthCheckbox);
            this.layoutPage.Controls.Add(this.label5);
            this.layoutPage.Controls.Add(this.resultingAreaTextBox);
            this.layoutPage.Controls.Add(this.label4);
            this.layoutPage.Controls.Add(this.label3);
            this.layoutPage.Controls.Add(this.label2);
            this.layoutPage.Controls.Add(this.label1);
            this.layoutPage.Controls.Add(this.squareWidthLabel);
            this.layoutPage.Controls.Add(this.capStyleComboBox);
            this.layoutPage.Controls.Add(this.grdWidthTextBox);
            this.layoutPage.Controls.Add(this.wallWidthTextBox);
            this.layoutPage.Controls.Add(this.pathWidthTextBox);
            this.layoutPage.Controls.Add(this.squareWidthTextBox);
            this.layoutPage.Controls.Add(this.setLayoutButton);
            this.layoutPage.Location = new System.Drawing.Point(4, 22);
            this.layoutPage.Name = "layoutPage";
            this.layoutPage.Padding = new System.Windows.Forms.Padding(3);
            this.layoutPage.Size = new System.Drawing.Size(260, 223);
            this.layoutPage.TabIndex = 1;
            this.layoutPage.Text = "Layout";
            this.layoutPage.UseVisualStyleBackColor = true;
            // 
            // autoGridWidthCheckBox
            // 
            this.autoGridWidthCheckBox.AutoSize = true;
            this.autoGridWidthCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoGridWidth", true));
            this.autoGridWidthCheckBox.Location = new System.Drawing.Point(134, 116);
            this.autoGridWidthCheckBox.Name = "autoGridWidthCheckBox";
            this.autoGridWidthCheckBox.Size = new System.Drawing.Size(15, 14);
            this.autoGridWidthCheckBox.TabIndex = 14;
            this.autoGridWidthCheckBox.UseVisualStyleBackColor = true;
            this.autoGridWidthCheckBox.Click += new System.EventHandler(this.OnClickAutoCheckbox);
            // 
            // autoWallWidthCheckBox
            // 
            this.autoWallWidthCheckBox.AutoSize = true;
            this.autoWallWidthCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoWallWidth", true));
            this.autoWallWidthCheckBox.Location = new System.Drawing.Point(134, 84);
            this.autoWallWidthCheckBox.Name = "autoWallWidthCheckBox";
            this.autoWallWidthCheckBox.Size = new System.Drawing.Size(15, 14);
            this.autoWallWidthCheckBox.TabIndex = 13;
            this.autoWallWidthCheckBox.UseVisualStyleBackColor = true;
            this.autoWallWidthCheckBox.Click += new System.EventHandler(this.OnClickAutoCheckbox);
            // 
            // autoPathWidthCheckBox
            // 
            this.autoPathWidthCheckBox.AutoSize = true;
            this.autoPathWidthCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoPathWidth", true));
            this.autoPathWidthCheckBox.Location = new System.Drawing.Point(134, 58);
            this.autoPathWidthCheckBox.Name = "autoPathWidthCheckBox";
            this.autoPathWidthCheckBox.Size = new System.Drawing.Size(15, 14);
            this.autoPathWidthCheckBox.TabIndex = 12;
            this.autoPathWidthCheckBox.UseVisualStyleBackColor = true;
            this.autoPathWidthCheckBox.Click += new System.EventHandler(this.OnClickAutoCheckbox);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(131, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Automatic";
            // 
            // autoSquareWidthCheckbox
            // 
            this.autoSquareWidthCheckbox.AutoSize = true;
            this.autoSquareWidthCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "AutoSquareWidth", true));
            this.autoSquareWidthCheckbox.Location = new System.Drawing.Point(134, 32);
            this.autoSquareWidthCheckbox.Name = "autoSquareWidthCheckbox";
            this.autoSquareWidthCheckbox.Size = new System.Drawing.Size(15, 14);
            this.autoSquareWidthCheckbox.TabIndex = 11;
            this.autoSquareWidthCheckbox.UseVisualStyleBackColor = true;
            this.autoSquareWidthCheckbox.Click += new System.EventHandler(this.OnClickAutoCheckbox);
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
            // 
            // resultingAreaTextBox
            // 
            this.resultingAreaTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.resultingAreaTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultingAreaTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "ResultingArea", true));
            this.resultingAreaTextBox.Enabled = false;
            this.resultingAreaTextBox.Location = new System.Drawing.Point(173, 113);
            this.resultingAreaTextBox.MaxLength = 12;
            this.resultingAreaTextBox.Name = "resultingAreaTextBox";
            this.resultingAreaTextBox.Size = new System.Drawing.Size(72, 20);
            this.resultingAreaTextBox.TabIndex = 10;
            this.resultingAreaTextBox.TabStop = false;
            this.resultingAreaTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(170, 97);
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
            // 
            // grdWidthTextBox
            // 
            this.grdWidthTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "GridWidth", true));
            this.grdWidthTextBox.Location = new System.Drawing.Point(91, 113);
            this.grdWidthTextBox.MaxLength = 2;
            this.grdWidthTextBox.Name = "grdWidthTextBox";
            this.grdWidthTextBox.Size = new System.Drawing.Size(36, 20);
            this.grdWidthTextBox.TabIndex = 4;
            this.grdWidthTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // wallWidthTextBox
            // 
            this.wallWidthTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "WallWidth", true));
            this.wallWidthTextBox.Location = new System.Drawing.Point(91, 81);
            this.wallWidthTextBox.MaxLength = 2;
            this.wallWidthTextBox.Name = "wallWidthTextBox";
            this.wallWidthTextBox.Size = new System.Drawing.Size(36, 20);
            this.wallWidthTextBox.TabIndex = 3;
            this.wallWidthTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pathWidthTextBox
            // 
            this.pathWidthTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "PathWidth", true));
            this.pathWidthTextBox.Location = new System.Drawing.Point(91, 55);
            this.pathWidthTextBox.MaxLength = 2;
            this.pathWidthTextBox.Name = "pathWidthTextBox";
            this.pathWidthTextBox.Size = new System.Drawing.Size(36, 20);
            this.pathWidthTextBox.TabIndex = 2;
            this.pathWidthTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // squareWidthTextBox
            // 
            this.squareWidthTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "SquareWidth", true));
            this.squareWidthTextBox.Location = new System.Drawing.Point(91, 29);
            this.squareWidthTextBox.MaxLength = 2;
            this.squareWidthTextBox.Name = "squareWidthTextBox";
            this.squareWidthTextBox.Size = new System.Drawing.Size(36, 20);
            this.squareWidthTextBox.TabIndex = 1;
            this.squareWidthTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // setLayoutButton
            // 
            this.setLayoutButton.Location = new System.Drawing.Point(91, 194);
            this.setLayoutButton.Name = "setLayoutButton";
            this.setLayoutButton.Size = new System.Drawing.Size(75, 23);
            this.setLayoutButton.TabIndex = 99;
            this.setLayoutButton.Text = "Set";
            this.setLayoutButton.UseVisualStyleBackColor = true;
            this.setLayoutButton.Click += new System.EventHandler(this.OnLayoutSet);
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
            // dataBindingSource
            // 
            this.dataBindingSource.DataSource = typeof(SWA.Ariadne.Settings.AriadneSettingsData);
            this.dataBindingSource.CurrentItemChanged += new System.EventHandler(this.OnLayoutDataChanged);
            // 
            // DetailsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.tabControl1);
            this.MaximumSize = new System.Drawing.Size(300, 300);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "DetailsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ariadne Details";
            this.tabControl1.ResumeLayout(false);
            this.layoutPage.ResumeLayout(false);
            this.layoutPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage shapePage;
        private System.Windows.Forms.TabPage layoutPage;
        private System.Windows.Forms.Button setLayoutButton;
        private System.Windows.Forms.TabPage colorsPage;
        private System.Windows.Forms.TextBox grdWidthTextBox;
        private System.Windows.Forms.TextBox wallWidthTextBox;
        private System.Windows.Forms.TextBox pathWidthTextBox;
        private System.Windows.Forms.TextBox squareWidthTextBox;
        private System.Windows.Forms.Label squareWidthLabel;
        private System.Windows.Forms.ComboBox capStyleComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox resultingAreaTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox autoGridWidthCheckBox;
        private System.Windows.Forms.CheckBox autoWallWidthCheckBox;
        private System.Windows.Forms.CheckBox autoPathWidthCheckBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox autoSquareWidthCheckbox;
        private System.Windows.Forms.BindingSource dataBindingSource;
    }
}