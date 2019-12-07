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
			//string remoteUri = "https://docs.google.com/spreadsheets/d/1-vTMclGuOJeaw4yqm3AWvg04q-C4sHyQ0zmKiQTY9aY/export?format=xlsx&id=1-vTMclGuOJeaw4yqm3AWvg04q-C4sHyQ0zmKiQTY9aY";
			string remoteUri = "http://www.ya.ru";
			string fileName = "file.html";
			string backupDirName = "c:\\work\\backup";

			var fd = new FileDownloader();
			var result = fd.Download(remoteUri, fileName, backupDirName);

			Assert.AreEqual(0, result.ResultCode);

		}
	}
}
