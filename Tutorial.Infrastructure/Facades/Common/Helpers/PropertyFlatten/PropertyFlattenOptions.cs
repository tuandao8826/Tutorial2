namespace Tutorial.Infrastructure.Facades.Common.Helpers.PropertyFlatten;

/// <summary>
/// Options for flattening properties with specific delimiters.
/// </summary>
public class PropertyFlattenOptions
{
    /// <summary>
    /// Gets or sets the delimiter used for property flattening.
    /// </summary>
    public PropertyFlattenDelimiter Delimiter { get; set; } = PropertyFlattenDelimiter.Dots;

    /// <summary>
    /// Gets or sets the maximum depth for property flattening.
    /// If set, the flattening process will only traverse properties up to the specified depth.
    /// A depth of null or not set indicates no depth limit.
    /// </summary>
    public int? MaxDepth { get; set; }
}

/// <summary>
/// Enumeration representing different delimiters for property flattening.
/// </summary>
public enum PropertyFlattenDelimiter
{
    /// <summary>
    /// Represents don't have the delimiter.
    /// </summary>
    None = 0,

    /// <summary>
    /// Represents the dot (.) as the delimiter.
    /// </summary>
    Dots = 1,

    /// <summary>
    /// Represents square brackets [] as the delimiter.
    /// </summary>
    SquareBrackets = 2,

    /// <summary>
    /// Represents underscore (_) as the delimiter.
    /// </summary>
    Underscore = 3,

    /// <summary>
    /// Represents parentheses () as the delimiter.
    /// </summary>
    Parentheses = 4,

    /// <summary>
    /// Represents hyphen (-) as the delimiter.
    /// </summary>
    Hyphen = 5,

    /// <summary>
    /// Represents colons (:) as the delimiter.
    /// </summary>
    Colons = 6,

    /// <summary>
    /// Represents commas (,) as the delimiter.
    /// </summary>
    Commas = 7,

    /// <summary>
    /// Represents slash (/) as the delimiter.
    /// </summary>
    Slash = 8,
}