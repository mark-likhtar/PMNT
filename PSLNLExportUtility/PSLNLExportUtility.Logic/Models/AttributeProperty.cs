using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Models
{
    public class AttributeProperty<A>
    {
        public A Attribute { get; set; }
        public PropertyInfo Property { get; set; }
    }
}
