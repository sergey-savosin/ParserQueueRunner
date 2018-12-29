using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserQueueRunner.Model
{
    public class WebParserConfig
    {
        public string AddinConfigName { get; set; }
        public string DealNumberColumn { get; set; }
        public string IsTrackColumn { get; set; }
        public int StartRowNumber { get; set; }
        public string ParserConfigName { get; set; }

        public string ResultNumberColumn { get; set; }
        public string DealNumberHyperlinkColumn { get; set; }
        public string DocumentPdfFolderNameColumn { get; set; }
        public string DocumentPdfUrlColumn { get; set; }
        public string LastDealDateColumn { get; set; }
    }
}
