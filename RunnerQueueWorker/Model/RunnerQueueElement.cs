using System;

namespace RunnerQueueWorker.Model
{
	/// <summary>
	/// Description of RunnerQueueElement.
	/// </summary>
	public class RunnerQueueElement
	{
		public int RunnerQueueId;
		public string CommandText;
		public string Parameter1;
		public string Parameter2;
        public DateTime CreatedTimeUtc;
        public int QueueStatusId;
	}
}
