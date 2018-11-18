using System.Collections.Generic;

namespace ParserQueueRunner.Model
{
    public class EmailParameters
    {
        public EmailMessageParameters message;
        public List<EmailAttachmentParameters> attachments;
    }
}