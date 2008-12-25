using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SWA.Ariadne.Gui.Dialogs
{
    public partial class AboutDetailsForm : Form
    {
        public AboutDetailsForm()
        {
            InitializeComponent();

            this.Size = new Size(500, 480);
            this.webBrowser.DocumentText = InsertFeatureLog(Properties.Resources.OverviewHtml, Properties.Resources.FeatureLogTxt);
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