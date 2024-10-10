using System.Collections;

namespace Tutorial.Infrastructure.Facades.Common.Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets the element type of a collection type.
        /// </summary>
        /// <param name="type">The collection type.</param>
        /// <returns>The element type if the type is a collection; otherwise, null.</returns>
        public static Type? GetElementTypeOfCollection(this Type type)
        {
            if (IsCollection(type))
            {
                return type.IsArray ? type.GetElementType() : type.GetGenericArguments().FirstOrDefault();
            }

            return null;
        }

        public static bool IsCollection(this Type type) => type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
    }
}