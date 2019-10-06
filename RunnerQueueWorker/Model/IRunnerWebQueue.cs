namespace RunnerQueueWorker.Model
{
    public interface IRunnerWebQueue
    {
        RunnerQueueElement GetNewElement();
        void SetQueueElementStatus(int RunnerQueueId, QueueStatus queueStatus, string ErrorMessage = "");
    }
}
