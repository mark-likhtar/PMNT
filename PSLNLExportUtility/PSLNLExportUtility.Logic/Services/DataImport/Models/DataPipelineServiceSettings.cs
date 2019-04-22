namespace PSLNLExportUtility.Logic.Services.DataImport.Models
{
    public class DataPipelineServiceSettings
    {
        public string QueueDirectoryPath { get; set; }

        public string ProcessedDirectoryPath { get; set; }

        public string ErrorDirectoryPath { get; set; }
    }
}
