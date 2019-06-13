using System;
using System.IO;
using System.Linq;

namespace PSLNLExportUtility.Logic.Services.CsvDataReader
{
    internal static class ConnectionHelper
    {
        public const string CSV_EXTENSION = ".csv";

        public static string GetConnectionString(string filePath)
        {
            string extension = Path.GetExtension(filePath);

            if (!extension.Equals(CSV_EXTENSION))
            {
                throw new ArgumentException(
                    $"File must be have one of these extensions: {CSV_EXTENSION}"
                );
            }
            string directory = new FileInfo(filePath).Directory.FullName;
            string connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{directory}\";Extended Properties=\"Text;HDR=No;\"";
            return connectionString;
        }
    }
}
