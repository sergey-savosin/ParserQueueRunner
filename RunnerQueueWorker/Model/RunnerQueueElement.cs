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
        public DateTime CreatedTimeUtc;
        public int QueueStatusId;
	}
}
