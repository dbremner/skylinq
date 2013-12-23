using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SkyLinq.Web.Models;

namespace SkyLinq.Web.Controllers
{
    public class LogApiController : ApiController
    {
        public IEnumerable<IDictionary<string, object>> GetTop()
        {
            LogModel model = new LogModel(@"G:\codecamp\Linq\LogFiles");
            return model.GetTop("URIStem", "DESC", 20);
        }
    }
}
