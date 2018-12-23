using ParserQueueRunner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ParserQueueRunner
{
    public class OnlineEmailComposer : IEmailComposer
    {
        EmailParameters _emailParameters;
        IEmailSender _emailSender;

        public OnlineEmailComposer(EmailParameters emailParameters, IEmailSender emailSender)
        {
            _emailParameters = emailParameters;
            _emailSender = emailSender;
        }

        public void ComposeAndSendEmail()
        {
            try
            {
                var messageParams = _emailParameters.message;
                var firstAttachmentParams = _emailParameters.attachments?.FirstOrDefault();
                using (var message = new MailMessage(messageParams.AddressFrom, messageParams.AddressTo))
                {
                    message.SubjectEncoding = Encoding.UTF8;
                    message.Subject = messageParams.Subject;
                    message.BodyEncoding = Encoding.UTF8;
                    message.Body = messageParams.BodyText;
                    //message.IsBodyHtml = false;
                    
                    // ToDo: использовать messageParams
                    message.From = new MailAddress("savosin_sergey@mail.ru", "Тестирование ParserQueueRunner", Encoding.UTF8);

                    // Добавление html тела письма
                    ContentType mimeType = new ContentType("text/html");
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(messageParams.BodyHtml, mimeType);
                    message.AlternateViews.Add(htmlView);

                    // Добавление одного вложения
                    if (firstAttachmentParams != null && 1==0)
                    {
                        Attachment data = new Attachment(firstAttachmentParams.FilePath, firstAttachmentParams.MediaType);
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.FileName = firstAttachmentParams.FileName;
                        message.Attachments.Add(data);
                    }

                    _emailSender.SendEmail(message);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Compose email error: " + exc.Message);
                throw exc;
            }

        }
    }
}
