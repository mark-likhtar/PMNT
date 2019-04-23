namespace PSLNLExportUtility.Logic.Services.ExcelDataReader
{
    internal class Query
    {
        private readonly string _targetSheetName;

        private Query(string targetSheetName)
        {
            _targetSheetName = targetSheetName;
        }

        public static Query From(string targetSheetName)
        {
            return new Query(targetSheetName);
        }

        public string ReadData =>
            $@"
                SELECT
                  LTRIM(RTRIM([EMPLID]))                   AS Id,
                  LTRIM(RTRIM([EFFDT]))                    AS EffectiveDate,
                  LTRIM(RTRIM([EMPL_STATUS]))              AS Status,
                  LTRIM(RTRIM([FIRST_NAME]))               AS FirstName,
                  LTRIM(RTRIM([MIDDLE I]))                 AS MiddleName,
                  LTRIM(RTRIM([LAST_NAME]))                AS LastName,
                  LTRIM(RTRIM([JOBCODE]))                  AS JobCode,
                  LTRIM(RTRIM([JOBCODE DESCR]))            AS JobCodeDescription,
                  LTRIM(RTRIM([DEPTID]))                   AS DepartmentId,
                  LTRIM(RTRIM([DEPTID DESCR]))             AS DepartmentDescription,
                  LTRIM(RTRIM([LOCATION]))                 AS Location,
                  LTRIM(RTRIM([LOCATION DESCR]))           AS LocationDescription,
                  LTRIM(RTRIM([COMPANY]))                  AS Company,
                  LTRIM(RTRIM([COMPANY DESCR]))            AS CompanyDescription,
                  LTRIM(RTRIM([EMAILID (Employee Email)])) AS Email,
                  LTRIM(RTRIM([MANAGER NAME]))             AS ManagerName,
                  LTRIM(RTRIM([MANAGER EMAIL]))            AS ManagerEmail,
                  LTRIM(RTRIM([SUPPER LOCATION]))          AS SupperLocation,
                  LTRIM(RTRIM([TERMINATION_DT]))           AS TerminationDate,
                  LTRIM(RTRIM([BADGE NUMBER]))             AS BadgeNumber,
                  LTRIM(RTRIM([PER_ORG]))                  AS PerOrg
                FROM [{_targetSheetName}]
                WHERE [EMPLID] IS NOT NULL
            ";
    }
}
