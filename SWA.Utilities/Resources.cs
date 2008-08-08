using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SWA.Utilities
{
    public class Resources
    {
        /// <summary>
        /// Returns a list of the given type's Properties that return a Bitmap type.
        /// Use this on a type derived from a Resources file.
        /// </summary>
        /// <param name="resourcesType"></param>
        /// <returns></returns>
        public static List<MethodInfo> BitmapProperties(Type resourcesType)
        {
            List<MethodInfo> result = new List<System.Reflection.MethodInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            foreach (PropertyInfo info in resourcesType.GetProperties(flags))
            {
                Type propertyType = info.PropertyType;
                Type bitmapType = typeof(System.Drawing.Bitmap);
                if (bitmapType.IsAssignableFrom(propertyType))
                {
                    result.Add(info.GetGetMethod(true));
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a Bitmap using one of the methods in the given list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static System.Drawing.Bitmap CreateBitmap(List<MethodInfo> list, Random r)
        {
            MethodInfo method = list[r.Next(list.Count)];
            System.Drawing.Bitmap result = (System.Drawing.Bitmap)method.Invoke(null, null);

            return result;
        }
    }
}
