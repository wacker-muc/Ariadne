﻿// ------------------------------------------------------------------------------
//<autogenerated>
//        This code was generated by Microsoft Visual Studio Team System 2005.
//
//        Changes to this file may cause incorrect behavior and will be lost if
//        the code is regenerated.
//</autogenerated>
//------------------------------------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SWA.Ariadne.Gui.Tests
{
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class BaseAccessor {
    
    protected Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject m_privateObject;
    
    protected BaseAccessor(object target, Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType type) {
        m_privateObject = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(target, type);
    }
    
    protected BaseAccessor(Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType type) : 
            this(null, type) {
    }
    
    internal virtual object Target {
        get {
            return m_privateObject.Target;
        }
    }
    
    public override string ToString() {
        return this.Target.ToString();
    }
    
    public override bool Equals(object obj) {
        if (typeof(BaseAccessor).IsInstanceOfType(obj)) {
            obj = ((BaseAccessor)(obj)).Target;
        }
        return this.Target.Equals(obj);
    }
    
    public override int GetHashCode() {
        return this.Target.GetHashCode();
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class SWA_Ariadne_Gui_OptionsDialogAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::SWA.Ariadne.Gui.Dialogs.OptionsDialog));
    
    internal SWA_Ariadne_Gui_OptionsDialogAccessor(global::SWA.Ariadne.Gui.Dialogs.OptionsDialog target) : 
            base(target, m_privateType) {
    }
    
    internal global::System.ComponentModel.IContainer components {
        get {
            global::System.ComponentModel.IContainer ret = ((global::System.ComponentModel.IContainer)(m_privateObject.GetField("components")));
            return ret;
        }
        set {
            m_privateObject.SetField("components", value);
        }
    }
    
    internal global::System.Windows.Forms.Button buttonOK {
        get {
            global::System.Windows.Forms.Button ret = ((global::System.Windows.Forms.Button)(m_privateObject.GetField("buttonOK")));
            return ret;
        }
        set {
            m_privateObject.SetField("buttonOK", value);
        }
    }
    
    internal global::System.Windows.Forms.Button buttonCancel {
        get {
            global::System.Windows.Forms.Button ret = ((global::System.Windows.Forms.Button)(m_privateObject.GetField("buttonCancel")));
            return ret;
        }
        set {
            m_privateObject.SetField("buttonCancel", value);
        }
    }
    
    internal global::System.Windows.Forms.Label labelCopyright {
        get {
            global::System.Windows.Forms.Label ret = ((global::System.Windows.Forms.Label)(m_privateObject.GetField("labelCopyright")));
            return ret;
        }
        set {
            m_privateObject.SetField("labelCopyright", value);
        }
    }
    
    internal global::System.Windows.Forms.TabControl tabControl1 {
        get {
            global::System.Windows.Forms.TabControl ret = ((global::System.Windows.Forms.TabControl)(m_privateObject.GetField("tabControl1")));
            return ret;
        }
        set {
            m_privateObject.SetField("tabControl1", value);
        }
    }
    
    internal global::System.Windows.Forms.TabPage tabPage1 {
        get {
            global::System.Windows.Forms.TabPage ret = ((global::System.Windows.Forms.TabPage)(m_privateObject.GetField("tabPage1")));
            return ret;
        }
        set {
            m_privateObject.SetField("tabPage1", value);
        }
    }
    
    internal global::System.Windows.Forms.CheckBox checkBoxEfficientSolvers {
        get {
            global::System.Windows.Forms.CheckBox ret = ((global::System.Windows.Forms.CheckBox)(m_privateObject.GetField("checkBoxEfficientSolvers")));
            return ret;
        }
        set {
            m_privateObject.SetField("checkBoxEfficientSolvers", value);
        }
    }
    
    internal global::System.Windows.Forms.TextBox textBoxStepsPerSecond {
        get {
            global::System.Windows.Forms.TextBox ret = ((global::System.Windows.Forms.TextBox)(m_privateObject.GetField("textBoxStepsPerSecond")));
            return ret;
        }
        set {
            m_privateObject.SetField("textBoxStepsPerSecond", value);
        }
    }
    
    internal global::System.Windows.Forms.Label label1 {
        get {
            global::System.Windows.Forms.Label ret = ((global::System.Windows.Forms.Label)(m_privateObject.GetField("label1")));
            return ret;
        }
        set {
            m_privateObject.SetField("label1", value);
        }
    }
    
    internal global::System.Windows.Forms.CheckBox checkBoxDetailsBox {
        get {
            global::System.Windows.Forms.CheckBox ret = ((global::System.Windows.Forms.CheckBox)(m_privateObject.GetField("checkBoxDetailsBox")));
            return ret;
        }
        set {
            m_privateObject.SetField("checkBoxDetailsBox", value);
        }
    }
    
    internal global::System.Windows.Forms.CheckBox checkBoxBlinking {
        get {
            global::System.Windows.Forms.CheckBox ret = ((global::System.Windows.Forms.CheckBox)(m_privateObject.GetField("checkBoxBlinking")));
            return ret;
        }
        set {
            m_privateObject.SetField("checkBoxBlinking", value);
        }
    }
    
    internal global::System.Windows.Forms.TabPage tabPage2 {
        get {
            global::System.Windows.Forms.TabPage ret = ((global::System.Windows.Forms.TabPage)(m_privateObject.GetField("tabPage2")));
            return ret;
        }
        set {
            m_privateObject.SetField("tabPage2", value);
        }
    }
    
    internal global::System.Windows.Forms.NumericUpDown imageMinSizeNumericUpDown {
        get {
            global::System.Windows.Forms.NumericUpDown ret = ((global::System.Windows.Forms.NumericUpDown)(m_privateObject.GetField("imageMinSizeNumericUpDown")));
            return ret;
        }
        set {
            m_privateObject.SetField("imageMinSizeNumericUpDown", value);
        }
    }
    
    internal global::System.Windows.Forms.Label labelImagesMinSize {
        get {
            global::System.Windows.Forms.Label ret = ((global::System.Windows.Forms.Label)(m_privateObject.GetField("labelImagesMinSize")));
            return ret;
        }
        set {
            m_privateObject.SetField("labelImagesMinSize", value);
        }
    }
    
    internal global::System.Windows.Forms.NumericUpDown imageMaxSizeNumericUpDown {
        get {
            global::System.Windows.Forms.NumericUpDown ret = ((global::System.Windows.Forms.NumericUpDown)(m_privateObject.GetField("imageMaxSizeNumericUpDown")));
            return ret;
        }
        set {
            m_privateObject.SetField("imageMaxSizeNumericUpDown", value);
        }
    }
    
    internal global::System.Windows.Forms.Label labelImagesMaxSize {
        get {
            global::System.Windows.Forms.Label ret = ((global::System.Windows.Forms.Label)(m_privateObject.GetField("labelImagesMaxSize")));
            return ret;
        }
        set {
            m_privateObject.SetField("labelImagesMaxSize", value);
        }
    }
    
    internal global::System.Windows.Forms.NumericUpDown imageNumberNumericUpDown {
        get {
            global::System.Windows.Forms.NumericUpDown ret = ((global::System.Windows.Forms.NumericUpDown)(m_privateObject.GetField("imageNumberNumericUpDown")));
            return ret;
        }
        set {
            m_privateObject.SetField("imageNumberNumericUpDown", value);
        }
    }
    
    internal global::System.Windows.Forms.Label labelImagesNumber {
        get {
            global::System.Windows.Forms.Label ret = ((global::System.Windows.Forms.Label)(m_privateObject.GetField("labelImagesNumber")));
            return ret;
        }
        set {
            m_privateObject.SetField("labelImagesNumber", value);
        }
    }
    
    internal global::System.Windows.Forms.Button selectImageFolderButton {
        get {
            global::System.Windows.Forms.Button ret = ((global::System.Windows.Forms.Button)(m_privateObject.GetField("selectImageFolderButton")));
            return ret;
        }
        set {
            m_privateObject.SetField("selectImageFolderButton", value);
        }
    }
    
    internal global::System.Windows.Forms.TextBox imageFolderTextBox {
        get {
            global::System.Windows.Forms.TextBox ret = ((global::System.Windows.Forms.TextBox)(m_privateObject.GetField("imageFolderTextBox")));
            return ret;
        }
        set {
            m_privateObject.SetField("imageFolderTextBox", value);
        }
    }
    
    internal global::System.Windows.Forms.FolderBrowserDialog imageFolderBrowserDialog {
        get {
            global::System.Windows.Forms.FolderBrowserDialog ret = ((global::System.Windows.Forms.FolderBrowserDialog)(m_privateObject.GetField("imageFolderBrowserDialog")));
            return ret;
        }
        set {
            m_privateObject.SetField("imageFolderBrowserDialog", value);
        }
    }
    
    internal global::System.Windows.Forms.TabPage tabPage3 {
        get {
            global::System.Windows.Forms.TabPage ret = ((global::System.Windows.Forms.TabPage)(m_privateObject.GetField("tabPage3")));
            return ret;
        }
        set {
            m_privateObject.SetField("tabPage3", value);
        }
    }
    
    internal global::System.Windows.Forms.CheckBox checkBoxOutlineShapes {
        get {
            global::System.Windows.Forms.CheckBox ret = ((global::System.Windows.Forms.CheckBox)(m_privateObject.GetField("checkBoxOutlineShapes")));
            return ret;
        }
        set {
            m_privateObject.SetField("checkBoxOutlineShapes", value);
        }
    }
    
    internal void OptionsDialog_Load(object sender, global::System.EventArgs e) {
        object[] args = new object[] {
                sender,
                e};
        m_privateObject.Invoke("OptionsDialog_Load", new System.Type[] {
                    typeof(object),
                    typeof(global::System.EventArgs)}, args);
    }
    
    internal void buttonOK_Click(object sender, global::System.EventArgs e) {
        object[] args = new object[] {
                sender,
                e};
        m_privateObject.Invoke("buttonOK_Click", new System.Type[] {
                    typeof(object),
                    typeof(global::System.EventArgs)}, args);
    }
    
    internal void buttonCancel_Click(object sender, global::System.EventArgs e) {
        object[] args = new object[] {
                sender,
                e};
        m_privateObject.Invoke("buttonCancel_Click", new System.Type[] {
                    typeof(object),
                    typeof(global::System.EventArgs)}, args);
    }
    
    internal void selectImageFolderButton_Click(object sender, global::System.EventArgs e) {
        object[] args = new object[] {
                sender,
                e};
        m_privateObject.Invoke("selectImageFolderButton_Click", new System.Type[] {
                    typeof(object),
                    typeof(global::System.EventArgs)}, args);
    }
    
    internal void LoadSettings() {
        object[] args = new object[0];
        m_privateObject.Invoke("LoadSettings", new System.Type[0], args);
    }
    
    internal void SaveSettings() {
        object[] args = new object[0];
        m_privateObject.Invoke("SaveSettings", new System.Type[0], args);
    }
    
    internal void Dispose(bool disposing) {
        object[] args = new object[] {
                disposing};
        m_privateObject.Invoke("Dispose", new System.Type[] {
                    typeof(bool)}, args);
    }
    
    internal void InitializeComponent() {
        object[] args = new object[0];
        m_privateObject.Invoke("InitializeComponent", new System.Type[0], args);
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class SWA_Ariadne_Gui_Mazes_ContourImageAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::SWA.Ariadne.Gui.Mazes.ContourImage));
    
    internal SWA_Ariadne_Gui_Mazes_ContourImageAccessor(global::SWA.Ariadne.Gui.Mazes.ContourImage target) : 
            base(target, m_privateType) {
    }
    
    internal static int ContourDistance {
        get {
            int ret = ((int)(m_privateType.GetStaticField("ContourDistance")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("ContourDistance", value);
        }
    }
    
    internal static int BlurDistanceMax {
        get {
            int ret = ((int)(m_privateType.GetStaticField("BlurDistanceMax")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("BlurDistanceMax", value);
        }
    }
    
    internal static int MaxColorDistance {
        get {
            int ret = ((int)(m_privateType.GetStaticField("MaxColorDistance")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("MaxColorDistance", value);
        }
    }
    
    internal static int NbE {
        get {
            int ret = ((int)(m_privateType.GetStaticField("NbE")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbE", value);
        }
    }
    
    internal static int NbNE {
        get {
            int ret = ((int)(m_privateType.GetStaticField("NbNE")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbNE", value);
        }
    }
    
    internal static int NbN {
        get {
            int ret = ((int)(m_privateType.GetStaticField("NbN")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbN", value);
        }
    }
    
    internal static int NbNW {
        get {
            int ret = ((int)(m_privateType.GetStaticField("NbNW")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbNW", value);
        }
    }
    
    internal static int NbW {
        get {
            int ret = ((int)(m_privateType.GetStaticField("NbW")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbW", value);
        }
    }
    
    internal static int NbSW {
        get {
            int ret = ((int)(m_privateType.GetStaticField("NbSW")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbSW", value);
        }
    }
    
    internal static int NbS {
        get {
            int ret = ((int)(m_privateType.GetStaticField("NbS")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbS", value);
        }
    }
    
    internal static int NbSE {
        get {
            int ret = ((int)(m_privateType.GetStaticField("NbSE")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbSE", value);
        }
    }
    
    internal static int[] NbDX {
        get {
            int[] ret = ((int[])(m_privateType.GetStaticField("NbDX")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbDX", value);
        }
    }
    
    internal static int[] NbDY {
        get {
            int[] ret = ((int[])(m_privateType.GetStaticField("NbDY")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("NbDY", value);
        }
    }
    
    internal static int[,] Nb {
        get {
            int[,] ret = ((int[,])(m_privateType.GetStaticField("Nb")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("Nb", value);
        }
    }
    
    internal global::System.Drawing.Color backgroundColor {
        get {
            global::System.Drawing.Color ret = ((global::System.Drawing.Color)(m_privateObject.GetField("backgroundColor")));
            return ret;
        }
        set {
            m_privateObject.SetField("backgroundColor", value);
        }
    }
    
    internal int bgR {
        get {
            int ret = ((int)(m_privateObject.GetField("bgR")));
            return ret;
        }
        set {
            m_privateObject.SetField("bgR", value);
        }
    }
    
    internal int bgG {
        get {
            int ret = ((int)(m_privateObject.GetField("bgG")));
            return ret;
        }
        set {
            m_privateObject.SetField("bgG", value);
        }
    }
    
    internal int bgB {
        get {
            int ret = ((int)(m_privateObject.GetField("bgB")));
            return ret;
        }
        set {
            m_privateObject.SetField("bgB", value);
        }
    }
    
    internal int BlurDistance {
        get {
            int ret = ((int)(m_privateObject.GetProperty("BlurDistance")));
            return ret;
        }
    }
    
    internal int FrameWidth {
        get {
            int ret = ((int)(m_privateObject.GetProperty("FrameWidth")));
            return ret;
        }
    }
    
    internal global::System.Drawing.Bitmap template {
        get {
            global::System.Drawing.Bitmap ret = ((global::System.Drawing.Bitmap)(m_privateObject.GetField("template")));
            return ret;
        }
        set {
            m_privateObject.SetField("template", value);
        }
    }
    
    internal global::System.Drawing.Rectangle bbox {
        get {
            global::System.Drawing.Rectangle ret = ((global::System.Drawing.Rectangle)(m_privateObject.GetField("bbox")));
            return ret;
        }
        set {
            m_privateObject.SetField("bbox", value);
        }
    }
    
    internal global::System.Drawing.Bitmap mask {
        get {
            global::System.Drawing.Bitmap ret = ((global::System.Drawing.Bitmap)(m_privateObject.GetField("mask")));
            return ret;
        }
        set {
            m_privateObject.SetField("mask", value);
        }
    }
    
    internal global::System.Drawing.Graphics gMask {
        get {
            global::System.Drawing.Graphics ret = ((global::System.Drawing.Graphics)(m_privateObject.GetField("gMask")));
            return ret;
        }
        set {
            m_privateObject.SetField("gMask", value);
        }
    }
    
    internal global::System.Drawing.Bitmap image {
        get {
            global::System.Drawing.Bitmap ret = ((global::System.Drawing.Bitmap)(m_privateObject.GetField("image")));
            return ret;
        }
        set {
            m_privateObject.SetField("image", value);
        }
    }
    
    internal System.Collections.Generic.List<int>[] inside {
        get {
            System.Collections.Generic.List<int>[] ret = ((System.Collections.Generic.List<int>[])(m_privateObject.GetField("inside")));
            return ret;
        }
        set {
            m_privateObject.SetField("inside", value);
        }
    }
    
    internal System.Collections.Generic.List<int>[] border {
        get {
            System.Collections.Generic.List<int>[] ret = ((System.Collections.Generic.List<int>[])(m_privateObject.GetField("border")));
            return ret;
        }
        set {
            m_privateObject.SetField("border", value);
        }
    }
    
    internal System.Collections.Generic.List<int>[] contour {
        get {
            System.Collections.Generic.List<int>[] ret = ((System.Collections.Generic.List<int>[])(m_privateObject.GetField("contour")));
            return ret;
        }
        set {
            m_privateObject.SetField("contour", value);
        }
    }
    
    internal static void PrepareInfluenceRegions(int influenceRange) {
        object[] args = new object[] {
                influenceRange};
        m_privateType.InvokeStatic("PrepareInfluenceRegions", new System.Type[] {
                    typeof(int)}, args);
    }
    
    internal void CreateMask(int fuzziness) {
        object[] args = new object[] {
                fuzziness};
        m_privateObject.Invoke("CreateMask", new System.Type[] {
                    typeof(int)}, args);
    }
    
    internal bool ScanObject(int x0, int y0, int fuzziness, int[,] alpha) {
        object[] args = new object[] {
                x0,
                y0,
                fuzziness,
                alpha};
        bool ret = ((bool)(m_privateObject.Invoke("ScanObject", new System.Type[] {
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int).MakeArrayType(2)}, args)));
        return ret;
    }
    
    internal void LeftNeighbor(int fuzziness, int x, int y, int nbR, out int nbL, out int xL, out int yL) {
        object[] args = new object[] {
                fuzziness,
                x,
                y,
                nbR,
                null,
                null,
                null};
        m_privateObject.Invoke("LeftNeighbor", new System.Type[] {
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int).MakeByRefType(),
                    typeof(int).MakeByRefType(),
                    typeof(int).MakeByRefType()}, args);
        nbL = ((int)(args[4]));
        xL = ((int)(args[5]));
        yL = ((int)(args[6]));
    }
    
    internal void RightNeighbor(int fuzziness, int x, int y, int nbL, out int nbR, out int xR, out int yR) {
        object[] args = new object[] {
                fuzziness,
                x,
                y,
                nbL,
                null,
                null,
                null};
        m_privateObject.Invoke("RightNeighbor", new System.Type[] {
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int).MakeByRefType(),
                    typeof(int).MakeByRefType(),
                    typeof(int).MakeByRefType()}, args);
        nbR = ((int)(args[4]));
        xR = ((int)(args[5]));
        yR = ((int)(args[6]));
    }
    
    internal void InitializeScanLines(int width, int height) {
        object[] args = new object[] {
                width,
                height};
        m_privateObject.Invoke("InitializeScanLines", new System.Type[] {
                    typeof(int),
                    typeof(int)}, args);
    }
    
    internal static void InsertObjectPoint(System.Collections.Generic.List<int> scanLine, int x) {
        object[] args = new object[] {
                scanLine,
                x};
        m_privateType.InvokeStatic("InsertObjectPoint", new System.Type[] {
                    typeof(System.Collections.Generic.List<int>),
                    typeof(int)}, args);
    }
    
    internal static void InsertPair(System.Collections.Generic.List<int> scanLine, int xL, int xR) {
        object[] args = new object[] {
                scanLine,
                xL,
                xR};
        m_privateType.InvokeStatic("InsertPair", new System.Type[] {
                    typeof(System.Collections.Generic.List<int>),
                    typeof(int),
                    typeof(int)}, args);
    }
    
    internal static int EliminateInsideRegions(System.Collections.Generic.List<int>[] scanLines, int y0, int sy) {
        object[] args = new object[] {
                scanLines,
                y0,
                sy};
        int ret = ((int)(m_privateType.InvokeStatic("EliminateInsideRegions", new System.Type[] {
                    typeof(System.Collections.Generic.List<int>).MakeArrayType(),
                    typeof(int),
                    typeof(int)}, args)));
        return ret;
    }
    
    internal static System.Collections.Generic.List<int>[] ShrinkRegion(System.Collections.Generic.List<int>[] scanLines, int margin) {
        object[] args = new object[] {
                scanLines,
                margin};
        System.Collections.Generic.List<int>[] ret = ((System.Collections.Generic.List<int>[])(m_privateType.InvokeStatic("ShrinkRegion", new System.Type[] {
                    typeof(System.Collections.Generic.List<int>).MakeArrayType(),
                    typeof(int)}, args)));
        return ret;
    }
    
    internal static void IntersectScanLines(System.Collections.Generic.List<int>[] source, System.Collections.Generic.List<int>[] target, int dx, int dy) {
        object[] args = new object[] {
                source,
                target,
                dx,
                dy};
        m_privateType.InvokeStatic("IntersectScanLines", new System.Type[] {
                    typeof(System.Collections.Generic.List<int>).MakeArrayType(),
                    typeof(System.Collections.Generic.List<int>).MakeArrayType(),
                    typeof(int),
                    typeof(int)}, args);
    }
    
    internal static void UniteScanLines(System.Collections.Generic.List<int>[] source, System.Collections.Generic.List<int>[] target) {
        object[] args = new object[] {
                source,
                target};
        m_privateType.InvokeStatic("UniteScanLines", new System.Type[] {
                    typeof(System.Collections.Generic.List<int>).MakeArrayType(),
                    typeof(System.Collections.Generic.List<int>).MakeArrayType()}, args);
    }
    
    internal static void IntersectScanLines(System.Collections.Generic.List<int>[] source, System.Collections.Generic.List<int>[] target, int dx, int dy, int p0) {
        object[] args = new object[] {
                source,
                target,
                dx,
                dy,
                p0};
        m_privateType.InvokeStatic("IntersectScanLines", new System.Type[] {
                    typeof(System.Collections.Generic.List<int>).MakeArrayType(),
                    typeof(System.Collections.Generic.List<int>).MakeArrayType(),
                    typeof(int),
                    typeof(int),
                    typeof(int)}, args);
    }
    
    internal float GuessBackgroundColor(int fuzziness) {
        object[] args = new object[] {
                fuzziness};
        float ret = ((float)(m_privateObject.Invoke("GuessBackgroundColor", new System.Type[] {
                    typeof(int)}, args)));
        return ret;
    }
    
    internal int ColorDistance(global::System.Drawing.Color color) {
        object[] args = new object[] {
                color};
        int ret = ((int)(m_privateObject.Invoke("ColorDistance", new System.Type[] {
                    typeof(global::System.Drawing.Color)}, args)));
        return ret;
    }
    
    internal void CreateImage() {
        object[] args = new object[0];
        m_privateObject.Invoke("CreateImage", new System.Type[0], args);
    }
    
    internal void ApplyMask() {
        object[] args = new object[0];
        m_privateObject.Invoke("ApplyMask", new System.Type[0], args);
    }
    
    internal static global::System.Drawing.Bitmap Crop(global::System.Drawing.Bitmap image, global::System.Drawing.Rectangle srcRect) {
        object[] args = new object[] {
                image,
                srcRect};
        global::System.Drawing.Bitmap ret = ((global::System.Drawing.Bitmap)(m_privateType.InvokeStatic("Crop", new System.Type[] {
                    typeof(global::System.Drawing.Bitmap),
                    typeof(global::System.Drawing.Rectangle)}, args)));
        return ret;
    }
    
    internal void FillOutside(global::System.Drawing.Color color, System.Collections.Generic.List<int>[] scanLines) {
        object[] args = new object[] {
                color,
                scanLines};
        m_privateObject.Invoke("FillOutside", new System.Type[] {
                    typeof(global::System.Drawing.Color),
                    typeof(System.Collections.Generic.List<int>).MakeArrayType()}, args);
    }
    
    internal static global::System.Drawing.Rectangle BoundingBox(System.Collections.Generic.List<int>[] scanLines) {
        object[] args = new object[] {
                scanLines};
        global::System.Drawing.Rectangle ret = ((global::System.Drawing.Rectangle)(m_privateType.InvokeStatic("BoundingBox", new System.Type[] {
                    typeof(System.Collections.Generic.List<int>).MakeArrayType()}, args)));
        return ret;
    }
    
    internal void PaintGradient(int[,] alpha, int contourDist, int blurDist, global::System.Drawing.Color black) {
        object[] args = new object[] {
                alpha,
                contourDist,
                blurDist,
                black};
        m_privateObject.Invoke("PaintGradient", new System.Type[] {
                    typeof(int).MakeArrayType(2),
                    typeof(int),
                    typeof(int),
                    typeof(global::System.Drawing.Color)}, args);
    }
    
    internal void DrawContour(System.Collections.Generic.List<int>[] scanLines, global::System.Drawing.Color color) {
        object[] args = new object[] {
                scanLines,
                color};
        m_privateObject.Invoke("DrawContour", new System.Type[] {
                    typeof(System.Collections.Generic.List<int>).MakeArrayType(),
                    typeof(global::System.Drawing.Color)}, args);
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class SWA_Ariadne_Gui_Mazes_ImageLoaderAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::SWA.Ariadne.Gui.Mazes.ImageLoader));
    
    internal SWA_Ariadne_Gui_Mazes_ImageLoaderAccessor(global::SWA.Ariadne.Gui.Mazes.ImageLoader target) : 
            base(target, m_privateType) {
    }
    
    internal string imageFolder {
        get {
            string ret = ((string)(m_privateObject.GetField("imageFolder")));
            return ret;
        }
        set {
            m_privateObject.SetField("imageFolder", value);
        }
    }
    
    internal int minSize {
        get {
            int ret = ((int)(m_privateObject.GetField("minSize")));
            return ret;
        }
        set {
            m_privateObject.SetField("minSize", value);
        }
    }
    
    internal int maxSize {
        get {
            int ret = ((int)(m_privateObject.GetField("maxSize")));
            return ret;
        }
        set {
            m_privateObject.SetField("maxSize", value);
        }
    }
    
    internal global::System.Collections.Queue queue {
        get {
            global::System.Collections.Queue ret = ((global::System.Collections.Queue)(m_privateObject.GetField("queue")));
            return ret;
        }
        set {
            m_privateObject.SetField("queue", value);
        }
    }
    
    internal int queueLength {
        get {
            int ret = ((int)(m_privateObject.GetField("queueLength")));
            return ret;
        }
        set {
            m_privateObject.SetField("queueLength", value);
        }
    }
    
    internal global::System.Threading.Thread thread {
        get {
            global::System.Threading.Thread ret = ((global::System.Threading.Thread)(m_privateObject.GetField("thread")));
            return ret;
        }
        set {
            m_privateObject.SetField("thread", value);
        }
    }
    
    internal global::System.Threading.Semaphore queueEmptySemaphore {
        get {
            global::System.Threading.Semaphore ret = ((global::System.Threading.Semaphore)(m_privateObject.GetField("queueEmptySemaphore")));
            return ret;
        }
        set {
            m_privateObject.SetField("queueEmptySemaphore", value);
        }
    }
    
    internal global::System.Threading.Semaphore queueFullSemaphore {
        get {
            global::System.Threading.Semaphore ret = ((global::System.Threading.Semaphore)(m_privateObject.GetField("queueFullSemaphore")));
            return ret;
        }
        set {
            m_privateObject.SetField("queueFullSemaphore", value);
        }
    }
    
    internal void LoadImages() {
        object[] args = new object[0];
        m_privateObject.Invoke("LoadImages", new System.Type[0], args);
    }
    
    internal global::SWA.Ariadne.Gui.Mazes.ContourImage LoadImage(string imagePath, global::System.Random r) {
        object[] args = new object[] {
                imagePath,
                r};
        global::SWA.Ariadne.Gui.Mazes.ContourImage ret = ((global::SWA.Ariadne.Gui.Mazes.ContourImage)(m_privateObject.Invoke("LoadImage", new System.Type[] {
                    typeof(string),
                    typeof(global::System.Random)}, args)));
        return ret;
    }
    
    internal System.Collections.Generic.List<string> FindImages(string folderPath, int count, bool quickSearch, global::System.Random r) {
        object[] args = new object[] {
                folderPath,
                count,
                quickSearch,
                r};
        System.Collections.Generic.List<string> ret = ((System.Collections.Generic.List<string>)(m_privateObject.Invoke("FindImages", new System.Type[] {
                    typeof(string),
                    typeof(int),
                    typeof(bool),
                    typeof(global::System.Random)}, args)));
        return ret;
    }
}
}
