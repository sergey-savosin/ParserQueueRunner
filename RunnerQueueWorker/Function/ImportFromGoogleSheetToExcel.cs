using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using RunnerQueueWorker.Function.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunnerQueueWorker.Function
{
	public class ImportFromGoogleSheetToExcel
	{
		readonly string _applicationName = "RunnerQueueWorker";
		readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };

		public ImportFromGoogleSheetToExcel()
		{

		}

		public RunWebCommandResult Execute(ImportFromGoogleSheetParameters importParameters)
		{
			GoogleCredential credential;

			credential = GoogleCredential
				.FromFile(importParameters.CredentialsFileNamePath)
				.CreateScoped(Scopes);
			
			// Create Google Sheets API service.
			var service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = _applicationName,
			});

			// Define request parameters.
			string spreadsheetId = importParameters.SpreadsheetId;
			string range = importParameters.ImportRange;
			SpreadsheetsResource.ValuesResource.GetRequest request =
					service.Spreadsheets.Values.Get(spreadsheetId, range);

			ValueRange response = request.Execute();
			IList<IList<object>> values = response.Values;
			if (values != null && values.Count > 0)
			{
				Console.WriteLine("- Reading data from Google sheet: -");
				for (int row = 0; row < values.Count; row++)
				{
					Console.Write("[row {0}] ", row);
					for (int col = 0; col < values[row].Count; col++)
					{
						Console.Write("|{1,5}", col, values[row][col]);
					}
					Console.WriteLine();
				}
			}
			else
			{
				Console.WriteLine("No data found.");
			}
			//Console.Read();

			return null;
		}
	}
}
