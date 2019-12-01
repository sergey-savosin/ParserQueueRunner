using RunnerQueueWorker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunnerQueueWorker.Function
{
    public class ConsoleAppTextRunner : ICommandTextRunner
    {
        public CommandTextRunnerResult Execute(CommandTextRunnerConfig config, CommandTextRunnerParams param)
        {
            //ConsoleApp app = new ConsoleApp(param.CommandText, param.CommandParameters);
            //List<string> cmdOutput = new List<string>();
            //app.ConsoleOutput += (o, args) =>
            //{
            //    cmdOutput.Add(args.Line);
            //};
            //app.Run();
            //var exited = app.WaitForExit(2000); // 2 second to wait

            //string strOutput = String.Join("; ", cmdOutput.ToArray());

            //Console.WriteLine("IsExited: {0}", exited);
            //Console.WriteLine("ExitCode: {0}", app.ExitCode);
            //Console.WriteLine("Result:" + strOutput);

            //var res = new CommandTextRunnerResult()
            //{
            //    OutputText = strOutput,
            //    ResultCode = app.ExitCode.Value,
            //    ErrorText = "no errors"
            //};

            return null;
        }
    }
}
