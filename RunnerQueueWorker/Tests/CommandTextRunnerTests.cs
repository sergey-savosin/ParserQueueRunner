using Microsoft.VisualStudio.TestTools.UnitTesting;
using RunnerQueueWorker.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace RunnerQueueWorker.Tests
{
    [TestClass]
    public class CommandTextRunnerTests
    {
        readonly ICommandTextRunner runner;
        CommandTextRunnerConfig cmdConfig = new CommandTextRunnerConfig() { GoogleSheetURI = "" };
        CommandTextRunnerParams cmdParams = new CommandTextRunnerParams();

        public CommandTextRunnerTests()
        {
            runner = new ConsoleAppTextRunner();
        }

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void Test1()
        {
            cmdParams.CommandText = "calc.exe";
            cmdParams.CommandParameters = "";
            var res = runner.Execute(cmdConfig, cmdParams);

            Assert.IsNotNull(res);
        }
    }
}
