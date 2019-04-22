using NLog;
using NLog.Targets;
using System;

namespace PSLNLExportUtility.Infrastructure.Logging
{
    public class Logger
    {
        private static readonly string MEMORY_TARGET_NAME = "memory";

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void Error(Exception exception, string message)
        {
            _logger.Error(exception, message);
        }

        public void Error(Exception exception)
        {
            _logger.Error(exception);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public string GetLogs()
        {
            MemoryTarget memoryTarget =
                (MemoryTarget)LogManager.Configuration.FindTargetByName(MEMORY_TARGET_NAME);

            return string.Join(Environment.NewLine, memoryTarget.Logs);
        }
    }
}
