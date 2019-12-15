using Microsoft.VisualStudio.TestTools.UnitTesting;
using RunnerQueueWorker.Function;
using RunnerQueueWorker.Function.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunnerQueueWorker.Tests
{
	[TestClass]
	public class ImportFromGoogleSheetTests
	{
		[TestMethod]
		public void ImportData()
		{
			var import = new ImportFromGoogleSheetToExcel();
			var importParameters = new ImportFromGoogleSheetParameters()
			{
				SpreadsheetId = "1DVVmqRWVTn4nQkwVOd5VOucQ_L4fTT4S3EiVR-W13qA",
				ImportRange = "Sheet1!A1:E",
				CredentialsFileNamePath = @"C:\git\ParserQueueRunner\credentials.json",
				ExcelInsertRange = ""
			};

			var result = import.Execute(importParameters);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.ResultCode);
		}
	}
}
