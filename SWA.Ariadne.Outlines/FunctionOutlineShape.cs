using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SWA.Utilities;

namespace SWA.Ariadne.Outlines
{
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
        private MethodInfo f;

        /// <summary>
        /// General purpose parameters controlling the function.
        /// Should be initialized by a configuration method.
        /// </summary>
        protected double t1 = 0, t2 = 0, t3 = 0;

        /// <summary>
        /// One of four orientations of the coordinate system.
        /// TODO: eight orientations
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
                object[] args;

                if (symmetryRotation == 0)
                {
                    double r, phi;
                    Geometry.RectToPolar(dx, dy, out r, out phi);
                    args = new object[] { r, phi };
                }
                else
                {
                    switch (symmetryRotation)
                    {
                        default:
                        case 1: // natural orientation
                            args = new object[] { dx, dy };
                            break;
                        case 2: // 90 degrees rotated
                            args = new object[] { -dy, dx };
                            break;
                        case 3: // 180 degrees rotated
                            args = new object[] { -dx, -dy };
                            break;
                        case 4: // 270 degrees rotated
                            args = new object[] { dy, -dx };
                            break;
                    }
                }

                double z = (double)f.Invoke(this, args);
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
        {
            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xc, out yc, out sz);

            // The coordinate system's center is adjusted to an integer value (in shape coordinates)
            this.xOffset = -Math.Round(xc);
            this.yOffset = -Math.Round(yc);

            double n = 5; // number of units (in function coordinates) that span the shape's size
            this.scale = n / sz;

            this.f = function;
            this.symmetryRotation = symmetryRotation;
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape RandomInstance(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            int p = r.Next(Functions.Count);
            //p = Functions.Count - 2 + r.Next(2);
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
            
            // Reset general purpose parameters.
            result.t1 = result.t2 = result.t3 = 0.0;

            // Execute the function's configurator.
            Type attributeType = typeof(TDFConfiguratorAttribute);
            object[] attributes = function.GetCustomAttributes(attributeType, false);
            if (attributes.Length > 0)
            {
                // Note: result.GetType() may be a subclass; the method name must be definied by that subclass
                TDFConfiguratorAttribute cfgAttribute = (TDFConfiguratorAttribute)attributes[0];
                MethodInfo cfgMethod = result.GetType().GetMethod(cfgAttribute.methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                cfgMethod.Invoke(result, new object[1] { r });
            }

            return result;
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
        private double TDF_01(double x, double y)
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
        private double TDF_02(double x, double y)
        {
            return TDF_01(x, y) * TDF_01(y, x);
        }

        /// <summary>
        /// Cosine of x multiplied by cosine of y, subtracting a specific constant.
        /// Creates a sparse array of circles!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //[TDF(4, 1.0, 1.0)]
        private double TDF_03(double x, double y)
        {
            return TDF_02(x, y) - 0.6204;
        }

        /// <summary>
        /// Cosine of x multiplied by cosine of y, subtracting a small value.
        /// Creates a sparse array of rounded squares; the other squares are connected yia their diagonals!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(4, 0.8, 1.2)]
        private double TDF_04(double x, double y)
        {
            return TDF_02(x, y) - 0.05;
        }

        /// <summary>
        /// Cosine of x multiplied by cosine of y, subtracting a specific constant.
        /// Creates an array of circles!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //[TDF(4, 0.8, 1.2)]
        private double TDF_05(double x, double y)
        {
            return Math.Abs(TDF_02(x, y)) - 0.6204;
        }

        /// <summary>
        /// Cosine of x multiplied by cosine of y, subtracting a small value.
        /// Creates a closely packed grid of (rounded) tiles.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(4, 0.8, 1.2)]
        [TDFConfigurator("TDF_06_Configurator")]
        private double TDF_06(double x, double y)
        {
            // The shift parameter should be calibrated so that there is a one-square wide path between the tiles.
            return Math.Abs(TDF_02(x, y)) - t1;
        }

        /// <summary>
        /// Calibrate the parameter t1.
        /// Goal: There should be a small gap (approx. one square wide) between the tiles.
        /// Some tiles may even touch.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="squaresPerUnit"></param>
        private void TDF_06_Configurator(Random r)
        {
            // Find the minimum function value at integer coordinates, when t1 is 0.
            t1 = 0;
            double xMin = 0, zMin = TDF_06(xMin, 0);
            for (int i = 1; i <= 6; i++)
            {
                double x = Math.Round(i / this.scale) * this.scale;
                double z = TDF_06(x, 0);
                if (z < zMin)
                {
                    zMin = z;
                    xMin = x;
                }
            }

            // Get the smaller of the two function values at half a square width distance.
            double delta = 0.5 * this.scale;
            zMin = Math.Min(TDF_06(xMin - delta, 0), TDF_06(xMin + delta, 0));

            // t1 can be determined directly from zMin:
            // If t1 > zMin, the function value will become negative.
            t1 = zMin * 1.001;
        }

        /// <summary>
        /// Paraboloid.
        /// Creates a circle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //[TDF()]
        private double TDF_11(double x, double y)
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
        private double TDF_12(double x, double y)
        {
            return (1.0 - (x * x - y * y));
        }

        /// <summary>
        /// Creates a distorted hyperbola.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //[TDF(2)]
        private double TDF_21(double x, double y)
        {
            return TDF_12(x, y) + 3.0 * TDF_02(x, y);
        }

        /// <summary>
        /// Creates concentric circles.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [TDF(0, 0.5, 2.0)]
        private double TDF_31(double r, double phi)
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
        private double TDF_32(double r, double phi)
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
        #region Member variables

        public readonly string methodName;

        #endregion

        #region Constructor

        public TDFConfiguratorAttribute(string configuratorMethodName)
        {
            this.methodName = configuratorMethodName;
        }

        #endregion
    }
}
