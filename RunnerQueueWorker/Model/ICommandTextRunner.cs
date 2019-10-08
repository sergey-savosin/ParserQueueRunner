namespace RunnerQueueWorker.Model
{
    interface ICommandTextRunner
    {
        CommandTextRunnerResult Execute(CommandTextRunnerConfig config, CommandTextRunnerParams param);
    }
}
