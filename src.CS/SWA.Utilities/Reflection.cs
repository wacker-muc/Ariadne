using System.Reflection;

namespace SWA.Utilities
{
    public static class Reflection
    {
        /// <summary>
        /// Gets the value of the private field named <paramref name="member"/>
        /// in the given <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="member">Member.</param>
        /// <typeparam name="TMember">The type of the member value.</typeparam>
        /// <exception cref="System.InvalidCastException">
        /// The selected field cannot be cast to the specified <typeparamref name="TMember"/>
        /// </exception>
        public static TMember GetPrivateField<TMember>(object obj, string member)
        {
            object result = null;

            FieldInfo info = obj.GetType().GetField(member,
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (info != null)
            {
                result = info.GetValue(obj);
            }

            return (TMember) result;
        }
    }
}
