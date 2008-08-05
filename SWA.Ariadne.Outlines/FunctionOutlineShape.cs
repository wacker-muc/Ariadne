using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SWA.Utilities;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// An OutlineShape defined by a geometrical function in two dimensions: f(x,y).
    /// The division between inside and outside is defined by the function's roots,
    /// i.e. the contour is where the function f(x,y) becomes zero.
    /// </summary>
    internal class FunctionOutlineShape : OutlineShape
    {
        #region Class variables and class constructor

        private static List<MethodInfo> Functions = new List<MethodInfo>();
        private static List<TDFAttribute> Attributes = new List<TDFAttribute>();

        /// <summary>
        /// This class initially needs to collect a set of methods that can generate two-dimensional function shapes.
        /// </summary>
        /// Note: We need to collect the functions of subclasses, as well.
        ///       Using their class constructors would not work as they are not called until some other method is called.
        static FunctionOutlineShape()
        {
            CollectFunctions(typeof(FunctionOutlineShape));
            CollectFunctions(typeof(FractalOutlineShape));
        }

        /// <summary>
        /// Collect all (internal) methods that have the TDF attribute.
        /// </summary>
        /// <param name="classType"></param>
        protected static void CollectFunctions(Type classType)
        {
            Type attributeType = typeof(TDFAttribute);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (System.Reflection.MethodInfo info in classType.GetMethods(flags))
            {
                object[] attributes = info.GetCustomAttributes(attributeType, false);
                if (attributes.Length > 0)
                {
                    Functions.Add(info);
                    Attributes.Add((TDFAttribute)attributes[0]);
                }
            }
        }

        #endregion

        #region Member variables and Properties

        /// <summary>
        /// A two dimensional function.
        /// </summary>
        private delegate double FunctionDelegate(double x, double y);
        private FunctionDelegate f;

        /// <summary>
        /// General purpose parameters controlling the function.
        /// Should be initialized by a configuration method.
        /// </summary>
        protected double t1 = 0, t2 = 0, t3 = 0;

        /// <summary>
        /// One of four orientations of the coordinate system.
        /// For value 0, the function is called with parameters f(r, phi) instead of f(x, y).
        /// </summary>
        private int symmetryRotation;

        /// <summary>
        /// Offset applied to shape coordinates.  The shape center will be the function's coordinate system origin.
        /// </summary>
        private double xOffset, yOffset;

        /// <summary>
        /// Factor applied for converting shape coordinates to function coordinates.
        /// </summary>
        private double scale;

        /// <summary>
        /// A distortion method applied to the input coordinates, immediately before the function is called.
        /// </summary>
        private delegate void DistortionDelegate(ref double x, ref double y, double a, double f);
        private DistortionDelegate distortion;
        private double distortionAmplitude;
        private double distortionFrequency;

        /// <summary>
        /// Returns true if the given point is inside the shape.
        /// Here: ... if the function evaluates to a positive value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool this[int x, int y]
        {
            get
            {
                double dx = scale * (x + xOffset), dy = -scale * (y + yOffset);
                double arg1, arg2;

                if (symmetryRotation == 0)
                {
                    double r, phi;
                    Geometry.RectToPolar(dx, dy, out r, out phi);
                    arg1 = r; arg2 = phi;
                }
                else
                {
                    switch (symmetryRotation)
                    {
                        default:
                        case 1: // natural orientation
                            arg1 = dx; arg2 = dy;
                            break;
                        case 2: // 90 degrees rotated
                            arg1 = -dy; arg2 = dx;
                            break;
                        case 3: // 180 degrees rotated
                            arg1 = -dx; arg2 = -dy;
                            break;
                        case 4: // 270 degrees rotated
                            arg1 = dy; arg2 = -dx;
                            break;
                    }
                }

                if (distortion != null)
                {
                    distortion(ref arg1, ref arg2, distortionAmplitude, distortionFrequency);
                }

                double z = f(arg1, arg2);
                return (z > 0);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create an OutlineShape based on a two dimensional function.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        /// Note: The constructor must be public as it is invoked via reflection.
        public FunctionOutlineShape(int xSize, int ySize, double centerX, double centerY, double shapeSize, MethodInfo function, int symmetryRotation)
            : base(xSize, ySize)
        {
            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xc, out yc, out sz);

            // The coordinate system's center is adjusted to an integer value (in shape coordinates)
            this.xOffset = -Math.Round(xc);
            this.yOffset = -Math.Round(yc);

            double n = 5; // number of units (in function coordinates) that span the shape's size
            this.scale = n / sz;

            this.f = (FunctionDelegate)Delegate.CreateDelegate(typeof(FunctionDelegate), this, function);
            this.symmetryRotation = symmetryRotation;
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape RandomInstance(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            int p = r.Next(Functions.Count);
            //p = Functions.Count - 2 + r.Next(2);
            //p = r.Next(Functions.Count - 2);
            //p = r.Next(2);
            //p = 4;
            MethodInfo function = Functions[p];
            TDFAttribute characteristics = Attributes[p];

            int symmetryRotation;
            if (characteristics.symmetry == 0)
            {
                symmetryRotation = 0;
            }
            else
            {
                symmetryRotation = 1 + r.Next(4 / characteristics.symmetry);
            }

            double scale = characteristics.scaleMin + r.NextDouble() * (characteristics.scaleMax - characteristics.scaleMin);

            // Call the declaring type's constructor.
            FunctionOutlineShape result = (FunctionOutlineShape)function.DeclaringType.GetConstructor(
                new Type[7] { typeof(int), typeof(int), typeof(double), typeof(double), typeof(double), typeof(MethodInfo), typeof(int) }).Invoke(
                new object[7] { xSize, ySize, centerX, centerY, scale * shapeSize, function, symmetryRotation }
                );

            result.ConfigureDistortion(function, r);
            result.ConfigureFunctionParameters(function, r);

            return result;
        }

        /// <summary>
        /// Apply the function's configuration attribute to set up the t1, t2, t3 parameters.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="r"></param>
        private void ConfigureFunctionParameters(MethodInfo function, Random r)
        {
            // Reset general purpose parameters.
            t1 = t2 = t3 = 0.0;

            // Execute the function's configurator.
            Type attributeType = typeof(TDFConfiguratorAttribute);
            object[] attributes = function.GetCustomAttributes(attributeType, false);
            if (attributes.Length > 0)
            {
                // Note: this.GetType() may be a subclass; the method name must be definied by that subclass
                TDFConfiguratorAttribute cfgAttribute = (TDFConfiguratorAttribute)attributes[0];
                MethodInfo cfgMethod = this.GetType().GetMethod(cfgAttribute.methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                cfgMethod.Invoke(this, new object[1] { r });
            }
        }

        /// <summary>
        /// Apply the function's distortion attribute to set up the distortion parameters.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="r"></param>
        private void ConfigureDistortion(MethodInfo function, Random r)
        {
            Type attributeType = typeof(TDFDistortionAttribute);
            object[] attributes = function.GetCustomAttributes(attributeType, false);
            if (attributes.Length > 0)
            {
                double pRejected = 0.0;
                for (int i = 0; i < attributes.Length; i++)
                {
                    TDFDistortionAttribute attr = (TDFDistortionAttribute)attributes[i];
                    if (r.NextDouble() - pRejected < attr.probability)
                    {
                        MethodInfo distortionInfo = typeof(TDFDistortionAttribute).GetMethod(attr.methodName);
                        this.distortion = (DistortionDelegate)Delegate.CreateDelegate(typeof(DistortionDelegate), distortionInfo);
                        this.distortionAmplitude = attr.aMin + r.NextDouble() * (attr.aMax - attr.aMin);
                        if (attr.fStep > 0)
                        {
                            this.distortionFrequency = attr.fStep * r.Next((int)attr.fMin, (int)attr.fMax + 1);
                        }
                        else
                        {
                            this.distortionFrequency = attr.fMin + r.NextDouble() * (attr.fMax - attr.fMin);
                        }
                        break;
                    }
                    else
                    {
                        pRejected += attr.probability;
                    }
                }
            }
        }

        #endregion

        #region Geometric functions, tagged with the TDF attribute

        /// <summary>
        /// Cosine of x.
        /// Creates parallel stripes.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(2, 0.5, 1.0)]
        [TDFDistortion(0.10, "DistortX_CosY", 0.2, 0.5, 0.5, 2.0)]
        [TDFDistortion(0.10, "DistortX_CosY_Alternating", 0.15, 0.3, 0.5, 2.0)]
        [TDFDistortion(0.20, "DistortXY_StretchY_Rotate", 1.0, 1.0, 1, 5, Math.PI / 12)]
        [TDFDistortion(0.15, "DistortXY_Exp", 0.25, 0.5)]
        [TDFDistortion(0.15, "DistortXY_Exp", -0.15, -0.3)]
        private double TDF_Stripes(double x, double y)
        {
            return Math.Cos(0.5 * Math.PI * x);
        }

        /// <summary>
        /// Cosine of x multiplied by cosine of y.
        /// Creates checkered squares.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(4, 0.5, 1.0)]
        [TDFDistortion(0.07, "DistortX_CosY", 0.2, 0.5, 0.5, 2.0)]
        [TDFDistortion(0.07, "DistortY_CosX", 0.2, 0.5, 0.5, 2.0)]
        [TDFDistortion(0.15, "DistortXY_CosY_CosX", 0.2, 0.5, 1, 4, 0.5)]
        [TDFDistortion(0.11, "DistortXY_CosY_CosX_Alternating", 0.15, 0.3, 1, 4, 0.5)]
        [TDFDistortion(0.10, "DistortXY_StretchY_Rotate", 1.0, 1.0, 1, 3, Math.PI / 8)]
        [TDFDistortion(0.15, "DistortXY_Exp", 0.25, 0.5)]
        [TDFDistortion(0.15, "DistortXY_Exp", -0.15, -0.3)]
        private double TDF_Squares(double x, double y)
        {
            return TDF_Stripes(x, y) * TDF_Stripes(y, x);
        }

        /// <summary>
        /// Cosine of x multiplied by cosine of y, subtracting a specific constant.
        /// Creates a sparse array of circles!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //[TDF(4, 1.0, 1.0)]
        private double TDF_SmallCirclesSparse(double x, double y)
        {
            return TDF_Squares(x, y) - 0.6204;
        }

        /// <summary>
        /// Cosine of x multiplied by cosine of y, subtracting a small value.
        /// Creates a sparse array of rounded squares; the other squares are connected yia their diagonals!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(4, 0.8, 1.2)]
        [TDFDistortion(0.2, "DistortXY_CosY_CosX", 0.1, 0.3)]
        [TDFDistortion(0.3, "DistortXY_CosY_CosX_Alternating", 0.1, 0.3)]
        private double TDF_RoundedSquaresSparse(double x, double y)
        {
            return TDF_Squares(x, y) - 0.05;
        }

        /// <summary>
        /// Cosine of x multiplied by cosine of y, subtracting a specific constant.
        /// Creates an array of circles!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //[TDF(4, 0.8, 1.2)]
        private double TDF_SmallCircles(double x, double y)
        {
            return Math.Abs(TDF_Squares(x, y)) - 0.6204;
        }

        /// <summary>
        /// Cosine of x multiplied by cosine of y, subtracting a small value.
        /// Creates a closely packed grid of (rounded) tiles.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(4, 0.8, 1.2)]
        [TDFConfigurator("TDF_RoundedSquares_Configurator")]
        [TDFDistortion(0.15, "DistortXY_CosY_CosX", 0.2, 0.3)]
        [TDFDistortion(0.25, "DistortXY_CosY_CosX_Alternating", 0.15, 0.25)]
        [TDFDistortion(0.15, "DistortXY_Exp", 0.25, 0.5)]
        [TDFDistortion(0.15, "DistortXY_Exp", -0.15, -0.3)]
        private double TDF_RoundedSquares(double x, double y)
        {
            // The shift parameter is calibrated so that there is a one-square wide path between the tiles.
            return Math.Abs(TDF_Squares(x, y)) - t1;
        }

        /// <summary>
        /// Calibrate the parameter t1.
        /// Goal: There should be a small gap (approx. one square wide) between the tiles.
        /// Some tiles may even touch.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="squaresPerUnit"></param>
        private void TDF_RoundedSquares_Configurator(Random r)
        {
            // Find the minimum function value at integer coordinates, when t1 is 0.
            t1 = 0;
            double xMin = 0, zMin = TDF_RoundedSquares(xMin, 0);
            for (int i = 1; i <= 6; i++)
            {
                double x = Math.Round(i / this.scale) * this.scale;
                double z = TDF_RoundedSquares(x, 0);
                if (z < zMin)
                {
                    zMin = z;
                    xMin = x;
                }
            }

            // Get the smaller of the two function values at half a square width distance.
            double delta = 0.5 * this.scale;
            zMin = Math.Min(TDF_RoundedSquares(xMin - delta, 0), TDF_RoundedSquares(xMin + delta, 0));

            // t1 can be determined directly from zMin:
            // If t1 > zMin, the function value will become negative.
            t1 = zMin * 1.001;

            // For a distorted coordinate system, we need a larger value to get connected gap lines.
            if (this.distortion != null)
            {
                t1 *= 1.5;
            }
        }

        /// <summary>
        /// Paraboloid.
        /// Creates a circle or ellipsis.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(4, 1.5, 2.0)]
        [TDFDistortion(1.0, "DistortXY_StretchY_Rotate", 1.6, 3.0, 0.0, Math.PI)]
        private double TDF_Circle(double x, double y)
        {
            return (1.0 - (x * x + y * y));
        }

        /// <summary>
        /// Double cone.
        /// Creates a hyperbola.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(2)]
        [TDFDistortion(0.4, "DistortY_Stretch", 0.5, 1.5)]
        [TDFDistortion(0.2, "DistortXY_StretchY_Rotate", 0.8, 1.2, Math.PI * 1 / 12, Math.PI * 5 / 12)]
        private double TDF_Hyperbola(double x, double y)
        {
            return (1.0 - (x * x - y * y));
        }

        /// <summary>
        /// Creates concentric circles.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(0, 0.5, 2.0)]
        [TDFDistortion(0.3, "DistortR_Exp", -0.25, -0.15)]
        [TDFDistortion(0.3, "DistortR_Exp", +0.2, +0.5)]
        private double TDF_ConcentricCircles(double r, double phi)
        {
            return Math.Cos(0.5 * Math.PI * r);
        }

        /// <summary>
        /// Creates a spiral.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(0, 0.4, 1.2)]
        [TDFDistortion(0.3, "DistortR_Exp", -0.4, -0.2)]
        [TDFDistortion(0.3, "DistortR_Exp", +0.2, +0.5)]
        private double TDF_Spiral(double r, double phi)
        {
            return Math.Cos(0.5 * Math.PI * r + phi);
        }

        #endregion
    }

    /// <summary>
    /// A method attribute that describes the characteristics of a two-dimensional function.
    /// For (symmetry = 0), the function is called with parameters f(r, phi), otherwise f(x, y).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class TDFAttribute : System.Attribute
    {
        #region Member variables

        /// <summary>
        /// Rotational symmetry: 1 = single, 2 = mirror, 4 = fourfold, 0 = rotational
        /// </summary>
        public readonly int symmetry = 1;

        /// <summary>
        /// Range of additional scaling factors to be applied to the coordinate system.
        /// </summary>
        public readonly double scaleMin = 1.0, scaleMax = 1.0;

        #endregion

        #region Constructor

        public TDFAttribute(int symmetry)
        {
            this.symmetry = symmetry;
        }

        public TDFAttribute(int symmetry, double scaleMin, double scaleMax)
        {
            this.symmetry = symmetry;
            this.scaleMin = scaleMin;
            this.scaleMax = scaleMax;
        }

        #endregion
    }

    /// <summary>
    /// A method attribute that defines a configurator method for the function it is applied to.
    /// The configurator is called once when the FunctionOutlineShape is constructed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class TDFConfiguratorAttribute : System.Attribute
    {
        public readonly string methodName;

        public TDFConfiguratorAttribute(string configuratorMethodName)
        {
            this.methodName = configuratorMethodName;
        }
    }

    /// <summary>
    /// A method attribute that defines a distortion of a function's input parameters.
    /// X any Y (or R and Phi) are slightly modified.
    /// </summary>
    /// 
    /// Example:
    /// DistortX_CosY() subtracts the cosine of Y from X.
    /// The function value calculated for the coordinates (x,y) is not f(x,y) but f(x-d,y).
    /// The outline contour that follows the function's roots is modulated:
    /// a straight (vertical) line becomes a cosine wave.
    /// 
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class TDFDistortionAttribute : System.Attribute
    {
        #region Member variables

        /// <summary>
        /// Probability with which this distortion should be applied.
        /// </summary>
        public readonly double probability;

        /// <summary>
        /// Method defined by the TDFDistortionAttribute class.
        /// </summary>
        public readonly string methodName;

        public MethodInfo method;

        /// <summary>
        /// Amplitude range: 0 .. 1.
        /// </summary>
        public readonly double aMin, aMax;

        /// <summary>
        /// Frequency range: 1/2 .. 2.
        /// If fStep is not zero, multiples of fStep.
        /// </summary>
        public readonly double fMin, fMax;

        /// <summary>
        /// The frequency should be rounded to a multiple of fStep (if positive).
        /// </summary>
        public readonly double fStep;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="probability">0..1</param>
        /// <param name="distortionMethodName"></param>
        /// <param name="aMin">minimum distortion amplitude, in shape coordinate units</param>
        /// <param name="aMax">maximum distortion amplitude, in shape coordinate units</param>
        public TDFDistortionAttribute(double probability, string distortionMethodName, double aMin, double aMax)
            : this(probability, distortionMethodName, aMin, aMax, 1, 1)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="probability">0..1</param>
        /// <param name="distortionMethodName"></param>
        /// <param name="aMin">minimum distortion amplitude, in shape coordinate units</param>
        /// <param name="aMax">maximum distortion amplitude, in shape coordinate units</param>
        /// <param name="fMin">minimum distortion frequency, approx. 1.0</param>
        /// <param name="fMax">maximum distortion frequency, approx. 1.0</param>
        public TDFDistortionAttribute(double probability, string distortionMethodName, double aMin, double aMax, double fMin, double fMax)
        {
            this.probability = probability;
            this.methodName = distortionMethodName;
            this.aMin = aMin;
            this.aMax = aMax;
            this.fMin = fMin;
            this.fMax = fMax;

            this.method = this.GetType().GetMethod(distortionMethodName);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="probability">0..1</param>
        /// <param name="distortionMethodName"></param>
        /// <param name="aMin">minimum distortion amplitude, in shape coordinate units</param>
        /// <param name="aMax">maximum distortion amplitude, in shape coordinate units</param>
        /// <param name="fMin">minimum distortion frequency, multiplied with fStep</param>
        /// <param name="fMax">maximum distortion frequency, multiplied with fStep</param>
        /// <param name="fStep">distortion frequency stepping</param>
        public TDFDistortionAttribute(double probability, string distortionMethodName, double aMin, double aMax, int fMin, int fMax, double fStep)
            : this(probability, distortionMethodName, aMin, aMax, fMin, fMax)
        {
            this.fStep = fStep;
        }

        #endregion

        #region Coordinate distortion methods.

        /// <summary
        /// Distort the X parameter.
        /// Add a cosine wave.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a">amplitude</param>
        /// <param name="f">frequency</param>
        public static void DistortX_CosY(ref double x, ref double y, double a, double f)
        {
            x -= a * Math.Cos(0.5 * Math.PI * f * y);
        }

        /// <summary
        /// Distort the X parameter.
        /// Add a cosine wave.
        /// Every second wave is inverted.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a">amplitude</param>
        /// <param name="f">frequency</param>
        public static void DistortX_CosY_Alternating(ref double x, ref double y, double a, double f)
        {
            // The wave lines are located where x or y is an odd number.
            double sgnX = ((int)Math.Round(0.5 * (x - 1.0)) % 2 == 0 ? +1.0 : -1.0);

            x -= sgnX * a * Math.Cos(0.5 * Math.PI * f * y);
        }

        /// <summary>
        /// Distort the Y parameter.
        /// Add a cosine wave.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a">amplitude</param>
        /// <param name="f">frequency</param>
        public static void DistortY_CosX(ref double x, ref double y, double a, double f)
        {
            y -= a * Math.Cos(0.5 * Math.PI * f * x);
        }

        /// <summary>
        /// Distort the Y parameter.
        /// Divide by A, thus stretching the shape.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a">amplitude</param>
        /// <param name="f">frequency</param>
        public static void DistortY_Stretch(ref double x, ref double y, double a, double f)
        {
            y /= a;
        }

        /// <summary>
        /// Distort the X and Y parameter.
        /// Stretch the shape by A in Y direction and rotate it by F.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a">amplitude</param>
        /// <param name="f">frequency</param>
        /// 
        /// Note: The distortions need to be applied in inverse order: first rotate, then stretch.
        /// 
        public static void DistortXY_StretchY_Rotate(ref double x, ref double y, double a, double f)
        {
            double r, phi;
            Geometry.RectToPolar(x, y, out r, out phi);
            phi -= f;
            Geometry.PolarToRect(r, phi, out x, out y);

            y /= a;
        }

        /// <summary>
        /// Distort the X and Y parameters.
        /// Add a cosine wave.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a">amplitude</param>
        /// <param name="f">frequency</param>
        public static void DistortXY_CosY_CosX(ref double x, ref double y, double a, double f)
        {
            double x0 = x;
            x -= a * Math.Cos(0.5 * Math.PI * f * y);
            y -= a * Math.Cos(0.5 * Math.PI * f * x0);
        }

        /// <summary>
        /// Distort the X and Y parameters.
        /// Add a cosine wave.
        /// Every second wave is inverted.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a">amplitude</param>
        /// <param name="f">frequency</param>
        public static void DistortXY_CosY_CosX_Alternating(ref double x, ref double y, double a, double f)
        {
            // The wave lines are located where x or y is an odd number.
            double sgnX = ((int)Math.Round(0.5 * (x - 1.0)) % 2 == 0 ? +1.0 : -1.0);
            double sgnY = ((int)Math.Round(0.5 * (y - 1.0)) % 2 == 0 ? +1.0 : -1.0);

            double x0 = x;
            x -= sgnX * a * Math.Cos(0.5 * Math.PI * f * y);
            y -= sgnY * a * Math.Cos(0.5 * Math.PI * f * x0);
        }

        /// <summary>
        /// Distort the X and Y parameters.
        /// Scale exponentially, creating a concave bending.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a">amplitude</param>
        /// <param name="f">frequency</param>
        public static void DistortXY_Exp(ref double x, ref double y, double a, double f)
        {
            double r, phi;
            Geometry.RectToPolar(x, y, out r, out phi);
            DistortR_Exp(ref r, ref phi, a, f);
            Geometry.PolarToRect(r, phi, out x, out y);
        }

        /// <summary>
        /// Distort the R parameter.
        /// Scale exponentially, creating a concave bending.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="phi"></param>
        /// <param name="a">amplitude</param>
        /// <param name="f">frequency</param>
        public static void DistortR_Exp(ref double r, ref double phi, double a, double f)
        {
            r = Math.Pow(r, 1 + a);

            // Compensate for the overall change of scale.
            // This gives a reasonably balanced picture.
            r /= (1 + a);
        }

        #endregion
    }
}
