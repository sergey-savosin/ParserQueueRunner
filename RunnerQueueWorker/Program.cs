using Newtonsoft.Json;
using RunnerQueueWorker.Function;
using RunnerQueueWorker.Model;
using RunnerQueueWorker.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RunnerQueueWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-- RunnerQueueWorker, ver 0.3 --");
            Console.WriteLine("Starting work.");

            string EXE = Assembly.GetExecutingAssembly().GetName().Name;
            string startupPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string iniFullPath = Path.Combine(startupPath, EXE + ".ini");

            var parserWebQueueParameters = new RunnerWebQueueParameters();
            //ToDo: implement GoogeSheet downloader + archivator
            //WebParserConfig parserConfig = new WebParserConfig();
            int nElementsToProcess = 0;

            try
            {
                // Считывание настроек программы
                IniReader iniReader = new IniReader(iniFullPath);
                string testValue = iniReader.GetValue("test", "section", "10");
                Console.WriteLine($"Read config file: {iniFullPath}");

                // Program params
                nElementsToProcess = iniReader.GetIntValue("NumberElementsForProcessing", "Program", 0);

                // WebService parameters
                parserWebQueueParameters.NewElementUrl = iniReader.GetValue("NewElementUrl", "QueueWebService");
                parserWebQueueParameters.Timeout = iniReader.GetIntValue("Timeout", "QueueWebService", 20000);
                parserWebQueueParameters.ContentType = "application/json";
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Can't find ini file at {iniFullPath}.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error readind ini file: {ex.GetType()}");
                return;
            }

            while (nElementsToProcess-- > 0)
            {
                int cnt = processQueue(parserWebQueueParameters);
                if (cnt == 0)
                    break;
            }

            Console.WriteLine("Work finished.");
            //Console.WriteLine("Work finished. Press any key.");
            //Console.ReadKey();
        }

        /// <summary>
        /// Обработка элемента очереди ParserQueue
        /// </summary>
        /// <returns>Кол-во обработанных элементов очереди</returns>
        public static int processQueue(RunnerWebQueueParameters parserWebQueueParameters)
        {
            // Создание обработчика очереди
            IRunnerWebQueue parserWebQueue = new OnlineRunnerWebQueue(parserWebQueueParameters);
            RunnerQueueElement elt = null;

            try
            {
                // Получение элемента очереди
                elt = parserWebQueue.GetNewElement();

                if (elt == null)
                {
                    Console.WriteLine("No new elements to process.");
                    return 0;
                }

                // Пометка статусом "Взят в обработку"
                parserWebQueue.SetQueueElementStatus(elt.RunnerQueueId, QueueStatus.Processing);

                // Выполнить команду
                Console.WriteLine("Starting command: {0}, params: {1}", elt.CommandName, elt.CommandParameters);
                CommandTextRunnerResult runnerResult = RunCommandText(elt.CommandName, elt.CommandParameters);


                // ToDo: обработка ошибки
                if (runnerResult.ResultCode != 0)
                {
                    Console.WriteLine("Error in main: {0}. \nStack trace: {1}", runnerResult.OutputText, runnerResult.ErrorText);
					parserWebQueue.SetQueueElementStatus(elt.RunnerQueueId, QueueStatus.DoneWithError, runnerResult.OutputText);
					Console.WriteLine("Error saved.");
					return 0; //stop processing
                }


                // ToDo: вывести элемент очереди
                //printParserQueueElement(elt);

                // Пометка элемента очереди как успешно обработанный
                parserWebQueue.SetQueueElementStatus(elt.RunnerQueueId, QueueStatus.Done);

                return 1;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error in main: {0}", exc.Message);

                // Установить статус "Ошибка обработки"
                if (elt != null)
                {
                    parserWebQueue.SetQueueElementStatus(elt.RunnerQueueId, QueueStatus.DoneWithError, exc.Message);
					Console.WriteLine("Error saved.");
                }

            }
            return 0;

        }

        private static CommandTextRunnerResult RunCommandText(string commandName, string commandParametersJson)
        {
			if (commandName == "RunApplication")
			{
				RunApplicationParameters runParams = JsonConvert.DeserializeObject<RunApplicationParameters>(commandParametersJson);


				CommandTextRunnerConfig config = new CommandTextRunnerConfig()
				{
					CommandStartTimeout = 30000, //ToDo read value to config.ini
					GoogleSheetURI = ""
				};

				CommandTextRunnerParams param = new CommandTextRunnerParams()
				{
					CommandText = runParams.ApplicationPath,
					CommandParameters = ""
				};

				ICommandTextRunner commandTextRunner = new WindowsCommandTextRunner();
				return commandTextRunner.Execute(config, param);
			}
			else if (commandName == "DownloadGoogleSheet")
			{
				throw new NotSupportedException("CommandName not supported: " + commandName);
			}
			else
			{
				throw new NotSupportedException("CommandName not supported: " + commandName);
			}
        }
    }
}
