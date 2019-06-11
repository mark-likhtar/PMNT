using PSLNLExportUtility.Logic.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Models
{
    public class Department
    {
        [WMIProperty("ID", typeof(int))]
        public int Id { get; set; }

        [WMIProperty("Name", typeof(string))]
        public string Name { get; set; }
    }
}
