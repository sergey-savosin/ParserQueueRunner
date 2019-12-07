using System;

namespace RunnerQueueWorker.Model
{
	/// <summary>
	/// Description of RunnerQueueElement.
	/// </summary>
	public class RunnerQueueElement
	{
		public int RunnerQueueId;
		public string CommandName;
		public string CommandParameters;
        public DateTime CreatedTime;
        public int QueueStatusId;
	}
}
