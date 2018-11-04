/*
 * Created by SharpDevelop.
 * User: Шелли
 * Date: 29.10.2018
 * Time: 9:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using ParserQueueRunner.Model;

namespace ParserQueueRunner
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
			processQueue();
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		
		public static void processQueue()
		{
			setQueueElementAsProcessed(1);
			return;
			
			var elt = getNewQueueElement_web();
			printParserQueueElement(elt);
			
		}
		
		static void printParserQueueElement(ParserQueueElement elt)
		{
			Console.WriteLine("Queue Element: QueueId={0}, DocNumber={1}, Email={2}, CreatedTime={3}",
			                  elt.ParserQueueId,
			                  elt.ClientDocNum,
			                  elt.ClientEmail,
			                  elt.CreatedTime);
		}
		
		static ParserQueueElement getNewQueueElement_Test()
		{
			var elt = new ParserQueueElement()
			{
				ParserQueueId = 1,
				ClientDocNum = "doc#1",
				ClientEmail = "test@mail.ru",
				CreatedTime = DateTime.Now
			};
			
			return elt;
		}
		
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
					Console.WriteLine("Response status = " + ((HttpWebResponse)response).StatusDescription);
					
					using (Stream dataStream = response.GetResponseStream())
					{
						using (StreamReader reader = new StreamReader(dataStream))
						{
							responseFromServer = reader.ReadToEnd();
							Console.WriteLine(responseFromServer);
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
					//s.Write(byteArray, 0, byteArray.Length);
					//s.Close();
					
					using (var sw = new StreamWriter(s))
					{
						sw.Write(postDataJson);
					}
				}
				
				using (WebResponse response = request.GetResponse())
				{
					Console.WriteLine("Response status = " + ((HttpWebResponse)response).StatusDescription);
					
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
	}
}