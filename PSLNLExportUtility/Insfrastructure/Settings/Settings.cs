namespace PSLNLExportUtility.Insfrastructure.Settings
{
    public static class Settings
    {
        public static class DirectoriesPaths
        {
            public static string Queue => SettingsLoader.GetValue<string>("QueueDirectoryPath");

            public static string Processed => SettingsLoader.GetValue<string>("ProcessedDirectoryPath");

            public static string Error => SettingsLoader.GetValue<string>("ErrorDirectoryPath");

            public static string Output => SettingsLoader.GetValue<string>("OutputDirectoryPath");
        }

        public static class EmailAccount
        {
            public static string ReportEmail = SettingsLoader.GetValue<string>("ReportEmail");

            public static string ReportSubject = SettingsLoader.GetValue<string>("ReportSubject");
        }

        public static bool PipelineEnabled => SettingsLoader.GetValue<bool>("PipelineEnabled");

        public static bool WithKronosNumber => SettingsLoader.GetValue<bool>("WithKronosNumber");
    }
}
