﻿using PSLNLExportUtility.Infrastructure.Logging;
using PSLNLExportUtility.Logic.Services.DataImport.Models;
using PSLNLExportUtility.Logic.Services.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PSLNLExportUtility.Logic.Services.DataImport
{
    public class DataPipelineService
    {
        private const string CSV_FILE_SEARCH_PATTERN = "*.csv*";
        private const string DEFAULT_LOG_FILE_NAME = "application.log";

        private readonly DataPipelineServiceSettings _settings;
        private readonly Logger _logger;
        private readonly bool _isPipelineEnabled;

        public string CurrentFileLocation { get; private set; }

        public DataPipelineService(Logger logger, DataPipelineServiceSettings settings, bool pipelineEnabled = true)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _isPipelineEnabled = pipelineEnabled;
        }

        public void PrepareForProcessing()
        {
            CurrentFileLocation = GetLatestCreatedFilePathFromQueue();
            if (CurrentFileLocation == null)
            {
                _logger.Info("There are no csv files to work with.");
                return;
            }

            _logger.Info($"Found the file by the specified path: '{CurrentFileLocation}'.");
            if (!_isPipelineEnabled)
            {
                return;
            }

            MoveCurrentFileToDirectory(Path.GetTempPath());
        }

        public void SaveDataToError(string logs)
        {
            if (!_isPipelineEnabled)
            {
                return;
            }

            ValidateSourceFilePrescence();

            string directoryPath = CreateErrorDirectory();
            MoveCurrentFileToDirectory(directoryPath);
            WriteLogsToFile(logs, directoryPath, DEFAULT_LOG_FILE_NAME);
        }

        public void SaveDataToProcessed()
        {
            if (!_isPipelineEnabled)
            {
                return;
            }

            ValidateSourceFilePrescence();

            MoveCurrentFileToDirectory(_settings.ProcessedDirectoryPath, true);
        }

        private void WriteLogsToFile(string logs, string directoryPath, string fileName)
        {
            string filePath = Path.Combine(directoryPath, fileName);

            File.WriteAllText(filePath, logs, Encoding.UTF8);
        }

        private string GetLatestCreatedFilePathFromQueue()
        {
            var files = Directory
                .EnumerateFiles(_settings.QueueDirectoryPath, CSV_FILE_SEARCH_PATTERN)
                .ToList();

            files.Sort(new FileNameComparer());

            return files.FirstOrDefault();
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

        private void MoveCurrentFileToDirectory(string destinationDirectoryPath, bool withDateStamp = false)
        {
            var fileName = "";

            if (withDateStamp)
            {
                fileName = $"{Path.GetFileNameWithoutExtension(CurrentFileLocation)}_{DateTime.Now.TimeOfDay}{Path.GetExtension(CurrentFileLocation)}".Replace(":", "");
            }
            else
            {
                fileName = Path.GetFileName(CurrentFileLocation);
            }

            string newFilePath = Path.Combine(
                destinationDirectoryPath,
                fileName
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
