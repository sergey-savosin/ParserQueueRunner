using RunnerQueueWorker.Model;
using System;
using System.Diagnostics;

namespace RunnerQueueWorker
{
    public class WindowsCommandTextRunner : ICommandTextRunner
    {
        public CommandTextRunnerResult Execute(CommandTextRunnerConfig config, CommandTextRunnerParams param)
        {
            string errorText = null;
            string errorStackTrace = null;
            int resultCode = 0;

            try
            {
                runCommand_cmd(param.CommandText, config.CommandStartTimeout);
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                errorStackTrace = ex.StackTrace;
                resultCode = -1;
            }

            return new CommandTextRunnerResult()
            {
                ErrorText = errorStackTrace,
                OutputText = errorText,
                ResultCode = resultCode
            };
        }

        static void runCommand_cmd(string commandText, int timeout)
        {
            //* Create your Process
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + commandText;
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

            bool exited = process.WaitForExit(timeout);
            Console.WriteLine("IsExited> {0}", exited);

            //ToDo save output strings

        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine("error> " + outLine.Data);
        }

        static void runCommand_shell(string commandText, int timeout)
        {
            //* Create your Process
            Process process = new Process();
            process.StartInfo.FileName = commandText;
            process.StartInfo.Arguments = "";
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            //* Start process
            process.Start();
            bool exited = process.WaitForExit(timeout);

            if (!exited)
            {
                throw new Exception($"Command start timeout: {timeout} ms.");
            }

        }
    }
}
