using Newtonsoft.Json;
using RunnerQueueWorker.Function;
using RunnerQueueWorker.Function.Model;
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
            Console.WriteLine("-- RunnerQueueWorker, ver 0.4 --");
            Console.WriteLine("Starting work.");

			string iniFullPath = PathUtils.GetApplicationIniPath();

            var parserWebQueueParameters = new RunnerWebQueueParameters();
            int nElementsToProcess = 0;

            try
            {
                // Считывание настроек программы
                IniReader iniReader = new IniReader(iniFullPath);
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
                RunWebCommandResult runnerResult = RunWebCommand(elt.CommandName, elt.CommandParameters);


                // ToDo: обработка ошибки
                if (runnerResult.ResultCode != 0)
                {
                    Console.WriteLine("Error in main: {0}. \nStack trace: {1}", runnerResult.OutputText, runnerResult.ErrorText);
					parserWebQueue.SetQueueElementStatus(elt.RunnerQueueId, QueueStatus.DoneWithError, runnerResult.ErrorText);
					Console.WriteLine("Error saved.");
					return 0; //stop processing
                }

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

        private static RunWebCommandResult RunWebCommand(string commandName, string commandParametersJson)
        {
			if (commandName == "RunApplication")
			{
				RunApplicationParameters runParams = JsonConvert.DeserializeObject<RunApplicationParameters>(commandParametersJson);

				if (runParams.CommandStartTimeoutMS == 0)
				{
					runParams.CommandStartTimeoutMS = 30000;
				}

				IRunApplication commandTextRunner = new WindowsCommandTextRunner();
				return commandTextRunner.Execute(runParams);
			}
			else if (commandName == "DownloadFile")
			{
				var downloadParams = JsonConvert.DeserializeObject<FileDownloaderParams>(commandParametersJson);
				FileDownloader fd = new FileDownloader();
				return fd.Download(downloadParams.RemoteUri, downloadParams.FileName, downloadParams.TargetDirPath);
			}
			else if (commandName == "ImportFromGoogleSheetToExcel")
			{
				var importGSParams = JsonConvert.DeserializeObject<ImportFromGoogleSheetParameters>(commandParametersJson);
				var igs = new ImportFromGoogleSheetToExcel();
				return igs.Execute(importGSParams);
			}
			else
			{
				throw new NotSupportedException("CommandName not supported: " + commandName);
			}
        }
    }
}
