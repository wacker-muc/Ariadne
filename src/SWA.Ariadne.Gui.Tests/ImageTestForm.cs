using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui.Tests
{
    public partial class ImageTestForm : TestForm
    {
        public ImageTestForm()
        {
            InitializeComponent();
        }

        protected virtual void newImageButton_Click(object sender, EventArgs e)
        {
            DisplayImage();
        }

        private void DisplayImage()
        {
            Random r = SWA.Utilities.RandomFactory.CreateRandom();
            string imagePath = SelectImage(r);
            Image image = Image.FromFile(imagePath);
            this.imageButton.Image = image;
        }

        private static string SelectImage(Random r)
        {
            int p = r.Next(availableImages.Count);
            string imagePath = availableImages[p];
            return imagePath;
        }

        #region Static class constructor

        static List<string> availableImages = new List<string>();

        static ImageTestForm()
        {
            string folderPath = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);
            availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.jpg", true));
            availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.gif", true));
            availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.png", true));
        }

        #endregion
    }
}