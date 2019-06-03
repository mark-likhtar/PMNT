using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Services.Helpers
{
    public sealed class FileNameComparer : IComparer<string>
    {

        private DateTime GetTimeFromString(string date)
        {
            var regex = new Regex(@"Lenel_(?<year>[0-9]{4})_(?<mounth>[0-9]{2})_(?<day>[0-9]{2}).csv");

            var groups = regex.Match(date).Groups;

            var year = groups["year"].Success ? groups["year"].Value : "1970";
            var mounth = groups["mounth"].Success ? groups["mounth"].Value : "01";
            var day = groups["day"].Success ? groups["day"].Value : "01";

            return DateTime.ParseExact($"{year} {mounth} {day}", "yyyy MM dd", null);
        }

        public int Compare(string a, string b)
        {
            var timeA = GetTimeFromString(a);
            var timeB = GetTimeFromString(b);

            return timeA > timeB ? 1 : (timeA < timeB ? -1 : 0);
        }
    }
}
