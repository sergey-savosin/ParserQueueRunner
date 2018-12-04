using ParserQueueRunner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ParserQueueRunner
{
    public class OnlineEmailSender : IEmailSender
    {
        EmailSenderConfig _emailSenderConfig;

        public OnlineEmailSender(EmailSenderConfig emailSenderConfig)
        {
            _emailSenderConfig = emailSenderConfig;
        }

        public void SendEmail(MailMessage mailMessage)
        {
            using (var sc = new SmtpClient())
            {
                sc.Host = _emailSenderConfig.host;
                sc.Port = _emailSenderConfig.port;
                sc.Credentials = new NetworkCredential(_emailSenderConfig.username, _emailSenderConfig.password);
                sc.EnableSsl = _emailSenderConfig.enableSsl;

                sc.Send(mailMessage);
            }
        }
    }
}
