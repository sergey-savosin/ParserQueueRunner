namespace RunnerQueueWorker.Function.Model
{
	public class ImportFromGoogleSheetParameters
	{
		public string CredentialsFileNamePath { get; set; }

		public string ApplicationName { get; set;}

		public string SpreadsheetId { get; set; }

		public string ImportRange { get; set; }

		public string ExcelInsertRange { get; set; }
	}
}
