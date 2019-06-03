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

                IEnumerable<Employee> employees =
                    csvDataReaderService.ReadData(
                        dataPipelineService.CurrentFileLocation,
                        Settings.WithKronosNumber
                    );

                var cardholderService = CreateLenelService<Cardholder>("Lnl_Cardholder");
                var badgeService = CreateLenelService<Badge>("Lnl_Badge");
                var departmentService = CreateLenelService<Department>("Lnl_Department");
                var locationService = CreateLenelService<Location>("Lnl_Location");

                var cardholders = GetCardholders(cardholderService);
                var badges = GetBadges(badgeService);
                var departments = GetDepartments(departmentService);
                var locations = GetLocations(locationService);

                foreach (var employee in employees)
                {
                    var cardholder = employee.ToCardholder();
                    var department = employee.ToDepartment();
                    var badge = employee.ToBadge();

                    ManagementBaseObject existedCardholder = null;
                    ManagementBaseObject existedDepartment = null;
                    ManagementBaseObject existedBadge = null;
                    ManagementBaseObject existedLocation = null;

                    if (!string.IsNullOrEmpty(cardholder.Location))
                    {
                        locations.TryGetValue(cardholder.Location, out existedLocation);
                    }

                    if (!string.IsNullOrEmpty(cardholder.Id))
                    {
                        cardholders.TryGetValue(cardholder.Id, out existedCardholder);
                    }

                    if (department.Id != default)
                    {
                        departments.TryGetValue(department.Id, out existedDepartment);
                    }

                    if (existedDepartment == null)
                    {
                        cardholder.DepartmentId = default;
                        _report.Add($"{cardholder.Id}: failed - Department not found");
                    }

                    if (existedLocation == null)
                    {
                        var locationAdded = locationService.AddInstance(new Location { Name = cardholder.Location });
                        if (locationAdded)
                        {
                            locations = GetLocations(locationService);
                            existedLocation = locations[cardholder.Location];
                        }
                        else
                        {
                            _report.Add($"{cardholder.Id}: failed - Can't add location {cardholder.Location}");
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

        private static Dictionary<int, ManagementBaseObject> GetDepartments(LenelService<Department> departmentService)
        {
            var departments = new Dictionary<int, ManagementBaseObject>();
            foreach (var department in departmentService.GetInstances())
            {
                var key = department["ID"] as int?;

                if (key.HasValue)
                {
                    departments.Add(key.Value, department);
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
