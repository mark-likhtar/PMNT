using PSLNLExportUtility.Logic.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Models
{
    public class Cardholder
    {
        [WMIProperty("SSNO", typeof(string))]
        public string Id { get; set; }

        [WMIProperty("FIRSTNAME", typeof(string))]
        public string FirstName { get; set; }

        [WMIProperty("LASTNAME", typeof(string))]
        public string LastName { get; set; }

        [WMIProperty("FLDTEXT1057", typeof(string))]
        public string JobCode { get; set; }

        [WMIProperty("ADDR1", typeof(string))]
        public string JobCodeDescription { get; set; }

        //[WMIProperty("DEPT", typeof(int))]
        public int DepartmentId { get; set; }

        public string Location { get; set; }

        [WMIProperty("LOCATION", typeof(int))]
        public int SuperLocation { get; set; }

        [WMIProperty("COMPANYCODENUM", typeof(string))]
        public string CompanyDescription { get; set; }

        [WMIProperty("EMAIL", typeof(string))]
        public string Email { get; set; }

        [WMIProperty("MANAGERNAME", typeof(string))]
        public string ManagerName { get; set; }

        [WMIProperty("FLDTEXT1051", typeof(string))]
        public string ManagerEmail { get; set; }

        [WMIProperty("USERFIELD85", typeof(string))]
        public string BadgeNumber { get; set; }

        [WMIProperty("PERORGTEXT", typeof(string))]
        public string PerOrg { get; set; }
    }
}
