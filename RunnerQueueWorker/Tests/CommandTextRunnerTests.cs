using Microsoft.VisualStudio.TestTools.UnitTesting;
using RunnerQueueWorker.Model;

namespace RunnerQueueWorker.Tests
{
    [TestClass]
    public class CommandTextRunnerTests
    {
        readonly ICommandTextRunner runner;
        CommandTextRunnerConfig cmdConfig = new CommandTextRunnerConfig() {
            GoogleSheetURI = "",
            CommandStartTimeout = 20000
        };
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
        public void FileNotFound_Test()
        {
            cmdParams.CommandText = @"D:\work\vba\Parser.xla1";
            cmdParams.CommandParameters = "";
            var res = runner.Execute(cmdConfig, cmdParams);

            Assert.IsNotNull(res);
            Assert.AreEqual(-1, res.ResultCode);
            Assert.AreEqual("Не удается найти указанный файл", res.OutputText);
        }

        [TestMethod]
        public void StartExcelBook_Test()
        {
            cmdParams.CommandText = @"D:\work\vba\Parser.xla";
            cmdParams.CommandParameters = "";
            var res = runner.Execute(cmdConfig, cmdParams);

            Assert.IsNotNull(res);
            Assert.AreEqual(0, res.ResultCode, res.OutputText);
            Assert.IsNull(res.OutputText);
        }

        [TestMethod]
        public void StartShellScript_Test()
        {
            cmdParams.CommandText = @"D:\work\vba\StopExcel.bat";
            cmdParams.CommandParameters = "";
            var res = runner.Execute(cmdConfig, cmdParams);

            Assert.IsNotNull(res);
            Assert.AreEqual(0, res.ResultCode, res.OutputText);
            Assert.IsNull(res.OutputText);
        }
    }
}
