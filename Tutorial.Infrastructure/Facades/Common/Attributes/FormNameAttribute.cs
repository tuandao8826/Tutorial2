using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Facades.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormNameAttribute : Attribute
    {
        public string Name { get; }

        public FormNameAttribute(string name)
        {
            Name = name;
        }
    }
}
