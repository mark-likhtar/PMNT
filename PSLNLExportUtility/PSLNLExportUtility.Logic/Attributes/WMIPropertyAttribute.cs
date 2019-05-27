using PSLNLExportUtility.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Attributes
{
    public class WMIPropertyAttribute : Attribute
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public WMIPropertyAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public static AttributeProperty<WMIPropertyAttribute> GetAttributeField<T>(T instance, string name)
        {
            var props = instance
                .GetType()
                .GetProperties();

            foreach (var prop in props)
            {
                var attribute = prop.GetCustomAttributes(false)
                    .Where(attr => attr is WMIPropertyAttribute)
                    .FirstOrDefault() as WMIPropertyAttribute;

                if (attribute.Name == name)
                {
                    return new AttributeProperty<WMIPropertyAttribute>
                    {
                        Attribute = attribute,
                        Property = prop
                    };
                }
            }

            return null;
        }
    }
}
