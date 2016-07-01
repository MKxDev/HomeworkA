using System.Collections.Generic;
using System.Web.Http;

namespace API.Controllers
{
    public class StopsController : ApiController
    {
        // GET stops
        public IEnumerable<string> Get()
        {
            return new string[] { "Stop 1", "Stop 2" };
        }

        // GET stops/1
        public string Get(int id)
        {
            return $"Stop {id}";
        }
    }
}
