using Microsoft.VisualStudio.TestTools.UnitTesting;
using RunnerQueueWorker.Model;

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
            runner = new WindowsCommandTextRunner();
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
