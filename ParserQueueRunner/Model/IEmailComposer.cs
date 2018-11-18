namespace ParserQueueRunner.Model
{
    interface IEmailComposer
    {
        //void Create(EmailParameters emailParameters, IEmailSender emailSender);
        void ComposeAndSendEmail();
    }
}
