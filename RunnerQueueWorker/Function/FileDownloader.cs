using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RunnerQueueWorker.Function
{
    public class FileDownloader
    {
		string _startupPath { get; set; }

		public FileDownloader()
		{
			_startupPath = GetStartupPath();
		}

		public FileDownloader(string startupPath)
		{
			_startupPath = startupPath;
		}

		public void Download(string remoteUri, string fileName, string backupDirName)
		{
			Console.WriteLine("Startup path: {0}", _startupPath);

			Console.WriteLine("Check backup dir");
			string backupDirPath = Path.Combine(_startupPath, backupDirName);
			EnsureBackupDirectory(backupDirPath);

			Console.WriteLine("Making a backup");
			string filePath = Path.Combine(_startupPath, fileName);
			BackupCurrentFile(filePath, backupDirPath);

			Console.WriteLine("Starting download a file");
			DownloadAFile(remoteUri, fileName, _startupPath);

		}

		public static string GetStartupPath()
		{
			return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
		}

		public static void EnsureBackupDirectory(string dirPath)
		{
			Directory.CreateDirectory(dirPath);
		}

		public static void BackupCurrentFile(string filePath, string backupDirPath)
		{
			try
			{
				var fileFrom = filePath;
				var fileName = Path.GetFileName(filePath);
				var fileTo = Path.Combine(Path.GetDirectoryName(filePath), backupDirPath, fileName);
				File.Move(fileFrom, fileTo);
			}
			catch(IOException ex)
			{
				Console.WriteLine("move error: " + ex.Message);
			}
			catch(Exception ex)
			{
				Console.WriteLine("moce common error: " + ex.Message);
			}
		}

		public static void DownloadAFile(string remoteUri, string fileName, string targetDirectory)
		{
			string fullPath = Path.Combine(targetDirectory, fileName);

			// Create a new WebClient instance.
			using (WebClient myWebClient = new WebClient())
			{
				// Download the Web resource and save it into the current filesystem folder.
				myWebClient.DownloadFile(remoteUri, fullPath);
			}
		}

	}
}
