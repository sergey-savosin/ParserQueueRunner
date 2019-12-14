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

			credential = GoogleCredential.FromFile("credentials.json").CreateScoped(Scopes);
			// Create Google Sheets API service.
			var service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = _applicationName,
			});

			// Define request parameters.
			String spreadsheetId = "1DVVmqRWVTn4nQkwVOd5VOucQ_L4fTT4S3EiVR-W13qA";
			String range = "Sheet1!A2:E";
			SpreadsheetsResource.ValuesResource.GetRequest request =
					service.Spreadsheets.Values.Get(spreadsheetId, range);

			ValueRange response = request.Execute();
			IList<IList<Object>> values = response.Values;
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
