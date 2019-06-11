using PSLNLExportUtility.Logic.Services.LenelService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLReportUtiity
{
    class Program
    {
        static void Main(string[] args)
        {
            var searcher =
                new ManagementObjectSearcher("root\\OnGuard",
                "SELECT * FROM Lnl_Cardholder");
            var folder = ConfigurationManager.AppSettings["OutputFolder"];

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (StreamWriter file = new StreamWriter($"{folder}\\Lenel_Export_{DateTime.Now.ToString("yyyy_MM_dd")}.csv", true))
            {
                foreach (var queryObj in searcher.Get())
                {
                    var output = GetCardholderRecord(queryObj);
                    if (output != null)
                    {
                        output.ForEach(x => file.WriteLine(x));
                    }
                }
            }            
        }

        private static string DateToOutputDate(string date)
        {
            return $"${date.Substring(0, 4)}-${date.Substring(4, 4)}";
        }

        static List<string> GetCardholderRecord(ManagementBaseObject obj)
        {
            var items = new List<string>();

            if (string.IsNullOrEmpty(obj["SSNO"] as string))
            {
                return null;
            }

            var today = DateTime.Now.ToString("yyyyMMdd");

            if (!string.IsNullOrEmpty(obj["USERFIELD85"] as string))
            {
                var activate = obj["FLDDATE1059"] as string;
                if (!string.IsNullOrEmpty(activate))
                {
                    activate = activate.Substring(0, 8);
                    if (activate == today)
                    {
                        items.Add($"\"{obj["SSNO"]}\",\"GB\",\"{DateToOutputDate(activate)}\",\"{obj["USERFIELD85"]}\"");
                    }

                }
            }

            var badges = CardholderService.GetCardholderBadgesBySSNO(obj["SSNO"] as string);
            foreach (var badge in badges)
            {
                var searcher = new ManagementObjectSearcher("root\\OnGuard", $"SELECT * FROM Lnl_Badge WHERE ID = {badge.BadgeId}");
                foreach (var badgeObj in searcher.Get())
                {
                    var activate = badgeObj["ACTIVATE"] as string;
                    if (string.IsNullOrEmpty(activate))
                    {
                        break;
                    }

                    activate = activate.Substring(0, 8);
                    if (activate == today)
                    {
                        items.Add($"\"{obj["SSNO"]}\",\"BA\",\"{DateToOutputDate(activate)}\",\"{obj["ID"]}\"");
                    }
                }
            }

            return items;
        }
    }
}
