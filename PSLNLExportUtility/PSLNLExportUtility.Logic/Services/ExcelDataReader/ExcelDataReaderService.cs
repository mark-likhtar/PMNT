using Dapper;
using PSLNLExportUtility.Logic.Services.ExcelDataReader.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace PSLNLExportUtility.Logic.Services.ExcelDataReader
{
    public class ExcelDataReaderService
    {
        public IEnumerable<Employee> ReadData(string filePath)
        {
            using (OleDbConnection connection = CreateConnection(filePath))
            {
                string sheetName = GetFirstSheetName(connection);
                string query = Query.From(sheetName).ReadData;

                IEnumerable<Employee> result = connection.Query<Employee>(query);

                return result;
            }
        }

        private string GetFirstSheetName(OleDbConnection connection)
        {
            DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (schema == null || schema.Rows.Count <= 0)
            {
                throw new ArgumentNullException("Excel file does not contain data sheets.");
            }

            return schema.Rows[0]["TABLE_NAME"].ToString();
        }

        private OleDbConnection CreateConnection(string filePath)
        {
            var connection = new OleDbConnection(ConnectionHelper.GetConnectionString(filePath));
            connection.Open();

            return connection;
        }
    }
}
