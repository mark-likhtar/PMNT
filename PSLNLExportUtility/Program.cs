using PSLNLExportUtility.Insfrastructure.Settings;
using PSLNLExportUtility.Logic.Services.DataImport;
using PSLNLExportUtility.Logic.Services.DataImport.Models;
using PSLNLExportUtility.Infrastructure.Logging;
using System;
using System.Linq;
using PSLNLExportUtility.Logic.Services.CsvDataReader;
using System.Collections.Generic;
using PSLNLExportUtility.Logic.Models;
using PSLNLExportUtility.Logic.Services.LenelService;
using PSLNLExportUtility.Logic.Extensions;
using System.Management;
using PSLNLExportUtility.Logic.Services.EmailService;
using System.IO;

namespace PSLNLExportUtility
{
    public class Program
    {
        public static readonly Logger _logger = new Logger();
        public static readonly List<string> _report = new List<string>();

        public static void Main(string[] args)
        {
            _logger.Info("Started working.");
            bool hasFile;
            do
            {
                hasFile = ProcessFile();
            } while (hasFile);


            //CreateEmailService()
            //    .SendReport(
            //        Settings.EmailAccount.ReportEmail,
            //        Settings.EmailAccount.ReportSubject,
            //        _report
            //    );
            SaveOutput();

            _logger.Info("Finished working.");
        }

        private static void SaveOutput()
        {
            var folder = Settings.DirectoriesPaths.Output;

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (StreamWriter file = new StreamWriter($"{folder}\\Output_{DateTime.Now.ToString("yyyy_MM_dd")}.log", true))
            {
                foreach (var item in _report)
                {
                    file.WriteLine(item);
                }
            }
        }


        private static bool ProcessFile()
        {
            DataPipelineService dataPipelineService = null;

            try
            {
                dataPipelineService = CreateDataPipelineService();
                var csvDataReaderService = CreateCsvDataReaderService();

                dataPipelineService.PrepareForProcessing();
                if (dataPipelineService.CurrentFileLocation == null)
                {
                    return false;
                }

                List<Employee> employees =
                    csvDataReaderService.ReadData(
                        dataPipelineService.CurrentFileLocation,
                        Settings.WithKronosNumber
                    ).ToList();

                var cardholderService = CreateLenelService<Cardholder>("Lnl_Cardholder");
                var badgeService = CreateLenelService<Badge>("Lnl_Badge");
                var departmentService = CreateLenelService<Department>("Lnl_Department");
                var locationService = CreateLenelService<Location>("Lnl_Location");

                var cardholders = GetCardholders(cardholderService);
                var badges = GetBadges(badgeService);
                var departments = GetDepartments(departmentService);
                var locations = GetLocations(locationService);

                for (var i = 0; i < employees.Count(); i++)
                {
                    var employee = employees[i];
                    var cardholder = employee.ToCardholder();
                    var badge = employee.ToBadge();

                    ManagementBaseObject existedCardholder = null;
                    ManagementBaseObject existedDepartment = null;
                    ManagementBaseObject existedBadge = null;
                    ManagementBaseObject existedLocation = null;

                    if (!string.IsNullOrEmpty(cardholder.Location))
                    {
                        locations.TryGetValue(cardholder.Location, out existedLocation);
                    }

                    if (!string.IsNullOrEmpty(cardholder.DepartmentDescription))
                    {
                        departments.TryGetValue(cardholder.DepartmentDescription, out existedDepartment);
                    }

                    if (!string.IsNullOrEmpty(cardholder.Id))
                    {
                        cardholders.TryGetValue(cardholder.Id, out existedCardholder);
                    }

                    if (existedDepartment == null || existedLocation == null)
                    {
                        var departmentAdded = false;
                        var locationAdded = false;

                        if (existedDepartment == null)
                        {
                            departmentAdded = departmentService.AddInstance(new Department { Name = cardholder.DepartmentDescription });
                        }
                        if (existedLocation == null)
                        {
                            locationAdded = locationService.AddInstance(new Location { Name = cardholder.Location });
                        }

                        if (departmentAdded || locationAdded)
                        {
                            cardholderService = CreateLenelService<Cardholder>("Lnl_Cardholder");
                            badgeService = CreateLenelService<Badge>("Lnl_Badge");
                            departmentService = CreateLenelService<Department>("Lnl_Department");
                            locationService = CreateLenelService<Location>("Lnl_Location");

                            cardholders = GetCardholders(cardholderService);
                            badges = GetBadges(badgeService);
                            departments = GetDepartments(departmentService);
                            locations = GetLocations(locationService);

                            i--;
                            continue;
                        }
                        else
                        {
                            _report.Add($"{cardholder.Id}: failed - Can't add {(departmentAdded ? "department" : "location")} {cardholder.Location}");
                        }
                    }

                    if (existedLocation != null)
                    {
                        var id = existedLocation["ID"] as int?;
                        if (id.HasValue)
                        {
                            cardholder.SuperLocation = id.Value;
                        }
                    }

                    if (existedDepartment != null)
                    {
                        var id = existedDepartment["ID"] as int?;
                        if (id.HasValue)
                        {
                            cardholder.DeptId = id.Value;
                        }
                    }

                    var isSuccess = existedCardholder == null
                        ? cardholderService.AddInstance(cardholder)
                        : cardholderService.UpdateInstance(existedCardholder, cardholder);

                    if (!isSuccess)
                    {
                        _report.Add($"{cardholder.Id}: failed - Generic failure");
                        continue;
                    }

                    var personBadges = CardholderService.GetCardholderBadgesBySSNO(cardholder.Id).ToList();
                    foreach (var personBadge in personBadges)
                    {
                        if (badges.TryGetValue(personBadge.BadgeId, out existedBadge))
                        {
                            badge.ExtendedId = $"0x{personBadge.BadgeId}";
                            isSuccess = badgeService.UpdateInstance(existedBadge, badge);
                        }
                    }

                    if (isSuccess)
                    {
                        _report.Add($"{cardholder.Id}: proceed - Success");
                    }
                    else
                    {
                        _report.Add($"{cardholder.Id}: failed - Update badge failure");
                    }
                }

                dataPipelineService.SaveDataToProcessed();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                dataPipelineService?.SaveDataToError(_logger.GetLogs());
            }

            return true;
        }

