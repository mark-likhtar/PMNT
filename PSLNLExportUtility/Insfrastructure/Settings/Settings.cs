namespace PSLNLExportUtility.Insfrastructure.Settings
{
    public static class Settings
    {
        public static class DirectoriesPaths
        {
            public static string Queue => SettingsLoader.GetValue<string>("QueueDirectoryPath");

            public static string Processed => SettingsLoader.GetValue<string>("ProcessedDirectoryPath");

            public static string Error => SettingsLoader.GetValue<string>("ErrorDirectoryPath");
        }
    }
}
