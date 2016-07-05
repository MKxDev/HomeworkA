using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Consumer
{
    class Program
    {
        private const string BaseAddress = "http://localhost:8080/";
        private const string UrlTempalte = "stops/{0}/arrivals?time={1}";

        static void Main(string[] args)
        {
            var interval = GetTimerInterval();
            
            var timer = new System.Threading.Timer((state) => TimerTick(), null, 0, interval);
            
            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
        }

        private static void TimerTick()
        {
            var timeNow = DateTime.Now;

            // Test 3:01 PM
            //timeNow = new DateTime(1900, 1, 1, 15, 1, 0, DateTimeKind.Local);

            // Create a time span ignoring the seconds
            var now = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, timeNow.Hour, timeNow.Minute, 0).TimeOfDay;

            Console.WriteLine($"Checking for arrivals at {timeNow.ToString("h\\:mm tt")}");

            var t1 = GetArrivals(1, now);
            var t2 = GetArrivals(2, now);

            // Block until the tasks are done executing
            Task.WhenAll(t1, t2).Wait();

            // Print out results
            PrintArrivalInfo(1, now, t1.Result);
            PrintArrivalInfo(2, now, t2.Result);
        }

        private static int GetTimerInterval()
        {
            const int interval = 60 * 1000;

            return interval;
        }

        private static async Task<IEnumerable<ArrivalModel>> GetArrivals(int stopNumber, TimeSpan time)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var nowTsString = System.Uri.EscapeDataString(time.ToString("hh\\:mm"));

                // HTTP GET
                HttpResponseMessage response = await client.GetAsync(String.Format(UrlTempalte, stopNumber, nowTsString));
                
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    
                    var arrivals = JsonConvert.DeserializeObject<IEnumerable<ArrivalModel>>(jsonString);

                    return arrivals;
                }

                throw new HttpRequestException();
            }
        }

        private static void PrintArrivalInfo(int stopNumber, TimeSpan forTime, IEnumerable<ArrivalModel> arrivals)
        {
            Console.WriteLine($"Stop {stopNumber}:");

            foreach (ArrivalModel model in arrivals)
            {
                var diff1 = model.ArrivalTimes[0] - forTime;
                var diff2 = model.ArrivalTimes[1] - forTime;

                Console.WriteLine($"{model.RouteName} in {diff1.Minutes} mins and {diff2.Minutes} mins");
            }

            Console.WriteLine(String.Empty);
        }
    }
}
