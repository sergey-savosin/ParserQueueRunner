using Microsoft.VisualStudio.TestTools.UnitTesting;
using RunnerQueueWorker.Function;
using RunnerQueueWorker.Function.Model;
using RunnerQueueWorker.Model;

namespace RunnerQueueWorker.Tests
{
    [TestClass]
    public class CommandTextRunnerTests
    {
        readonly IRunApplication runner;
		RunApplicationParameters cmdParams = new RunApplicationParameters()
		{
			CommandStartTimeoutMS = 20000
		};

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
            cmdParams.ApplicationPath = @"D:\work\vba\Parser.xla1";
            var res = runner.Execute(cmdParams);

            Assert.IsNotNull(res);
            Assert.AreEqual(-1, res.ResultCode);
			StringAssert.Contains(res.OutputText, "\""+
				cmdParams.ApplicationPath +
				"\""+
				" не является внутренней или внешней");
        }

        [TestMethod]
        public void StartExcelBook_Test()
        {
            cmdParams.ApplicationPath = @"D:\work\vba\Parser.xla";
            var res = runner.Execute(cmdParams);

            Assert.IsNotNull(res);
            Assert.AreEqual(0, res.ResultCode, res.OutputText);
            Assert.IsNull(res.OutputText);
        }

        [TestMethod]
        public void StartShellScript_Test()
        {
            cmdParams.ApplicationPath = @"D:\work\vba\StopExcel.bat";
            var res = runner.Execute(cmdParams);

            Assert.IsNotNull(res);
            Assert.AreEqual(0, res.ResultCode, res.OutputText);
            Assert.IsNull(res.OutputText);
        }
    }
}
