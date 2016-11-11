using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerTrack.API.Models
{
    [Serializable]
    public class ServerLoad
    {
        public string ServerName { get; set; }
        public double CPULoad { get; set;}
        public double RAMLoad { get; set; }
        public DateTime UTCLogDate { get; set; }

    }
}