using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerTrack.API.Models
{
    public class ServerHistory
    {
        public string ServerName { get; set; }
        public DateTime Segment  { get; set; }
        public double CPUAverage { get; set; }
        public double RAMAverage { get; set; }
    }
}