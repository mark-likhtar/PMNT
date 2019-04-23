﻿using System;

namespace PSLNLExportUtility.Logic.Services.ExcelDataReader.Models
{
    public class Employee
    {
        public string Id { get; set; }

        public DateTime EffectiveDate { get; set; }

        public char Status { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string JobCode { get; set; }

        public string JobCodeDescription { get; set; }

        public string DepartmentId { get; set; }

        public string DepartmentDescription { get; set; }

        public string Location { get; set; }

        public string LocationDescription { get; set; }

        public string Company { get; set; }

        public string CompanyDescription { get; set; }

        public string Email { get; set; }

        public string ManagerName { get; set; }

        public string ManagerEmail { get; set; }

        public string SupperLocation { get; set; }

        public string TerminationDate { get; set; }

        public string BadgeNumber { get; set; }

        public string PerOrg { get; set; }
    }
}
