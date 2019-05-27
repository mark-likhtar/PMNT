using PSLNLExportUtility.Logic.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Models
{
    public class Badge
    {
        [WMIProperty("Activate", typeof(string))]
        public string EffectiveDate { get; set; }

        [WMIProperty("Status", typeof(int))]
        public int Status { get; set; }

        [WMIProperty("Deactivate", typeof(string))]
        public string TerminationDate { get; set; }
    }
}
