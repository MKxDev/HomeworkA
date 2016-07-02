using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Models;
using API.Services;

namespace API.Controllers
{
    public class StopsController : ApiController
    {
        [Route("stops/{stopNumber}/arrivals")]
        public IEnumerable<object> GetArrivals(int stopNumber, string time = null)
        {
            Console.WriteLine($"Calculating arrivals for Stop {stopNumber} at {time}");

            var stopsService = new StopsService();

            TimeSpan arrivalTime;
            
            // Default to server's "Now" time
            if (!String.IsNullOrWhiteSpace(time))
            {
                arrivalTime = TimeSpan.Parse(time);
            }
            else
            {
                arrivalTime = DateTime.Now.TimeOfDay;
            }
            
            // Get arrival info and return to the user
            IEnumerable<ArrivalModel> arrivals = stopsService.GetClosestArrivalTimes(stopNumber, arrivalTime);

            return arrivals;
        }
    }
}
