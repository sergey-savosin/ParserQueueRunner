namespace RunnerQueueWorker.Model
{
    public class RunnerWebQueueParameters
    {
        public string WebServiceUrl { get; set; }
        public string Method { get; set; }
        public int Timeout { get; set; }
        public string ContentType { get; set; }
    }
}
