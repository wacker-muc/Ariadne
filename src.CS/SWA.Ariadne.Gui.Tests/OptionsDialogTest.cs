using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using SWA.Ariadne.Gui;
using SWA.Ariadne.Gui.Dialogs;

namespace SWA.Ariadne.Gui.Tests
{
    /// <summary>
    /// Summary description for OptionsDialogTest
    /// </summary>
    [TestClass]
    public class OptionsDialogTest
    {
        public OptionsDialogTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Unit Tests

        /// <summary>
        /// A test for OptionsDialog ().
        /// Verify that the controls are properly aligned.
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.dll")]
        [TestMethod()]
        public void OD_ConstructorTest_01()
        {
            string testObject = "OptionsDialog";

            OptionsDialog target = new OptionsDialog();
            SWA_Ariadne_Gui_OptionsDialogAccessor accessor = new SWA_Ariadne_Gui_OptionsDialogAccessor(target);

            Control reference;

            reference = accessor.checkBoxDetailsBox;
            AssertEqualRightAlignment(testObject, reference, accessor.checkBoxBlinking);
            AssertEqualRightAlignment(testObject, reference, accessor.checkBoxEfficientSolvers);
            AssertEqualRightAlignment(testObject, reference, accessor.checkBoxOutlineShapes);

            reference = accessor.labelImagesNumber;
            AssertEqualLeftAlignment(testObject, reference, accessor.labelImagesMinSize);
            AssertEqualLeftAlignment(testObject, reference, accessor.labelImagesMaxSize);

            reference = accessor.imageFolderTextBox;
            AssertEqualLeftAlignment(testObject, reference, accessor.imageNumberNumericUpDown);
            AssertEqualLeftAlignment(testObject, reference, accessor.imageMinSizeNumericUpDown);
            AssertEqualLeftAlignment(testObject, reference, accessor.imageMaxSizeNumericUpDown);
        }

        private static void AssertEqualLeftAlignment(string testObject, Control ctrl1, Control ctrl2)
        {
            Assert.AreEqual(ctrl1.Left, ctrl2.Left, testObject + ": left alignment of " + ctrl1.Name + " and " + ctrl2.Name);
        }

        private static void AssertEqualRightAlignment(string testObject, Control ctrl1, Control ctrl2)
        {
            Assert.AreEqual(ctrl1.Right, ctrl2.Right, testObject + ": right alignment of " + ctrl1.Name + " and " + ctrl2.Name);
        }

        /// <summary>
        /// A test for OptionsDialog ().
        /// This can be used to debug the OptionsDialog functionality.
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.dll")]
        [TestMethod()]
        public void OD_ManualTest_01()
        {
            string testObject = "OptionsDialog";

            OptionsDialog target = new OptionsDialog();
            DialogResult result = target.ShowDialog();

            Assert.AreEqual(DialogResult.OK, result, testObject);
        }

        #endregion
    }
}
