using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using API.Helpers;

namespace API.Services
{
    public class StopsService
    {
        public const int MaxStopCount = 10;
        public const int MaxRouteCount = 3;

        private const int StopServiceInterval = 15;
        private const int RouteStartInterval = 2;
        private const int StopDistanceInMinutes = 2;
        private const int MaxClosestArrivals = 2;
        
        /// <summary>
        /// Does basic validation and then calculates next arrival times for each route at the specified
        /// stop.
        /// </summary>
        /// <param name="stopNumber">The stop for which to return route arrival times.</param>
        /// <param name="atTime">The time, relative to which to return next arrival times.</param>
        /// <returns></returns>
        public IEnumerable<ArrivalModel> GetClosestArrivalTimes(int stopNumber, TimeSpan atTime)
        {
            // Basic validation
            if (stopNumber < 1 || stopNumber > MaxStopCount) throw new ArgumentOutOfRangeException("Stop must be within 1 and 10");
            
            var arrivals = new List<ArrivalModel>();

            // Get arrival info for each route
            foreach (var item in Enumerable.Range(1, MaxRouteCount))
            {
                var routeArrivals = GetNextArrivals(stopNumber, item, atTime);

                arrivals.Add(new ArrivalModel
                {
                    RouteName = $"Route {item}",
                    ArrivalTimes = routeArrivals.ToList()
                });
            }

            return arrivals;
        }

        /// <summary>
        /// Gets next arrival times relative to atTime.
        /// </summary>
        /// <param name="stop">The stop for which to return route arrival time info.</param>
        /// <param name="route">The route for which to return arrival time info.</param>
        /// <param name="atTime">Time, relative to which to return arrival info.</param>
        /// <returns></returns>
        private IEnumerable<TimeSpan> GetNextArrivals(int stop, int route, TimeSpan atTime)
        {
            // Get all arrival times for the stop/route combo
            var serviceTimes = GetArrivalTimes(stop, route);

            // Get the next closest arrival times
            var closestArrivals = GetClosestArrivalTime(serviceTimes, atTime, MaxClosestArrivals);

            return closestArrivals;
        }

        /// <summary>
        /// Method that checks arrival times in a round-robin manner to find the next arrival times
        /// as specified by count.
        /// </summary>
        /// <param name="serviceTimes">The full list of arrival times.</param>
        /// <param name="atTime">Time, relative to which to return arrival time info.</param>
        /// <param name="count">The number of arrival times to return.</param>
        /// <returns>List of arrival times.</returns>
        private IEnumerable<TimeSpan> GetClosestArrivalTime(IEnumerable<TimeSpan> serviceTimes, TimeSpan atTime, int count)
        {
            var closestTimes = new List<TimeSpan>(count);

            // Iterate only if necessary
            if (count > 0)
            {
                var maxWaitTime = new TimeSpan(0, StopServiceInterval, 0);

                TimeSpan retVal = TimeSpan.Zero;

                // Flags that control when to start returning and how many items to return
                bool firstMatchFound = false;
                int needToTake = count;
                
                // Go over the inifite list of arrival times and find the closest match
                foreach (TimeSpan arrivalTime in serviceTimes.ToRoundRobinList())
                {
                    // Calculate the time difference between the current arrival time (arrivalTime) and given time (atTime)
                    var diff = arrivalTime - atTime;
                    
                    // This code block does two things
                    if (diff > TimeSpan.Zero && diff <= maxWaitTime || firstMatchFound)
                    {
                        // Mark that it's done
                        firstMatchFound = true;
                        
                        // Break or decrement as necessary
                        if (needToTake == 0)
                        {
                            break;
                        }
                        else
                        {
                            closestTimes.Add(arrivalTime);
                            needToTake--;
                        }
                    }
                }
            }

            return closestTimes;
        }
        
        /// <summary>
        /// Calculates arrival times for the specified stop-route combination taking into account
        /// information such as time between stops and start service times.
        /// </summary>
        /// <param name="stopNumber">The stop number to calculate the schedule for.</param>
        /// <param name="route">The route to calculate the schedule for.</param>
        /// <returns>List of arrival times for the specfied stop/route combniation.</returns>
        private IEnumerable<TimeSpan> GetArrivalTimes(int stopNumber, int route)
        {
            var serviceTimes = new List<TimeSpan>();

            // Calculate offsets based on route and stop number
            var startServiceOffset = (route - 1) * RouteStartInterval;
            var stopDistanceOffset = (stopNumber - 1) * StopDistanceInMinutes;

            // Start at noon + offset based on stop and route
            var serviceTime = new TimeSpan(0, startServiceOffset + stopDistanceOffset, 0);

            // End 24 hours later
            var endTime = serviceTime.Add(new TimeSpan(24, 0, 0));

            var serviceInterval = new TimeSpan(0, StopServiceInterval, 0);

            do
            {
                var localServiceTime = serviceTime.Days > 0 ? serviceTime.Subtract(new TimeSpan(24, 0, 0)) : serviceTime;

                serviceTimes.Add(localServiceTime);

                serviceTime = serviceTime.Add(serviceInterval);
            } while (serviceTime < endTime);

            return serviceTimes;
        }
    }
}
