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

            sendEmailByNewInterface();
            Console.ReadKey();
            return;

            int i = 3;
            while (i-- > 0)
            {
                int cnt = processQueue();
                if (cnt == 0)
                    break;
            }
        }

        /// <summary>
        /// Обработка элемента очереди ParserQueue
        /// </summary>
        /// <returns>Кол-во обработанных элементов очереди</returns>
        public static int processQueue()
		{
			var elt = getNewQueueElement_web();

            if (elt == null)
            {
                Console.WriteLine("No new elements to process.");
                return 0;
            }

            try
            {
                printParserQueueElement(elt);
                setQueueElementAsProcessed(elt.ParserQueueId);
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
		
		static ParserQueueElement getNewQueueElement_web()
		{
			const string WEBSERVICE_URL = "https://vprofy.ru/parserqueue/parserqueueendpoint.php";
			string responseFromServer = "";
			
			try
			{
				WebRequest request = WebRequest.Create(WEBSERVICE_URL);
				request.Method = "Get";
				request.Timeout = 20000;
				request.ContentType = "application/json";
				
				using (WebResponse response = request.GetResponse())
				{
					Console.WriteLine("[Get] Response status = " + ((HttpWebResponse)response).StatusDescription);
					
					using (Stream dataStream = response.GetResponseStream())
					{
						using (StreamReader reader = new StreamReader(dataStream))
						{
							responseFromServer = reader.ReadToEnd();
							Console.WriteLine("[Get response]\r\n" + responseFromServer);
						}
					}
				}

				ParserQueueElement el = JsonConvert.DeserializeObject<ParserQueueElement>(responseFromServer);				
				return el;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
			}
			
			return new ParserQueueElement();
		}
		
		static void setQueueElementAsProcessed(int ParserQueueId)
		{
			const string WEBSERVICE_URL = "https://vprofy.ru/parserqueue/parserqueueendpoint.php";
			string responseFromServer = "";
			string postDataJson = "{ \"queuestatusid\" : \"2\" }";
			try
			{
				string strPutUrl = WEBSERVICE_URL + "/parserqueue/" + ParserQueueId;
				byte[] byteArray = Encoding.UTF8.GetBytes(postDataJson);
				
				WebRequest request = WebRequest.Create(strPutUrl);
				request.Method = "Put";
				request.Timeout = 20000;
				request.ContentType = "application/json";
				request.ContentLength = byteArray.Length;
				
				using (var s = request.GetRequestStream())
				{
					using (var sw = new StreamWriter(s))
					{
						sw.Write(postDataJson);
					}
				}
				
				using (WebResponse response = request.GetResponse())
				{
					Console.WriteLine("[Put] Response status = " + ((HttpWebResponse)response).StatusDescription);
					
					using (Stream dataStream = response.GetResponseStream())
					{
						using (StreamReader reader = new StreamReader(dataStream))
						{
							responseFromServer = reader.ReadToEnd();
							Console.WriteLine(responseFromServer);
						}
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
			}
		}

        static void sendEmailByNewInterface()
        {
            string fileName = @"C:\work\assembler\modern-x86-assembly-language-programming-master\9781484200650_AppC.pdf";

            EmailSenderConfig senderConfig = new EmailSenderConfig()
            {
                host = "smtp.mail.ru",
                port = 587,
                enableSsl = true,
                username = "savosin_sergey@mail.ru",
                pasword = "glassma0-"
            };

            EmailParameters emailParameters = new EmailParameters()
            {
                attachments = new List<EmailAttachmentParameters>()
                {
                    new EmailAttachmentParameters()
                    {
                        FilePath = fileName,
                        FileName = Path.GetFileName(fileName),
                        MediaType = MediaTypeNames.Application.Pdf
                    }
                },
                message = new EmailMessageParameters()
                {
                    AddressFrom = "savosin_sergey@mail.ru",
                    AddressTo = "savortone@yandex.ru",
                    Subject = "--==Тема письма==--",
                    BodyText = "--==<b> Тело письма </b>==--"
                }
            };

            IEmailSender emailSender = new OnlineEmailSender(senderConfig);
            IEmailComposer emailComposer = new OnlineEmailComposer(emailParameters, emailSender);
            emailComposer.ComposeAndSendEmail();
        }
	}
}