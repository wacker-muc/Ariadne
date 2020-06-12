using System.Reflection;

namespace SWA.Utilities
{
    public static class Reflection
    {
        public static TMember GetPrivateField<TObject, TMember>(TObject obj, string member) where TMember : class
        {
            object result = null;

            FieldInfo info = typeof(TObject).GetField(member, BindingFlags.NonPublic | BindingFlags.Instance);
            if (info != null)
            {
                result = info.GetValue(obj);
            }

            return result as TMember;
        }
    }
}
