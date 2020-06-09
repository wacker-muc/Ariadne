using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SWA.Ariadne.Gui.Dialogs
{
    public partial class AboutDetailsForm : Form
    {
        public AboutDetailsForm()
        {
            InitializeComponent();

            if (Environment.NewLine.Length == 2) // test for "\r\n"
            {
                // Seems like we are on a Windows system -- everything OK
                this.webBrowser.DocumentText = InsertFeatureLog(Properties.Resources.OverviewHtml, Properties.Resources.FeatureLogTxt);
            }
            else
            {
                // System.Windows.Forms.WebBrowser doesn't work on Linux or Mac-OS :-(
                // Let's replace it with a simple TextBox.
                TextBox textBox = new TextBox();

                textBox.Anchor = this.webBrowser.Anchor;
                textBox.Location = this.webBrowser.Location;
                textBox.Name = "textBox1";
                textBox.Size = this.webBrowser.Size;
                textBox.TabIndex = this.webBrowser.TabIndex;
                textBox.Text = Properties.Resources.OverviewTxt
                    + Environment.NewLine
                    + Environment.NewLine
                    + Properties.Resources.FeatureLogTxt
                    ;
                textBox.Multiline = true;
                textBox.WordWrap = true;
                textBox.ReadOnly = true;
                textBox.ScrollBars = ScrollBars.Vertical;

                this.Controls.Remove(this.webBrowser);
                this.Controls.Add(textBox);

                textBox.Select(0, 1);
                textBox.DeselectAll();
            }
        }

        private string InsertFeatureLog(string html, string txt)
        {
            #region Convert txt to HTML syntax

            string[] patterns = new string[] {
                @"^(Version.*)$",
                @"^----.*$",
                @"^ \* (.*)$",
                @"--",
            };
            string[] replacements = new string[] {
                @"<h4>$1</h4><ul>",
                @"</ul><hr />",
                @"<li>$1</li>",
                @"&ndash;",
            };

            for (int i = 0; i < patterns.Length; i++)
            {
                txt = Regex.Replace(txt, patterns[i], replacements[i], RegexOptions.Multiline);
            }
            txt = Regex.Replace(txt, @"^.*?<h4>", @"<h4>", RegexOptions.Singleline);

            #endregion

            #region Insert txt into html

            html = Regex.Replace(html, @"<!--(.*#FeatureLog.*)-->", @"$1");
            html = Regex.Replace(html, @"<!--(.*)FEATURE_LOG -->", @"$1" + txt, RegexOptions.Singleline);

            #endregion

            return html;
        }
    }
}
