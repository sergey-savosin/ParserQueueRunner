﻿using RunnerQueueWorker.Function.Model;
using RunnerQueueWorker.Utils;
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
			_startupPath = PathUtils.GetStartupPath();
		}

		public FileDownloader(string startupPath)
		{
			_startupPath = startupPath;
		}

		public RunWebCommandResult Download(string remoteUri, string fileName, string targetDirPath)
		{
			Console.WriteLine("[FileDownloader] Startup path: {0}", _startupPath);

			if (string.IsNullOrEmpty(targetDirPath))
			{
				targetDirPath = _startupPath;
			}

			Console.WriteLine("[FileDownloader] Check target dir: {0}", targetDirPath);
			EnsureDirectoryExists(targetDirPath);

			//Console.WriteLine("[FileDownloader] Making a backup");
			//string filePath = Path.Combine(_startupPath, fileName);
			//BackupCurrentFile(filePath, backupDirPath);

			Console.WriteLine("[FileDownloader] Starting download a file: {0} from URI: {1}", fileName, remoteUri);
			try
			{
				DownloadAFile(remoteUri, fileName, targetDirPath);
			}
			catch (WebException webEx)
			{
				return new RunWebCommandResult()
				{
					ResultCode = -1,
					ErrorText = webEx.Message + " " + webEx.InnerException.Message
				};
			}
			catch (Exception ex)
			{
				return new RunWebCommandResult()
				{
					ResultCode = -1,
					ErrorText = ex.Message + ". " + ex.InnerException.Message
				};
			}

			Console.WriteLine("[FileDownloader] File downloaded.");

			return new RunWebCommandResult()
			{
				ResultCode = 0,
				ErrorText = "Ok",
				OutputText = "Output ok"
			};

		}

		public static void EnsureDirectoryExists(string dirPath)
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
