using RunnerQueueWorker.Function.Model;

namespace RunnerQueueWorker.Function.Model
{
    interface IRunApplication
    {
        RunWebCommandResult Execute(RunApplicationParameters parameters);
    }
}
