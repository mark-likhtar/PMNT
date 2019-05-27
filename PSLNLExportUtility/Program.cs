using PSLNLExportUtility.Insfrastructure.Settings;
using PSLNLExportUtility.Logic.Services.DataImport;
using PSLNLExportUtility.Logic.Services.DataImport.Models;
using PSLNLExportUtility.Infrastructure.Logging;
using System;
using PSLNLExportUtility.Logic.Services.CsvDataReader;
using System.Collections.Generic;
using PSLNLExportUtility.Logic.Models;

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
                var csvDataReaderService = CreateCsvDataReaderService();

                dataPipelineService.PrepareForProcessing();
                if (dataPipelineService.CurrentFileLocation == null)
                {
                    return;
                }

                IEnumerable<Employee> employees =
                    csvDataReaderService.ReadData(dataPipelineService.CurrentFileLocation);




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

        private static CsvDataReaderService CreateCsvDataReaderService()
        {
            return new CsvDataReaderService();
        }
    }
}
