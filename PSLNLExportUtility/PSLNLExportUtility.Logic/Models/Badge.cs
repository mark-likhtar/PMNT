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
        public long Id { get; set; }

        [WMIProperty("ACTIVATE", typeof(string))]
        public string EffectiveDate { get; set; }

        [WMIProperty("STATUS", typeof(int))]
        public int Status { get; set; }

        [WMIProperty("DEACTIVATE", typeof(string))]
        public string TerminationDate { get; set; }

        [WMIProperty("EXTENDED_ID", typeof(string))]
        public string ExtendedId { get; set; }
    }
}
