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
                Email = employee.Email,
                ManagerName = employee.ManagerName,
                ManagerEmail = employee.ManagerEmail,
                BadgeNumber = $"{employee.BadgeNumber}",
                PerOrg = employee.PerOrg
            };
        }

        public static Badge ToBadge(this Employee employee)
        {
            return new Badge
            {
                EffectiveDate = ManagementDateTimeConverter.ToDmtfDateTime(employee.EffectiveDate),
                Status = employee.Status == "A" ? 1 : 5,
                TerminationDate = ManagementDateTimeConverter.ToDmtfDateTime(
                    employee.TerminationDate != default 
                        ? employee.TerminationDate 
                        : employee.EffectiveDate.AddYears(10)
                )
            };
        }
        public static Department ToDepartment(this Employee employee)
        {
            return new Department
            {
                Id = employee.DepartmentId
            };
        }
    }
}
