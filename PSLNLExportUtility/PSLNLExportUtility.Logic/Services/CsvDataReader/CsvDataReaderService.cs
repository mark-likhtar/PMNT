using Dapper;
using PSLNLExportUtility.Logic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace PSLNLExportUtility.Logic.Services.CsvDataReader
{
    public class CsvDataReaderService
    {
        public IEnumerable<Employee> ReadData(string filePath, bool withKronosNumber)
        {
            using (OleDbConnection connection = CreateConnection(filePath))
            {
                string fileName = new FileInfo(filePath).Name;
                string sheetName = GetFirstSheetName(connection);
                string query = Query.From(fileName, sheetName, withKronosNumber).ReadData;

                return connection.Query<Employee>(query);
            }
        }

        private string GetFirstSheetName(OleDbConnection connection)
        {
            DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (schema == null || schema.Rows.Count <= 0)
            {
                throw new ArgumentNullException("Csv file does not contain data sheets.");
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
