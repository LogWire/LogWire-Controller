using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogWire.Controller.Data.Utils
{
    public class PagedResult<T> : PagedResultBase where T : class
    {
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}
