namespace RunnerQueueWorker.Model
{
    public class SetQueueElementStatusRequest
    {
        public string QueueStatusId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
