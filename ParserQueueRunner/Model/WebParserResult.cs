using System;

namespace ParserQueueRunner.Model
{
    public class WebParserResult
    {
        public string ParserStatus { get; set; }
        public string ParserError { get; set; }
        public DateTime LastDealDate { get; set; }
        public string CardUrl { get; set; }
        public bool HasAttachment { get; set; }
    }
}
