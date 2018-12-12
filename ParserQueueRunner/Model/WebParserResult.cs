using System;

namespace ParserQueueRunner.Model
{
    public class WebParserResult
    {
        public string ParserStatus { get; set; }
        public string ParserError { get; set; }
        public DateTime LastDealDate { get; set; }
        public string CardUrl { get; set; }
        public string DocumentPdfUrl { get; set; }
        public string DocumentPdfFolderName { get; set; }
        public string DocumentPfdPath { get; set; }
        public bool HasAttachment { get; set; }
    }
}
