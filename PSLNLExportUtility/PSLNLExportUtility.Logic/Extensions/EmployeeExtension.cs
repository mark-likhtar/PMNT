using PSLNLExportUtility.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Extensions
{
    public static class EmployeeExtension
    {
        public static Cardholder ToCardholder(this Employee employee)
        {
            return new Cardholder {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                JobCode = employee.JobCode,
                JobCodeDescription = employee.JobCodeDescription,
                DepartmentId = employee.DepartmentId,
                Location = employee.Location,
                CompanyDescription = employee.CompanyDescription,
                DepartmentDescription = employee.DepartmentDescription,
                Email = employee.Email,
                ManagerName = employee.ManagerName,
                ManagerEmail = employee.ManagerEmail,
                BadgeNumber = $"{employee.BadgeNumber}",
                PerOrg = employee.PerOrg,
                Status = employee.Status == "A" ? 1 : 5,
            };
        }

        public static Badge ToBadge(this Employee employee)
        {
            string TerminationDate = employee.Status != "A"
                ? ManagementDateTimeConverter.ToDmtfDateTime(DateTime.Now)
                : (employee.TerminationDate != default
                    ? ManagementDateTimeConverter.ToDmtfDateTime(employee.TerminationDate) 
                    : null);

            return new Badge
            {
                Status = employee.Status == "A" ? 1 : 5,
                TerminationDate = TerminationDate
            };
        }
    }
}
