using System.Reflection;
using System.Text;

namespace Tutorial.Infrastructure.Facades.Common.Helpers.PropertyFlatten;

public class PropertyFlattener
{
    private readonly List<PropertyFlatten> propertyFlattens = new();
    private readonly PropertyFlattenOptions options;

    protected PropertyFlattener(PropertyFlattenOptions? options = null)
    {
        this.options = options ?? new();
    }

    /// <summary>
    /// Gets or sets a function that defines custom criteria for accepting or rejecting types.
    /// The function should take a Type as input and return true if the type is acceptable; otherwise, false.
    /// This property allows users to provide a custom filtering mechanism for types in addition to the default criteria
    /// defined such as: String, Primitive Type, Enum, TimeSpan, DateTimeOffset, DateTime, Guid, byte[]
    /// </summary>
    public Func<Type, bool>? TypeAcceptFiller { get; set; }

    /// <summary>
    /// Flattens the properties of the given object.
    /// </summary>
    /// <param name="data">The object whose properties need to be flattened.</param>
    /// <returns>A collection of flattened properties.</returns>
    public IReadOnlyCollection<PropertyFlatten> FlattenData(object data)
    {
        FlattenPropertyInfo(string.Empty, data);
        return propertyFlattens;
    }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>The name of the property.</returns>
    public virtual string GetName(PropertyInfo propertyInfo) => propertyInfo.Name;

    /// <summary>
    /// Recursively flattens the properties of the parent object.
    /// </summary>
    /// <param name="path">The current path in the object hierarchy.</param>
    /// <param name="parent">The parent object whose properties are being flattened.</param>
    /// <param name="depth">The current depth in the object hierarchy.</param>
    private void FlattenPropertyInfo(string path, object parent, int depth = 0)
    {
        if (parent == null)
        {
            return;
        }

        Type parentType = parent.GetType();
        Type? enumerableType = parentType.GetElementTypeOfCollection();

        if (enumerableType != null)
        {
            parentType = enumerableType;
        }

        PropertyInfo[] propertyInfos = parentType.GetProperties();
        PropertyInfo[] acceptProperties = GetAcceptProperties(propertyInfos);
        PropertyInfo[] deeperProperties = propertyInfos.Except(acceptProperties).ToArray();
        propertyFlattens.AddRange(acceptProperties.SelectMany(x => CreateFlattenProperties(x, path, parent, enumerableType, depth)));
        depth++;

        if (options.MaxDepth.HasValue && depth > options.MaxDepth.Value)
        {
            return;
        }

        foreach (var property in deeperProperties)
        {
            Type? elementType = property.PropertyType.GetElementTypeOfCollection();
            if (elementType != null)
            {
                string enumerableTypeName = GetPath(path, property);

                if (IsAcceptType(elementType))
                {
                    propertyFlattens.AddRange(CreateFlattenProperties(property, path, parent, enumerableType, depth));
                }
                else
                {
                    FlattenPropertyInfo(enumerableTypeName, property.GetValue(parent)!, depth);
                }
            }
            else
            {
                FlattenPropertyInfo(GetPath(path, property), property.GetValue(parent)!, depth);
            }
        }
    }

    /// <summary>
    /// Gets the properties that should be accepted based on type criteria.
    /// </summary>
    /// <param name="propertyInfos">An array of PropertyInfo objects.</param>
    /// <returns>An array of PropertyInfo objects that meet the acceptance criteria.</returns>
    private PropertyInfo[] GetAcceptProperties(PropertyInfo[] propertyInfos)
        => propertyInfos.Where(propertyInfo => IsAcceptType(propertyInfo.PropertyType)).ToArray();

    /// <summary>
    /// Checks if a given type is acceptable based on specified criteria.
    /// </summary>
    /// <param name="type">The Type to be checked.</param>
    /// <returns>True if the type is acceptable; otherwise, false.</returns>
    private bool IsAcceptType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type == typeof(string) || type.IsPrimitive || type.IsEnum || type == typeof(DateTimeOffset) ||
               type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(Guid) ||
               type == typeof(byte[])
               || TypeAcceptFiller?.Invoke(type) == true;
    }

    /// <summary>
    /// Creates flattened properties based on the provided property information and parent object.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo of the property.</param>
    /// <param name="path">The current path in the object hierarchy.</param>
    /// <param name="parent">The parent object whose property is being flattened.</param>
    /// <param name="enumerableType">The element type of the property if it is enumerable; otherwise, null.</param>
    /// <param name="depth">The current depth in the object hierarchy.</param>
    /// <returns>An IEnumerable of PropertyFlatten objects representing the flattened properties.</returns>
    private IEnumerable<PropertyFlatten> CreateFlattenProperties(PropertyInfo propertyInfo, string path, object parent, Type? enumerableType, int depth)
    {
        string selectedTreeName = GetPath(path, propertyInfo);

        if (enumerableType != null)
        {
            dynamic enumerable = ((dynamic)parent).ToArray();
            int length = enumerable.Length;
            for (int i = 0; i < length; i++)
            {
                yield return new PropertyFlatten(selectedTreeName, propertyInfo.GetValue(enumerable[i]), propertyInfo, depth, i);
            }
        }
        else
        {
            yield return new PropertyFlatten(selectedTreeName, propertyInfo.GetValue(parent), propertyInfo, depth, null);
        }
    }

    /// <summary>
    /// Gets the path for a property based on the specified delimiter.
    /// </summary>
    /// <param name="path">The current path.</param>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>The path with the specified delimiter.</returns>
    private string GetPath(string path, PropertyInfo propertyInfo)
    {
        string propertyName = GetName(propertyInfo);
        StringBuilder pathBuilder = new();
        if (string.IsNullOrEmpty(path))
        {
            return pathBuilder.Append(propertyName).ToString();
        }
        else
        {
            pathBuilder.Append(path);
        }

        switch (options.Delimiter)
        {
            case PropertyFlattenDelimiter.None:
                break;

            case PropertyFlattenDelimiter.Dots:
                pathBuilder.Append('.').Append(propertyName);
                break;

            case PropertyFlattenDelimiter.SquareBrackets:
                pathBuilder.Append('[').Append(propertyName).Append(']');
                break;

            case PropertyFlattenDelimiter.Parentheses:
                pathBuilder.Append('(').Append(propertyName).Append(')');
                break;

            case PropertyFlattenDelimiter.Hyphen:
                pathBuilder.Append('-').Append(propertyName);
                break;

            case PropertyFlattenDelimiter.Colons:
                pathBuilder.Append(':').Append(propertyName);
                break;

            case PropertyFlattenDelimiter.Commas:
                pathBuilder.Append(',').Append(propertyName);
                break;

            case PropertyFlattenDelimiter.Slash:
                pathBuilder.Append('/').Append(propertyName);
                break;

            case PropertyFlattenDelimiter.Underscore:
                pathBuilder.Append('_').Append(propertyName);
                break;
        }

        return pathBuilder.ToString();
    }
}