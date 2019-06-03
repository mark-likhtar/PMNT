namespace PSLNLExportUtility.Logic.Services.CsvDataReader
{
    internal class Query
    {
        private readonly string _targetSheetName;

        private readonly string _fileName;

        private readonly bool _withKronosNumber;

        private Query(string fileName, string targetSheetName, bool withKronosNumber)
        {
            _targetSheetName = targetSheetName;
            _fileName = fileName;
            _withKronosNumber = withKronosNumber;
        }

        public static Query From(string fileName, string targetSheetName, bool withKronosNumber)
        {
            return new Query(fileName, targetSheetName, withKronosNumber);
        }

        private string ReadDataByFlag =>
            _withKronosNumber ? @"LTRIM(RTRIM([F16])) AS BadgeNumber, LTRIM(RTRIM([F17])) AS PerOrg" : @"LTRIM(RTRIM([F16])) AS PerOrg";

        public string ReadData =>
            $@"
                SELECT
                  LTRIM(RTRIM([F1]))  AS Id,
                  LTRIM(RTRIM([F2]))  AS EffectiveDate,
                  LTRIM(RTRIM([F3]))  AS Status,
                  LTRIM(RTRIM([F4]))  AS FirstName,
                  LTRIM(RTRIM([F5]))  AS LastName,
                  LTRIM(RTRIM([F6]))  AS JobCode,
                  LTRIM(RTRIM([F7]))  AS JobCodeDescription,
                  LTRIM(RTRIM([F8]))  AS DepartmentId,
                  LTRIM(RTRIM([F9]))  AS DepartmentDescription,
                  LTRIM(RTRIM([F10])) AS Location,
                  LTRIM(RTRIM([F11])) AS CompanyDescription,
                  LTRIM(RTRIM([F12])) AS Email,
                  LTRIM(RTRIM([F13])) AS ManagerName,
                  LTRIM(RTRIM([F14])) AS ManagerEmail,
                  LTRIM(RTRIM([F15])) AS TerminationDate,
                  {ReadDataByFlag}
                FROM [{_targetSheetName}]
            ";
    }
}
