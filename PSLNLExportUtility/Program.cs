using PSLNLExportUtility.Insfrastructure.Settings;
using PSLNLExportUtility.Logic.Services.DataImport;
using PSLNLExportUtility.Logic.Services.DataImport.Models;
using PSLNLExportUtility.Infrastructure.Logging;
using System;

namespace PSLNLExportUtility
{
    public class Program
    {
        public static readonly Logger _logger = new Logger();

        public static void Main(string[] args)
        {
            DataPipelineService dataPipelineService = null;

            try
            {
                dataPipelineService = new DataPipelineService(_logger, new DataPipelineServiceSettings
                {
                    QueueDirectoryPath = Settings.DirectoriesPaths.Queue,
                    ProcessedDirectoryPath = Settings.DirectoriesPaths.Processed,
                    ErrorDirectoryPath = Settings.DirectoriesPaths.Error,
                });

                dataPipelineService.ReadData();
                if (dataPipelineService.CurrentFileLocation == null)
                {
                    return;
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
    }
}
