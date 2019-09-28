/*
 * Created by SharpDevelop.
 * User: Шелли
 * Date: 29.10.2018
 * Time: 9:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using ParserQueueRunner.Model;
using RazorEngine;
using RazorEngine.Templating;

namespace ParserQueueRunner
{
	class Program
	{
		public static int Main(string[] args)
		{
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                // RazorEngine cannot clean up from the default appdomain...
                Console.WriteLine("Switching to second AppDomain, for RazorEngine...");
                AppDomainSetup adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                var current = AppDomain.CurrentDomain;
                // You only need to add strongnames when your appdomain is not a full trust environment.
                var strongNames = new StrongName[0];

                var domain = AppDomain.CreateDomain(
                    "MyMainDomain", null,
                    current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
                    strongNames);
                var exitCode = domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
                // RazorEngine will cleanup. 
                AppDomain.Unload(domain);
                return exitCode;
            }

            Console.WriteLine("-- ParserQueueRunner, ver 0.2 --");
            Console.WriteLine("Starting work.");

            // Проверка работы Excel Addin "Parser"
            //string res = CheckExcelAddin();

            string EXE = Assembly.GetExecutingAssembly().GetName().Name;
            string startupPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string iniFullPath = Path.Combine(startupPath, EXE + ".ini");

            ParserWebQueueParameters parserWebQueueParameters = new ParserWebQueueParameters();
            EmailSenderConfig senderConfig = new EmailSenderConfig();
            WebParserConfig parserConfig = new WebParserConfig();
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
                parserWebQueueParameters.WebServiceUrl = iniReader.GetValue("WebServiceUrl", "QueueWebService");
                parserWebQueueParameters.Method = "Get";
                parserWebQueueParameters.Timeout = iniReader.GetIntValue("Timeout", "QueueWebService", 20000);
                parserWebQueueParameters.ContentType = "application/json";

                // Email parameters
                senderConfig.host = iniReader.GetValue("Host", "EmailSender");
                senderConfig.port = iniReader.GetIntValue("Port", "EmailSender");
                senderConfig.enableSsl = iniReader.GetBoolValue("EnableSsl", "EmailSender");
                senderConfig.username = iniReader.GetValue("UserName", "EmailSender");
                senderConfig.password = iniReader.GetValue("Password", "EmailSender");
                senderConfig.usernameAlias = iniReader.GetValue("UserNameAlias", "EmailSender");

                // Excel Addin Parser
                parserConfig.AddinConfigName = iniReader.GetValue("ParserConfigName", "ExcelAddinParser");
                parserConfig.DealNumberColumn = iniReader.GetValue("DealNumberColumn", "ExcelAddinParser");
                parserConfig.IsTrackColumn = iniReader.GetValue("IsTrackColumn", "ExcelAddinParser");
                parserConfig.StartRowNumber = iniReader.GetIntValue("StartRowNumber", "ExcelAddinParser", 2);
                parserConfig.ResultNumberColumn = iniReader.GetValue("ResultNumberColumn", "ExcelAddinParser");
                parserConfig.DealNumberHyperlinkColumn = iniReader.GetValue("DealNumberHyperlinkColumn", "ExcelAddinParser");
                parserConfig.DocumentPdfFolderNameColumn = iniReader.GetValue("DocumentPdfFolderNameColumn", "ExcelAddinParser");
                parserConfig.DocumentPdfUrlColumn = iniReader.GetValue("DocumentPdfUrlColumn", "ExcelAddinParser");
                parserConfig.LastDealDateColumn = iniReader.GetValue("LastDealDateColumn", "ExcelAddinParser");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Can't find ini file at {iniFullPath}.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error readind ini file: {ex.GetType()}");
                return 0;
            }

            while (nElementsToProcess-- > 0)
            {
                int cnt = processQueue(parserWebQueueParameters, senderConfig, parserConfig);
                if (cnt == 0)
                    break;
            }

            Console.WriteLine("Work finished. Press any key.");
            //Console.ReadKey();

            return 0;
        }

        /// <summary>
        /// Проверка работы ExcelAddin
        /// </summary>
        /// <returns></returns>
        private static string CheckExcelAddin()
        {
            IWebPageParser webParser = new ExcelAddinWebPageParser();
            WebParserConfig parserConfig = new WebParserConfig()
            {
                AddinConfigName = ""
            };
            var parserResult = webParser.ParserCheck(parserConfig);
            return parserResult.ParserStatus +
                (
                string.IsNullOrEmpty(parserResult.ParserError) ? "" :
                ": " + parserResult.ParserError
                );
        }

        /// <summary>
        /// Получить данные из kad.ru по номеру документа
        /// </summary>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        private static WebParserResult ParseDocument(WebParserConfig parserConfig, string documentNumber)
        {
            IWebPageParser webParser = new ExcelAddinWebPageParser();

            WebParserParams parserParams = new WebParserParams()
            {
                DocumentNumber = documentNumber
            };

            var parserResult = webParser.ParseWebSite(parserConfig, parserParams);

            return parserResult;
        }

        /// <summary>
        /// Обработка элемента очереди ParserQueue
        /// </summary>
        /// <returns>Кол-во обработанных элементов очереди</returns>
        public static int processQueue(ParserWebQueueParameters parserWebQueueParameters, EmailSenderConfig emailSenderConfig,
            WebParserConfig webParserConfig)
        {
            // Создание обработчика очереди
            IParserWebQueue parserWebQueue = new OnlineParserWebQueue(parserWebQueueParameters);
            ParserQueueElement elt = null;

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
                parserWebQueue.SetQueueElementStatus(elt.ParserQueueId, 2);

                // Запустить web-parser
                Console.WriteLine("Start parsing website for Document Number: {0}", elt.ClientDocNum);
                WebParserResult parserResult = ParseDocument(webParserConfig, elt.ClientDocNum);

                Console.WriteLine("parser result: {0}. Last date: {1}, pdf link: {2}",
                    parserResult.ParserStatus,
                    parserResult.LastDealDate,
                    parserResult.CardUrl);

                if (parserResult.ParserStatus != "Ok")
                {
                    Console.WriteLine("Error: {0}", parserResult.ParserError);
                }

                // Отправка Email
                string fileName = parserResult.DocumentPfdPath;
                string emailTo = elt.ClientEmail;
                string docNumber = elt.ClientDocNum;
                DateTime requestDate = elt.CreatedTimeUtc;
                sendEmailByNewInterface(emailSenderConfig, fileName, emailTo, docNumber, requestDate, parserResult);

                printParserQueueElement(elt);

                // Пометка элемента очереди как успешно обработанный
                parserWebQueue.SetQueueElementStatus(elt.ParserQueueId, 3);

                return 1;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: {0}", exc.Message);

                // Установить статус "Ошибка обработки"
                if (elt != null)
                {
                    parserWebQueue.SetQueueElementStatus(elt.ParserQueueId, 4, exc.Message);
                }

            }
            return 0;

        }

        static void printParserQueueElement(ParserQueueElement elt)
		{
			Console.WriteLine("[Queue Element]: QueueId={0}, DocNumber={1}, Email={2}, CreatedTimeUtc={3}",
			                  elt.ParserQueueId,
			                  elt.ClientDocNum,
			                  elt.ClientEmail,
			                  elt.CreatedTimeUtc);
		}

        /// <summary>
        /// Составление и отправка Email.
        /// Что нужно подготовить:
        /// 0. Настройка для отправки письма:
        /// - параметры почтового сервера (host, port, ssl)
        /// - данные отправителя (username, password)
        /// - email отправителя (может быть равен username по умолчанию)
        /// 1. Имена файлов для вложения в письмо
        /// 2. Адрес получателя
        /// 3. Тема письма
        /// 4. Данные для тела письма (Номер документа, дата дела...)
        /// </summary>
        static void sendEmailByNewInterface(EmailSenderConfig senderConfig, string attachFileName, string AddressTo, string DocNumber, DateTime RequestDate, WebParserResult parserResult)
        {
            // Тема письма
            string mailSubject = $"--==Документ {DocNumber} от {RequestDate} ==--";

            // Тело письма
            string mailBodyText = ComposeMailTextBody(DocNumber, parserResult);
            string mailBodyHtml = ComposeMailHtmlBody(DocNumber, parserResult);

            EmailParameters emailParameters = new EmailParameters()
            {
                message = new EmailMessageParameters()
                {
                    AddressFrom = senderConfig.username,
                    AddressFromAlias = senderConfig.usernameAlias,
                    AddressTo = AddressTo,
                    Subject = mailSubject,
                    BodyText = mailBodyText,
                    BodyHtml = mailBodyHtml
                }
            };

            if (!string.IsNullOrEmpty(attachFileName))
            {
                emailParameters.attachments = new List<EmailAttachmentParameters>()
                {
                    new EmailAttachmentParameters()
                    {
                        FilePath = attachFileName,
                        FileName = Path.GetFileName(attachFileName),
                        MediaType = MediaTypeNames.Application.Pdf
                    }
                };
            }

            IEmailSender emailSender = new OnlineEmailSender(senderConfig);
            IEmailComposer emailComposer = new OnlineEmailComposer(emailParameters, emailSender);
            emailComposer.ComposeAndSendEmail();
        }

        /// <summary>
        /// Составитель тела письма (формат Html)
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="parserResult"></param>
        /// <returns>Строка для письма</returns>
        private static string ComposeMailHtmlBody(string docNumber, WebParserResult parserResult)
        {
            string htmlTemplateRelativePath = "MailTemplate\\HtmlTemplate.html";
            string startupPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string htmlTemplateFullPath = Path.Combine(startupPath, htmlTemplateRelativePath);

            string parseMessage = "";
            switch (parserResult.ParserStatus)
            {
                case "Not found":
                    parseMessage = "Документ не найден. Попробуйте указать другой номер дела.";
                    break;
                case "Error":
                    parseMessage = "Ошибка получения информации.<br>" + parserResult.ParserError;
                    break;
                case "Ok":
                    parseMessage = "Результат:";
                    break;
                default:
                    parseMessage = "Ошибка составления результата.";
                    break;
            }

            string template = File.ReadAllText(htmlTemplateFullPath);
            string result = Engine.Razor.RunCompile( template, "htmlTemplateKey", null,
                new {
                    DocNumber = docNumber,
                    Result = parseMessage,
                    LastDealDate = parserResult.LastDealDate.ToShortDateString(),
                    CardUrl = parserResult.CardUrl,
                    DocumentPdfUrl = parserResult.DocumentPdfUrl,
                    ParserStatus = parserResult.ParserStatus
                });

            return result;
        }

        /// <summary>
        /// Составитель тела письма (формат Pain Text)
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="parserResult"></param>
        /// <returns>Строка для письма</returns>
        private static string ComposeMailTextBody(string docNumber, WebParserResult parserResult)
        {
            string textTemplateRelativePath = "MailTemplate\\TextTemplate.html";
            string startupPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string textTemplateFullPath = Path.Combine(startupPath, textTemplateRelativePath);

            string parseMessage = "";
            switch (parserResult.ParserStatus)
            {
                case "Not found":
                    parseMessage = "Документ не найден. Попробуйте указать другой номер дела.";
                    break;
                case "Error":
                    parseMessage = "Ошибка получения информации.<br>" + parserResult.ParserError;
                    break;
                case "Ok":
                    parseMessage = "Результат:";
                    break;
                default:
                    parseMessage = "Ошибка составления результата.";
                    break;
            }

            string template = File.ReadAllText(textTemplateFullPath);
            string result = Engine.Razor.RunCompile(template, "textTemplateKey", null,
                new
                {
                    DocNumber = docNumber,
                    Result = parseMessage,
                    LastDealDate = parserResult.LastDealDate.ToShortDateString(),
                    CardUrl = parserResult.CardUrl,
                    DocumentPdfUrl = parserResult.DocumentPdfUrl,
                    ParserStatus = parserResult.ParserStatus
                });

            return result;

        }

        /// <summary>
        /// Составитель тела письма (формат Pain Text)
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="parserResult"></param>
        /// <returns>Строка для письма</returns>
        private static string ComposeMailTextBodyOld(string docNumber, WebParserResult parserResult)
        {
            StringBuilder sbMessageText = new StringBuilder($"Информация по документу № {docNumber}.\r\n");
            sbMessageText.AppendLine("----------------------------\r\n");

            if (parserResult.ParserStatus == "Not found")
            {
                sbMessageText.AppendLine("Документ не найден. Попробуйте указать другой номер документа.");
            }
            else if (parserResult.ParserStatus == "Error")
            {
                sbMessageText.AppendLine("Ошибка получения информации.\r\n");
                sbMessageText.AppendLine(parserResult.ParserError);
            }
            else if (parserResult.ParserStatus == "Ok")
            {
                sbMessageText.AppendLine("Информация найдена.");
                sbMessageText.AppendLine($"Последняя информация по делу от {parserResult.LastDealDate}.");
                sbMessageText.AppendLine($"Ссылка на дело в картотеке: {parserResult.CardUrl}.");
                sbMessageText.AppendLine($"Ссылка на документ: {parserResult.DocumentPdfUrl}.");
            }
            else
            {
                sbMessageText.AppendLine($"Ошибка: неизвестный статус обработки: {parserResult.ParserStatus}");
            }

            return sbMessageText.ToString();
        }
    }
}