        private static Dictionary<string, ManagementBaseObject> GetCardholders(LenelService<Cardholder> cardholderService)
        {
            var cardholders = new Dictionary<string, ManagementBaseObject>();
            foreach (var card in cardholderService.GetInstances())
            {
                var key = card["SSNO"] as string;
                if (!string.IsNullOrEmpty(key))
                {
                    cardholders.Add(key, card);
                }
            }

            return cardholders;
        }

        private static Dictionary<long, ManagementBaseObject> GetBadges(LenelService<Badge> badgeService)
        {
            var badges = new Dictionary<long, ManagementBaseObject>();
            foreach (var badge in badgeService.GetInstances())
            {
                var key = badge["ID"] as long?;

                if (key.HasValue)
                {
                    badges.Add(key.Value, badge);
                }
            }

            return badges;
        }
        private static Dictionary<string, ManagementBaseObject> GetLocations(LenelService<Location> locationService)
        {
            var locations = new Dictionary<string, ManagementBaseObject>();
            foreach (var location in locationService.GetInstances())
            {
                var key = location["Name"] as string;

                if (key != null)
                {
                    locations.Add(key, location);
                }
            }

            return locations;
        }

        private static Dictionary<string, ManagementBaseObject> GetDepartments(LenelService<Department> departmentService)
        {
            var departments = new Dictionary<string, ManagementBaseObject>();
            foreach (var department in departmentService.GetInstances())
            {
                var key = department["Name"] as string;

                if (key != null && departments.TryGetValue(key, out var value))
                {
                    continue;
                }

                if (key != null)
                {
                    departments.Add(key, department);
                }
            }

            return departments;
        }

        private static DataPipelineService CreateDataPipelineService()
        {
            return new DataPipelineService(_logger, new DataPipelineServiceSettings
            {
                QueueDirectoryPath = Settings.DirectoriesPaths.Queue,
                ProcessedDirectoryPath = Settings.DirectoriesPaths.Processed,
                ErrorDirectoryPath = Settings.DirectoriesPaths.Error,
            }, pipelineEnabled: Settings.PipelineEnabled);
        }

        private static EmailService CreateEmailService()
        {
            return new EmailService();
        }

        private static CsvDataReaderService CreateCsvDataReaderService()
        {
            return new CsvDataReaderService();
        }

        private static LenelService<T> CreateLenelService<T>(string className)
        {
            return new LenelService<T>(className);
        }
    }
}
