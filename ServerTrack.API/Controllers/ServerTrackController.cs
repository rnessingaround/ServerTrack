using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using ServerTrack.API.Models;


namespace ServerTrack.API.Controllers
{
    [RoutePrefix("servertrack")]
    public class ServerTrackController : ApiController
    {
            

        /// <summary>
        /// Strictly for loading test data to evalutate the serverhistory calls
        /// This would be removed for a production environment
        /// </summary>
        /// <returns></returns>
        [Route("loaddata")]
        public IHttpActionResult GetLoadData()
        {
            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-one", CPULoad = 36, RAMLoad = 74, UTCLogDate = DateTime.UtcNow});
            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-one", CPULoad = 46, RAMLoad = 73, UTCLogDate = DateTime.UtcNow.AddHours(1).AddMinutes(1) });
            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-one", CPULoad = 56, RAMLoad = 72, UTCLogDate = DateTime.UtcNow.AddHours(2).AddMinutes(2) });
            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-one", CPULoad = 66, RAMLoad = 71, UTCLogDate = DateTime.UtcNow.AddHours(3).AddMinutes(3) });
            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-one", CPULoad = 76, RAMLoad = 70, UTCLogDate = DateTime.UtcNow.AddHours(4).AddMinutes(4) });

            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-two", CPULoad = 137, RAMLoad = 75, UTCLogDate = DateTime.UtcNow.AddMinutes(1) });
            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-two", CPULoad = 138, RAMLoad = 76, UTCLogDate = DateTime.UtcNow.AddMinutes(2) });
            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-two", CPULoad = 139, RAMLoad = 77, UTCLogDate = DateTime.UtcNow.AddMinutes(3) });
            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-two", CPULoad = 140, RAMLoad = 78, UTCLogDate = DateTime.UtcNow.AddMinutes(4) });
            LoadHistory.ServerLoadHistry.Add(new ServerLoad { ServerName = "server-two", CPULoad = 141, RAMLoad = 79, UTCLogDate = DateTime.UtcNow.AddMinutes(5) });
            
            return Ok();
        }

        /// <summary>
        /// Strictly for testing to return all entries for a server
        /// This should be removed for a production environment
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [Route("all/{servername}")]
        public IHttpActionResult GetAll(string serverName)
        {
            if (!string.IsNullOrEmpty(serverName))
            {
                return Ok(LoadHistory.ServerLoadHistry.Where(slh => slh.ServerName.ToLower() == serverName.Trim().ToLower()).ToList());
            }
            else
            {
                return new ResponseMessageResult(Request.CreateErrorResponse(HttpStatusCode.NotFound, "SERVER_NAME_NOT_FOUND"));
            }
        }

        /// <summary>
        /// Get the history for the server based on hours or minutes
        /// </summary>
        /// <param name="serverName">name of server</param>
        /// <param name="segment">time segment hour|h minute|m</param>
        /// <returns></returns>
        [Route("serverhistory/{servername}/{segment}")]
        public IHttpActionResult GetServerHistory(string serverName, string segment)
        {
            if (!string.IsNullOrEmpty(serverName))
            {
                if (segment.ToLower().StartsWith("m"))
                {
                    int numberOfMinutes = 60;

                    return Ok(LoadHistory.ServerLoadHistry.Where(slh => slh.ServerName.ToLower() == serverName.Trim().ToLower())
                        .OrderByDescending(slh => slh.UTCLogDate)
                        .GroupBy(g => new DateTime(g.UTCLogDate.Year, g.UTCLogDate.Month, g.UTCLogDate.Day, g.UTCLogDate.Hour, g.UTCLogDate.Minute, 0))
                        .Select(s => new ServerHistory { ServerName = serverName, Segment = s.Key, CPUAverage = s.Average(a => a.CPULoad), RAMAverage = s.Average(a => a.RAMLoad) })
                        .Take(numberOfMinutes).ToList());
                }
                else if (segment.ToLower().StartsWith("h"))
                {
                    int numberOfHours = 24;

                    return Ok(LoadHistory.ServerLoadHistry.Where(slh => slh.ServerName.ToLower() == serverName.Trim().ToLower())
                    .OrderByDescending(slh => slh.UTCLogDate)
                    .GroupBy(g => new DateTime(g.UTCLogDate.Year, g.UTCLogDate.Month, g.UTCLogDate.Day, g.UTCLogDate.Hour, 0, 0))
                    .Select(s => new ServerHistory { ServerName = serverName, Segment = s.Key, CPUAverage = s.Average(a => a.CPULoad), RAMAverage = s.Average(a => a.RAMLoad) })
                    .Take(numberOfHours).ToList());
                }
                else {
                    return new ResponseMessageResult(Request.CreateErrorResponse(HttpStatusCode.NotFound, "SEGMENT_NOT_FOUND"));
                }
            }
            else
            {
                return new ResponseMessageResult(Request.CreateErrorResponse(HttpStatusCode.NotFound, "SERVER_NAME_NOT_FOUND"));
            }
        }


