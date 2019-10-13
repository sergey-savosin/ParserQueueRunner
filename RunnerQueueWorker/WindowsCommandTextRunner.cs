using RunnerQueueWorker.Model;
using System;
using System.Diagnostics;

namespace RunnerQueueWorker
{
    public class WindowsCommandTextRunner : ICommandTextRunner
    {
        public CommandTextRunnerResult Execute(CommandTextRunnerConfig config, CommandTextRunnerParams param)
        {
            runCommand_cmd(param.CommandText);

            return new CommandTextRunnerResult()
            {
                ErrorText = "not implemented",
                OutputText = "empty",
                ResultCode = 0
            };
        }

        static void runCommand_cmd(string commandText)
        {
            //* Create your Process
            Process process = new Process();
            process.StartInfo.FileName = commandText;
            process.StartInfo.Arguments = "";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            //* Set ONLY ONE handler here.
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            //* Start process
            process.Start();
            //* Read one element asynchronously
            process.BeginErrorReadLine();
            //* Read the other one synchronously
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine("output> " + output);

            bool exited = process.WaitForExit(2000);
            Console.WriteLine("IsExited> {0}", exited);

        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine("error> " + outLine.Data);
        }

        static void runCommand_normal(string commandText)
        {
        }
    }
}
