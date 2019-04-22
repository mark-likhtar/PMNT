using PSLNLExportUtility.Infrastructure.Logging;
using PSLNLExportUtility.Logic.Services.DataImport.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PSLNLExportUtility.Logic.Services.DataImport
{
    public class DataPipelineService
    {
        private const string EXCEL_FILE_SEARCH_PATTERN = "*.xls*";
        private const string DEFAULT_LOG_FILE_NAME = "application.log";

        private readonly DataPipelineServiceSettings _settings;
        private readonly Logger _logger;

        public string CurrentFileLocation { get; private set; }

        public DataPipelineService(Logger logger, DataPipelineServiceSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ReadData()
        {
            CurrentFileLocation = GetLatestCreatedFilePathFromQueue();
            if (CurrentFileLocation == null)
            {
                _logger.Info("There are no excel files to work with.");
                return;
            }

            _logger.Info($"Found the file by the specified path: '{CurrentFileLocation}'.");
            MoveCurrentFileToDirectory(Path.GetTempPath());
        }

        public void SaveDataToError(string logs)
        {
            ValidateSourceFilePrescence();

            string directoryPath = CreateErrorDirectory();
            MoveCurrentFileToDirectory(directoryPath);
            WriteLogsToFile(logs, directoryPath, DEFAULT_LOG_FILE_NAME);
        }

        public void SaveDataToProcessed()
        {
            ValidateSourceFilePrescence();

            MoveCurrentFileToDirectory(_settings.ProcessedDirectoryPath);
        }

        private void WriteLogsToFile(string logs, string directoryPath, string fileName)
        {
            string filePath = Path.Combine(directoryPath, fileName);

            File.WriteAllText(filePath, logs, Encoding.UTF8);
        }

        private string GetLatestCreatedFilePathFromQueue()
        {
            return Directory
                .EnumerateFiles(_settings.QueueDirectoryPath, EXCEL_FILE_SEARCH_PATTERN)
                .OrderByDescending(filePath => File.GetCreationTime(filePath))
                .FirstOrDefault();
        }

        private string CreateErrorDirectory()
        {
            string fileName = Path.GetFileNameWithoutExtension(CurrentFileLocation);

            string directoryPath = Path.Combine(
                _settings.ErrorDirectoryPath,
                fileName
            );
            if (Directory.Exists(directoryPath))
            {
                throw new ApplicationException($"Directory with the name '{fileName}' already exists.");
            }

            Directory.CreateDirectory(directoryPath);

            return directoryPath;
        }

        private void MoveCurrentFileToDirectory(string destinationDirectoryPath)
        {
            string newFilePath = Path.Combine(
                destinationDirectoryPath,
                Path.GetFileName(CurrentFileLocation)
            );

            File.Move(CurrentFileLocation, newFilePath);
            if (!File.Exists(newFilePath))
            {
                throw new ApplicationException(
                    $"Couldn't change source file path from '{CurrentFileLocation}' to '{newFilePath}'."
                );
            }

            CurrentFileLocation = newFilePath;
        }

        private void ValidateSourceFilePrescence()
        {
            if (!File.Exists(CurrentFileLocation))
            {
                throw new ApplicationException($"Couldn't find the file by the specified path: '{CurrentFileLocation}'.");
            }
        }
    }
}
