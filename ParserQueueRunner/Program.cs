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
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;
using ParserQueueRunner.Model;

namespace ParserQueueRunner
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Starting work.");

            //string res = CheckExcelAddin();
            //Console.WriteLine("Excel addin check result: {0}", res);

            //if (!(res.Equals("Ok")))
            //{
            //    return;
            //}

            int i = 3;
            while (i-- > 0)
            {
                int cnt = processQueue();
                if (cnt == 0)
                    break;
            }

            Console.WriteLine("Work finished. Press any key.");
            Console.ReadKey();
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
                AddinPath = webParser.GetParserPath(),
                AddinConfigName = "",
                AddinWorkbookTemplateFile = ""
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
        private static WebParserResult ParseDocument(string documentNumber)
        {
            const string parserConfigName = "Kad.arbitr.ru_r78_doc_05.11.2018";

            IWebPageParser webParser = new ExcelAddinWebPageParser();
            WebParserConfig parserConfig = new WebParserConfig()
            {
                AddinPath = webParser.GetParserPath(),
                AddinConfigName = parserConfigName,
                AddinWorkbookTemplateFile = ""
            };
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
        public static int processQueue()
        {
            try
            {
                // Создание обработчика очереди
                ParserWebQueueParameters parserWebQueueParameters = new ParserWebQueueParameters()
                {
                    WebServiceUrl = "https://vprofy.ru/parserqueue/parserqueueendpoint.php",
                    Method = "Get",
                    Timeout = 20000,
                    ContentType = "application/json"
                };

                IParserWebQueue parserWebQueue = new OnlineParserWebQueue(parserWebQueueParameters);

                // Получение элемента очереди
                var elt = parserWebQueue.GetNewElement();

                if (elt == null)
                {
                    Console.WriteLine("No new elements to process.");
                    return 0;
                }

                // Запустить web-parser
                Console.WriteLine("Start parsing website for Document Number: {0}", elt.ClientDocNum);
                WebParserResult parserResult = ParseDocument(elt.ClientDocNum);

                Console.WriteLine("parser result: {0}. Last date: {1}, pdf link: {2}",
                    parserResult.ParserStatus,
                    parserResult.LastDealDate,
                    parserResult.CardUrl);

                if (parserResult.ParserStatus != "Ok")
                {
                    Console.WriteLine("Error: {0}", parserResult.ParserError);
                }

                // Отправка Email
                // ToDo: emailTo брать из очереди
                // ToDo: файл вложения брать из результата запуска веб-парсера
                string fileName = parserResult.DocumentPfdPath;// @"C:\work\assembler\modern-x86-assembly-language-programming-master\9781484200650_AppC.pdf";
                string emailTo = "savortone@yandex.ru";
                string docNumber = elt.ClientDocNum;
                DateTime requestDate = elt.CreatedTimeUtc;
                sendEmailByNewInterface(fileName, emailTo, docNumber, requestDate, parserResult);

                printParserQueueElement(elt);

                // Пометка элемента очереди как успешно обработанный
                // ToDo: для ошибок указывать другой статус
                parserWebQueue.SetQueueElementAsProcessed(elt.ParserQueueId);

                return 1;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: {0}", exc.Message);
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
        static void sendEmailByNewInterface(string attachFileName, string AddressTo, string DocNumber, DateTime RequestDate, WebParserResult parserResult)
        {
            EmailSenderConfig senderConfig = new EmailSenderConfig()
            {
                host = "smtp.mail.ru",
                port = 587,
                enableSsl = true,
                username = "---",
                password = "---"
            };

            // Тема письма
            string mailSubject = $"--==Документ {DocNumber} от {RequestDate} ==--";

            // Тело письма

            string mailBody = ComposeMailTextBody(DocNumber, parserResult);

            EmailParameters emailParameters = new EmailParameters()
            {
                message = new EmailMessageParameters()
                {
                    AddressFrom = "savosin_sergey@mail.ru",
                    AddressTo = "savortone@yandex.ru",
                    Subject = mailSubject,
                    BodyText = mailBody
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
        /// Составитель тела письма (формат Pain Text)
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="parserResult"></param>
        /// <returns>Строка для письма</returns>
        private static string ComposeMailTextBody(string docNumber, WebParserResult parserResult)
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