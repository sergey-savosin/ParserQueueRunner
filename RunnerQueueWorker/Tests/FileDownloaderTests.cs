using Microsoft.VisualStudio.TestTools.UnitTesting;
using RunnerQueueWorker.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunnerQueueWorker.Tests
{
	[TestClass]
	public class FileDownloaderTests
	{
		[TestMethod]
		public void DownloadFile_Test()
		{
			string remoteUri = "https://docs.google.com/spreadsheets/d/1-vTMclGuOJeaw4yqm3AWvg04q-C4sHyQ0zmKiQTY9aY/export?format=xlsx&id=1-vTMclGuOJeaw4yqm3AWvg04q-C4sHyQ0zmKiQTY9aY";
			string fileName = "file.xlsx";
			string backupDirName = "backup";

			var fd = new FileDownloader("c:\\work");
			fd.Download(remoteUri, fileName, backupDirName);

		}
	}
}
