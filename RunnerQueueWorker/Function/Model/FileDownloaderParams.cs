namespace RunnerQueueWorker.Function.Model
{
	public class FileDownloaderParams
	{
		public string RemoteUri { get; set; }

		public string FileName { get; set; }

		public string TargetDirPath { get; set; }
	}
}
