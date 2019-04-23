using System;
using System.IO;
using System.Linq;

namespace PSLNLExportUtility.Logic.Services.ExcelDataReader
{
    internal static class ConnectionHelper
    {
        public const string XLS_PROVIDER = "Microsoft.Jet.OLEDB.4.0";
        public const string XLS_EXTENSION = ".xls";

        public const string XLSX_PROVIDER = "Microsoft.ACE.OLEDB.12.0";
        public const string XLSX_EXTENSION = ".xlsx";

        public const string EXCEL_VERSION_8 = "8.0";
        public const string EXCEL_VERSION_12 = "12.0";

        public static string GetConnectionString(string filePath)
        {
            string extension = Path.GetExtension(filePath);

            if (!extension.Equals(XLS_EXTENSION) && !extension.Equals(XLSX_EXTENSION))
            {
                string expectedExtensions = string
                    .Join(
                        ", ",
                        new[] { XLSX_EXTENSION, XLS_EXTENSION }
                            .Select(item => $"'{item}'")
                    );

                throw new ArgumentException(
                    $"File must be have one of these extensions: {expectedExtensions}"
                );
            }

            string provider = extension == XLS_EXTENSION
                ? XLS_PROVIDER
                : XLSX_PROVIDER;

            string excelVersion = extension == XLS_EXTENSION
                ? EXCEL_VERSION_8
                : EXCEL_VERSION_12;

            string connectionString = $"Provider={provider};Data Source={filePath};Extended Properties=\"Excel {excelVersion};HDR=Yes;IMEX=1;\"";

            return connectionString;
        }
    }
}
