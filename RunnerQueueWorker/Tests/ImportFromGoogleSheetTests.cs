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
			{ };

			var result = import.Execute(importParameters);
			//Assert.IsNotNull(result);
		}
	}
}
