using PSLNLExportUtility.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Services.LenelService
{
    public static class CardholderService
    {
        private static List<PersonBadge> persons = null;

        public static IEnumerable<PersonBadge> GetCardholderBadgesBySSNO(string ssno)
        {
            var person = GetPerson(ssno);

            if (person == null)
            {
                return new List<PersonBadge>();
            }

            var id = (int)person["ID"];
            return GetBadges(id);
        }

        private static IEnumerable<PersonBadge> GetBadges(int personId)
        {
            if (persons == null)
            {
                var searcher =
                    new ManagementObjectSearcher("root\\OnGuard",
                    "SELECT * FROM Lnl_BadgeOwner");

                persons = new List<PersonBadge>();

                foreach (var queryObj in searcher.Get())
                {
                    var badge = (string)queryObj["Badge"];
                    var badgeId = int.Parse(badge.Split('"')[1]);

                    var pers = (string)queryObj["Person"];
                    var persId = int.Parse(pers.Split('"')[1]);

                    persons.Add(new PersonBadge
                    {
                        BadgeId = badgeId,
                        PersonId = persId
                    });

                }
            }

            return persons.Where(x => x.PersonId == personId);
        }

        private static ManagementBaseObject GetPerson(string ssno)
        {
            var searcher = new ManagementObjectSearcher("root\\OnGuard", $"SELECT * FROM Lnl_Person WHERE SSNO = '{ssno}'");

            foreach (var item in searcher.Get())
            {
                return item;
            }

            return null;
        }
    }
}
