using System.Reflection;

namespace Tutorial.Infrastructure.Facades.Common.Helpers.PropertyFlatten
{
    public record PropertyFlatten(string Path, object? Value, PropertyInfo PropertyInfo, int Depth, int? Index);
}