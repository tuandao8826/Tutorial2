using System.Reflection;
using Tutorial.Infrastructure.Facades.Common.Attributes;
using Tutorial.Infrastructure.Facades.Common.Helpers.PropertyFlatten;

namespace Tutorial.Infrastructure.Facades.Common.HttpClients
{
    public class HttpPropertyFlattener : PropertyFlattener
    {
        public HttpPropertyFlattener(PropertyFlattenOptions? options = null)
            : base(options)
        {
        }

        public override string GetName(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<FormNameAttribute>()?.Name ?? base.GetName(propertyInfo);
        }
    }
}