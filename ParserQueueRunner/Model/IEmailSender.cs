using System.Net.Mail;

namespace ParserQueueRunner.Model
{
    public interface IEmailSender
    {
        void SendEmail(MailMessage mailMessage);
    }
}
