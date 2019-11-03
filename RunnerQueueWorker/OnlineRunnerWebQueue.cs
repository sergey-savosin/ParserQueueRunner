using Newtonsoft.Json;
using RunnerQueueWorker.Model;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace RunnerQueueWorker
{
    public class OnlineRunnerWebQueue : IRunnerWebQueue
    {
        readonly string WEBSERVICE_URL;
        readonly string _method;
        readonly int _timeout;
		readonly string _contentType;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parameters"></param>
        public OnlineRunnerWebQueue(RunnerWebQueueParameters parameters)
        {
            WEBSERVICE_URL = parameters.WebServiceUrl;
            _method = parameters.Method;
            _timeout = parameters.Timeout;
            _contentType = parameters.ContentType;
        }

        /// <summary>
        /// Возращает новый элемент очереди для обработки.
        /// Если очередь пуста, возвращает null.
        /// </summary>
        /// <returns>RunnerQueueElement</returns>
        public RunnerQueueElement GetNewElement()
        {
            string responseFromServer;

            try
            {
                WebRequest request = WebRequest.Create(WEBSERVICE_URL);
                request.Method = _method;
                request.Timeout = _timeout;
                request.ContentType = _contentType;

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

                RunnerQueueElement el = JsonConvert.DeserializeObject<RunnerQueueElement>(responseFromServer);
                return el;
            }
			catch (WebException ex)
			{
				var reader = new StreamReader(ex.Response.GetResponseStream());
				var content = reader.ReadToEnd();

				Console.WriteLine("Web error: " + content.ToString());
				throw ex;
			}

			catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return new RunnerQueueElement();

        }

        /// <summary>
        /// Пометить статус элемента очереди
        /// </summary>
        /// <param name="RunnerQueueId"></param>
        /// <param name="queueStatus">
        /// 1: Новый
        /// 2: В обработке
        /// 3: Успешно обработан
        /// 4: Ошибка обработки
        /// </param>
        public void SetQueueElementStatus(int RunnerQueueId, QueueStatus queueStatus, string ErrorMessage = "")
        {
            string responseFromServer;
            SetQueueElementStatusRequest postData = new SetQueueElementStatusRequest()
            {
                queuestatusid = ((int)queueStatus).ToString(),
                errormessage = ErrorMessage
            };

            string postDataJson = JsonConvert.SerializeObject(postData);

            try
            {
                string strPutUrl = WEBSERVICE_URL + "/runnerqueue/" + RunnerQueueId;
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
            catch (WebException ex)
            {
                var reader = new StreamReader(ex.Response.GetResponseStream());
                var content = reader.ReadToEnd();

                Console.WriteLine("Web error: " + content.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
				throw ex;
            }

        }
    }
}
