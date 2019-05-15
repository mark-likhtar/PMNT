using ORMi;
using System;

namespace PSLNLExportUtility.Logic.Services.LebelWMI.Model
{
    [WMIClass("Lnl_Cardholder")]
    public class Cardholder : WMIInstance
    {
        [WMIProperty("ID")]
        public string Id { get; set; }

        [WMIProperty("FIRSTNAME")]
        public string FirstName { get; set; }

        [WMIProperty("MIDNAME")]
        public string MiddleName { get; set; }

        [WMIProperty("LASTNAME")]
        public string LastName { get; set; }

        [WMIProperty("LASTCHANGED")]
        public DateTime LastChanged { get; set; }

        [WMIProperty("SSNO")]
        public string SSNO { get; set; }
    }
}
