using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Facades.Common.Helpers
{
    public static class KeyValueHelper
    {
        /// <summary>
        /// Returns a key-value-pairs representation of the object.
        /// For strings, URL query string format assumed and pairs are parsed from that.
        /// For objects that already implement IEnumerable&lt;KeyValuePair&gt;, the object itself is simply returned.
        /// For all other objects, all publicly readable properties are extracted and returned as pairs.
        /// </summary>
        /// <param name="obj">The object to parse into key-value pairs</param>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null" />.</exception>

        // - Dùng để chuyển đổi một đối tượng thành danh sách các cặp key-value (khóa-giá trị),
        // có thể được sử dụng để xây dựng header hoặc query parameter cho các yêu cầu HTTP.
        public static IEnumerable<(string Key, object? Value)> ParseKeyValuePairs(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            // sửa
            return obj switch
            {
                string s => StringToKeyValue(s),
                IEnumerable e => CollectionToKeyPair(e),
                _ => ObjectToKeyValue(obj)
            };
        }

        private static IEnumerable<(string Key, object? Value)> StringToKeyValue(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return Enumerable.Empty<(string, object?)>();
            }

            return
                from p in s.Split('&')
                let pair = SplitOnFirstOccurence(p, "=")
                let name = pair[0]
                let value = pair.Length == 1 ? null : pair[1]
                select (name, (object)value);
        }

        /// <summary>
        /// Splits at the first occurrence of the given separator.
        /// </summary>
        /// <param name="s">The string to split.</param>
        /// <param name="separator">The separator to split on.</param>
        /// <returns>Array of at most 2 strings. (1 if separator is not found.)</returns>
        private static string[] SplitOnFirstOccurence(string s, string separator)
        {
            // Needed because full PCL profile doesn't support Split(char[], int) (#119)
            if (string.IsNullOrEmpty(s))
            {
                return new[] { s };
            }

            var i = s.IndexOf(separator);
            return i == -1 ? new[] { s } : new[] { s[..i], s[(i + separator.Length)..] };
        }

        private static IEnumerable<(string Name, object? Value)> ObjectToKeyValue(object obj) =>
            from prop in obj.GetType().GetProperties()
            let getter = prop.GetGetMethod(false)
            where getter != null
            let val = getter.Invoke(obj, null)
            select (prop.Name, GetDeclaredTypeValue(val, prop.PropertyType));

        private static object? GetDeclaredTypeValue(object value, Type declaredType)
        {
            if (value == null || value.GetType() == declaredType)
            {
                return value;
            }

            declaredType = Nullable.GetUnderlyingType(declaredType) ?? declaredType;

            if (value is IEnumerable col
                && declaredType.IsGenericType
                && declaredType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && !col.GetType().GetInterfaces().Contains(declaredType)
                && declaredType.IsInstanceOfType(col))
            {
                var elementType = declaredType.GetGenericArguments()[0];
                return col.Cast<object>().Select(element => Convert.ChangeType(element, elementType));
            }

            return value;
        }

        private static IEnumerable<(string Key, object? Value)> CollectionToKeyPair(IEnumerable col)
        {
            bool TryGetProp(object obj, string name, out object? value)
            {
                var prop = obj.GetType().GetProperty(name);
                var field = obj.GetType().GetField(name);

                if (prop != null)
                {
                    value = prop.GetValue(obj, null);
                    return true;
                }

                if (field != null)
                {
                    value = field.GetValue(obj);
                    return true;
                }

                value = null;
                return false;
            }

            bool IsTuple2(object item, out object? name, out object? val)
            {
                name = null;
                val = null;
                return
                    OrdinalContains(item.GetType().Name, "Tuple") &&
                    TryGetProp(item, "Item1", out name) &&
                    TryGetProp(item, "Item2", out val) &&
                    !TryGetProp(item, "Item3", out _);
            }

            bool LooksLikeKV(object item, out object? name, out object? val)
            {
                name = null;
                val = null;
                return
                    (TryGetProp(item, "Key", out name) || TryGetProp(item, "key", out name) || TryGetProp(item, "Name", out name) || TryGetProp(item, "name", out name)) &&
                    (TryGetProp(item, "Value", out val) || TryGetProp(item, "value", out val));
            }

            foreach (var item in col)
            {
                if (item == null)
                {
                    continue;
                }

                if (!IsTuple2(item, out var name, out var val) && !LooksLikeKV(item, out name, out val))
                {
                    yield return (ToInvariantString(name) ?? throw new ArgumentNullException(nameof(col)), null);
                }
                else if (name != null)
                {
                    yield return (ToInvariantString(name) ?? throw new ArgumentNullException(nameof(col)), val);
                }
            }
        }

        private static bool OrdinalContains(string s, string value, bool ignoreCase = false)
            => s?.IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;

        /// <summary>
        /// Returns a string that represents the current object, using CultureInfo.InvariantCulture where possible.
        /// Dates are represented in IS0 8601.
        /// </summary>
        private static string? ToInvariantString(object? obj)
        {
            return obj switch
            {
                null => null,
                DateTime dt => dt.ToString("o", CultureInfo.InvariantCulture),
                DateTimeOffset dto => dto.ToString("o", CultureInfo.InvariantCulture),
                IConvertible c => c.ToString(CultureInfo.InvariantCulture),
                IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => obj.ToString()
            };
        }
    }
}
