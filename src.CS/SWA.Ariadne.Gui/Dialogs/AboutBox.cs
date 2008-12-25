using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Model;
using SWA.Ariadne.Gui.Mazes;
using SWA.Ariadne.Ctrl;

namespace SWA.Ariadne.Gui.Dialogs
{
    public partial class AboutBox : Form
        , SWA.Ariadne.Gui.Mazes.IMazeForm
    {
        #region Member variables

        private bool displayAuthorButton = false;

        #endregion

        #region Constructor

        public AboutBox()
        {
            InitializeComponent();
            InitializeComponent2();

            //  Initialize the AboutBox to display the product information from the assembly information.
            //  Change assembly information settings for your application through either:
            //  - Project->Properties->Application->Assembly Information
            //  - AssemblyInfo.cs
            this.Text = String.Format("About {0}", AssemblyProduct);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;

            FillMaze();
        }

        /// <summary>
        /// Continue after the designer generated code.
        /// </summary>
        private void InitializeComponent2()
        {
            this.mazeUserControl.MazeForm = this as IMazeForm;
        }

        #endregion

        private void FillMaze()
        {
            // Create a maze with fixed layout.
            mazeUserControl.Setup(5, 2, 3);

            // Draw the maze walls.
            mazeUserControl.MazePainter.PaintMaze(null);

            // Solve the maze.
            IMazeSolver solver = SolverFactory.CreateDefaultSolver(mazeUserControl.Maze, mazeUserControl.MazePainter);
            solver.Reset();
            solver.Solve();
        }

        #region IMazeForm implementation

        /// <summary>
        /// Places reserved areas into the maze.
        /// This method is called from the MazeUserControl before actually building the maze.
        /// </summary>
        /// <param name="maze"></param>
        public void MakeReservedAreas(Maze maze)
        {
            mazeUserControl.ReserveArea(this.okButton);
            mazeUserControl.ReserveArea(this.moreButton);
            if (displayAuthorButton)
            {
                this.outerAboutPanel.SendToBack();
                this.authorButton.BringToFront();
                mazeUserControl.ReserveArea(this.authorButton);
            }
            else
            {
                this.authorButton.SendToBack();
                this.outerAboutPanel.BringToFront();
                mazeUserControl.ReserveArea(this.outerAboutPanel);
            }
        }

        /// <summary>
        /// Displays information about the running MazeSolver in the status line.
        /// </summary>
        public void UpdateStatusLine()
        {
            // no action
        }

        /// <summary>
        /// Displays Maze and Solver characteristics in the window's caption bar.
        /// </summary>
        public void UpdateCaption()
        {
            // no action
        }

        public string StrategyName
        {
            get { return SolverFactory.DefaultStrategy.Name; }
        }

        #endregion

        #region Event handlers

        private void moreButton_Click(object sender, EventArgs e)
        {
            Form form = new AboutDetailsForm();
            form.ShowDialog();
        }

        private void mazeUserControl_Click(object sender, EventArgs e)
        {
            this.FillMaze();
        }

        private void labelCopyright_Click(object sender, EventArgs e)
        {
            this.displayAuthorButton = true;
            this.FillMaze();
        }

        private void authorButton_Click(object sender, EventArgs e)
        {
            this.displayAuthorButton = false;
            this.FillMaze();
        }

        #endregion

        #region Assembly Attribute Accessors

        public static string AssemblyTitle
        {
            get
            {
                // Get all Title attributes on this assembly
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (!String.IsNullOrEmpty(titleAttribute.Title))
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}