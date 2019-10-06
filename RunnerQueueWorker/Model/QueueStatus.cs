namespace RunnerQueueWorker.Model
{
    public enum QueueStatus
    {
        NewRecord = 1,
        Processing = 2,
        Done = 3,
        DoneWithError = 4
    }
}
