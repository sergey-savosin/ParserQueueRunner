using Newtonsoft.Json;
using ParserQueueRunner.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParserQueueRunner
{
    public class OnlineParserWebQueue : IParserWebQueue
    {
        readonly string WEBSERVICE_URL;
        readonly string _method;
        readonly int _timeout;
		readonly string _contentType;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parameters"></param>
        public OnlineParserWebQueue(ParserWebQueueParameters parameters)
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
        /// <returns>ParserQueueElement</returns>
        public ParserQueueElement GetNewElement()
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

                ParserQueueElement el = JsonConvert.DeserializeObject<ParserQueueElement>(responseFromServer);
                return el;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return new ParserQueueElement();

        }

        /// <summary>
        /// Пометить статус элемента очереди
        /// </summary>
        /// <param name="ParserQueueId"></param>
        /// <param name="StatusId">
        /// 1: Новый
        /// 2: В обработке
        /// 3: Успешно обработан
        /// 4: Ошибка обработки
        /// </param>
        public void SetQueueElementStatus(int ParserQueueId, int StatusId, string ErrorMessage = "")
        {
            string responseFromServer;
            SetQueueElementStatusRequest postData = new SetQueueElementStatusRequest()
            {
                queuestatusid = StatusId.ToString(),
                errormessage = ErrorMessage
            };

            //string postDataJson = "{ \"queuestatusid\" : \"" + StatusId.ToString() + "\" }";
            string postDataJson = JsonConvert.SerializeObject(postData);

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
    }
}
