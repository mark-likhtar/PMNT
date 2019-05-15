using PSLNLExportUtility.Insfrastructure.Settings;
using PSLNLExportUtility.Logic.Services.DataImport;
using PSLNLExportUtility.Logic.Services.DataImport.Models;
using PSLNLExportUtility.Infrastructure.Logging;
using System;
using PSLNLExportUtility.Logic.Services.ExcelDataReader;
using PSLNLExportUtility.Logic.Services.ExcelDataReader.Models;
using System.Collections.Generic;
using PSLNLExportUtility.Logic.Services.LebelWMI;
using System.Linq;

namespace PSLNLExportUtility
{
    public class Program
    {
        public static readonly Logger _logger = new Logger();

        public static void Main(string[] args)
        {
            _logger.Info("Started working.");

            DataPipelineService dataPipelineService = null;

            try
            {
                dataPipelineService = CreateDataPipelineService();
                var excelDataReaderService = CreateExcelDataReaderService();

                dataPipelineService.PrepareForProcessing();
                if (dataPipelineService.CurrentFileLocation == null)
                {
                    return;
                }

                IEnumerable<Employee> employees =
                    excelDataReaderService.ReadData(dataPipelineService.CurrentFileLocation);

                var cardholderService = CreateCardholderService();

                foreach (var employee in employees)
                {
                    cardholderService.UpsertEmployee(employee);
                }

                dataPipelineService.SaveDataToProcessed();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                dataPipelineService?.SaveDataToError(_logger.GetLogs());
            }
            finally
            {
                _logger.Info("Finished working.");
            }
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

        private static ExcelDataReaderService CreateExcelDataReaderService()
        {
            return new ExcelDataReaderService();
        }
        private static LenelService CreateCardholderService()
        {
            return new LenelService();
        }
    }
}
