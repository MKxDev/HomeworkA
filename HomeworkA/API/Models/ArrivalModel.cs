using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    public class ArrivalModel
    {
        public string RouteName { get; set; }

        public IEnumerable<TimeSpan> ArrivalTimes { get; set; }
    }
}