        [Route("serverload")]
        public IHttpActionResult PostServerLoad(ServerLoad sl)
        {
            ResponseMessageResult rmr = null;

            if (IsDataValid(sl.ServerName, sl.CPULoad.ToString(), sl.RAMLoad.ToString(), out rmr))
            {
                return rmr;
            }

            AddServerHistory(sl.ServerName, sl.CPULoad, sl.RAMLoad);

            return Ok();
        }


        //This is for testing (use the browsers to populate test data) and should be removed for a production environment
        [Route("serverload/{servername}/{cpuLoad}/{ramLoad}")]
        public IHttpActionResult GetServerLoad(string serverName, string cpuLoad, string ramLoad)
        {
            ResponseMessageResult rmr = null;
            
            if (IsDataValid(serverName, cpuLoad, ramLoad, out rmr))
            {
                return rmr;
            }

            AddServerHistory(serverName, double.Parse(cpuLoad), double.Parse(ramLoad));

            return Ok();
        }

        private bool IsDataValid(string serverName, string cpuLoad, string ramLoad, out ResponseMessageResult responseMessageResult)
        {
            bool returnVal = true;
            string responseMessageResultMessage = string.Empty;

            if (String.IsNullOrEmpty(serverName))
            {
                returnVal = false;
                responseMessageResultMessage =  "SERVER_NAME_IS_EMPTY";
            }

            double outCPULoad = 0;
            if (!double.TryParse(cpuLoad, out outCPULoad))
            {
                returnVal = false;
                if (string.IsNullOrEmpty(responseMessageResultMessage))
                {
                    responseMessageResultMessage =  "CPU_LOAD_NOT_DOUBLE";
                }
                else
                {
                    responseMessageResultMessage = string.Format("{0},{1}",responseMessageResultMessage,"CPU_LOAD_NOT_DOUBLE");
                }
            }

            double outRAMLoad = 0;
            if (!double.TryParse(ramLoad, out outRAMLoad))
            {
                returnVal = false;
                if (string.IsNullOrEmpty(responseMessageResultMessage))
                {
                    responseMessageResultMessage = "CPU_LOAD_NOT_DOUBLE";
                }
                else
                {
                    responseMessageResultMessage = string.Format("{0},{1}", responseMessageResultMessage, "RAM_LOAD_NOT_DOUBLE");
                }
            }

            responseMessageResult = new ResponseMessageResult(Request.CreateErrorResponse(HttpStatusCode.NotFound, "responseMessageResultMessage"));

            return returnVal;
        }

        private void AddServerHistory(string serverName, double outCPULoad, double outRAMLoad)
        {

            ServerLoad sl = new ServerLoad();
            sl.ServerName = serverName;
            sl.CPULoad = outCPULoad;
            sl.RAMLoad = outRAMLoad;
            sl.UTCLogDate = DateTime.UtcNow;
            lock (LoadHistory.ServerLoadHistry)
            {
                LoadHistory.ServerLoadHistry.Add(sl);
            }
        }
    }
    
}
