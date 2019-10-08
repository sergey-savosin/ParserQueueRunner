using RunnerQueueWorker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunnerQueueWorker
{
    public class WindowsCommandTextRunner : ICommandTextRunner
    {
        public CommandTextRunnerResult Execute(CommandTextRunnerConfig config, CommandTextRunnerParams param)
        {
            return new CommandTextRunnerResult()
            {
                ErroreText = "not implemented",
                ResultCode = 0
            };
        }

    }
}
