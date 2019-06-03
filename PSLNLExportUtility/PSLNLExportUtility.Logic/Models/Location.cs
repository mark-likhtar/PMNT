using PSLNLExportUtility.Logic.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Models
{
    public class Location
    {
        public int Id { get; set; }

        [WMIProperty("Name", typeof(string))]
        public string Name { get; set; }
    }
}
