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
			Console.WriteLine("Hello, World!");

            string res = CheckExcelAddin();
            Console.WriteLine("Excel addin check result: {0}", res);

            if (!(res.Equals("Ok")))
            {
                return;
            }

            int i = 3;
            while (i-- > 0)
            {
                int cnt = processQueue();
                if (cnt == 0)
                    break;
            }
        }

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


                // Отправка Email
                string fileName = @"C:\work\assembler\modern-x86-assembly-language-programming-master\9781484200650_AppC.pdf";
                string emailTo = "savortone@yandex.ru";
                string docNumber = elt.ClientDocNum + " at " + elt.ClientEmail;
                DateTime requestDate = elt.CreatedTimeUtc;
                sendEmailByNewInterface(fileName, emailTo, docNumber, requestDate);

                printParserQueueElement(elt);

                // Пометка элемента очереди как успешно обработанный
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

        //static ParserQueueElement getNewQueueElement_Test()
        //{
        //	var elt = new ParserQueueElement()
        //	{
        //		ParserQueueId = 1,
        //		ClientDocNum = "doc#1",
        //		ClientEmail = "test@mail.ru",
        //		CreatedTimeUtc = DateTime.Now
        //	};

        //	return elt;
        //}

        /// <summary>
        /// Получить элемент очереди
        /// </summary>
        /// <returns>ParserQueueElement</returns>
        static ParserQueueElement getNewQueueElement_web()
		{
            ParserWebQueueParameters parserWebQueueParameters = new ParserWebQueueParameters()
            {
                WebServiceUrl = "https://vprofy.ru/parserqueue/parserqueueendpoint.php",
                Method = "Get",
                Timeout = 20000,
                ContentType = "application/json"
            };

            IParserWebQueue parserWebQueue = new OnlineParserWebQueue(parserWebQueueParameters);
            return parserWebQueue.GetNewElement();

		}
		
		static void setQueueElementAsProcessed(int ParserQueueId)
		{
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
        static void sendEmailByNewInterface(string attachFileName, string AddressTo, string DocNumber, DateTime RequestDate)
        {
            EmailSenderConfig senderConfig = new EmailSenderConfig()
            {
                host = "smtp.mail.ru",
                port = 587,
                enableSsl = true,
                username = "savosin_sergey@mail.ru",
                password = ""
            };

            string mailSubject = $"--==Документ {DocNumber} от {RequestDate} ==--";
            EmailParameters emailParameters = new EmailParameters()
            {
                attachments = new List<EmailAttachmentParameters>()
                {
                    new EmailAttachmentParameters()
                    {
                        FilePath = attachFileName,
                        FileName = Path.GetFileName(attachFileName),
                        MediaType = MediaTypeNames.Application.Pdf
                    }
                },
                message = new EmailMessageParameters()
                {
                    AddressFrom = "savosin_sergey@mail.ru",
                    AddressTo = "savortone@yandex.ru",
                    Subject = mailSubject,
                    BodyText = "--==<b> Тело письма </b>==--"
                }
            };

            IEmailSender emailSender = new OnlineEmailSender(senderConfig);
            IEmailComposer emailComposer = new OnlineEmailComposer(emailParameters, emailSender);
            emailComposer.ComposeAndSendEmail();
        }
	}
}