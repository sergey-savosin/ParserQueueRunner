using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserQueueRunner.Model
{
    public class SetQueueElementStatusRequest
    {
        public string queuestatusid { get; set; }
        public string errormessage { get; set; }
    }
}
