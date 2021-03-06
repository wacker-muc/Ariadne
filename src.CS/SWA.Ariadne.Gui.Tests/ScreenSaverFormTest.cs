﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using SWA.Ariadne.Gui;

namespace SWA.Ariadne.Gui.Tests
{
    /// <summary>
    ///This is a test class for SWA.Ariadne.Gui.ScreenSaverForm and is intended
    ///to contain all SWA.Ariadne.Gui.ScreenSaverForm Unit Tests
    ///</summary>
    [TestClass()]
    public class ScreenSaverFormTest
    {
        #region TestContext

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region Unit Tests

#if false
        /// <summary>
        ///A test for ScreenSaverForm (string)
        ///</summary>
        [TestMethod()]
        public void SSF_ConstructorTest_01()
        {
            Form form = CreatePreviewWindow();
            string windowHandleArg = form.Handle.ToString();

            try
            {
                ScreenSaverForm target = new ScreenSaverForm(windowHandleArg);
                target.Show();
                Application.Run(form);
            }
            catch( Exception e )
            {
                Assert.Fail(e.ToString());
            }
        }
#endif

        private static Form CreatePreviewWindow()
        {
            Form result = new Form();
            result.Name = result.Text = "Ariadne Preview";
            result.ClientSize = new System.Drawing.Size(240, 180);
            result.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            result.ControlBox = true;
            result.MaximizeBox = false;
            result.MinimizeBox = false;
            result.ShowInTaskbar = false;

            result.Show();

            return result;
        }

        #endregion
    }
}
