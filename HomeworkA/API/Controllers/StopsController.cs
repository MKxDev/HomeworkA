using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using API.Models;
using API.Services;

namespace API.Controllers
{
    public class StopsController : ApiController
    {
        [Route("stops/{stopNumber}/arrivals")]
        public IEnumerable<object> GetArrivals(int stopNumber, string time = null)
        {
            var stopsService = new StopsService();

            TimeSpan arrivalTime;

            if (!String.IsNullOrWhiteSpace(time))
            {
                arrivalTime = TimeSpan.Parse(time);
            }
            else
            {
                arrivalTime = DateTime.Now.TimeOfDay;
            }
            
            IEnumerable<ArrivalModel> arrivals = stopsService.GetClosestArrivalTimes(stopNumber, arrivalTime);

            return arrivals;
        }
    }
}
