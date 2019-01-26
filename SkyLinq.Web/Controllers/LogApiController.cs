using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SkyLinq.Web.Models;
using System.IO;

namespace SkyLinq.Web.Controllers
{
    public sealed class LogApiController : ApiController
    {
        private LogApiModel _model = new LogApiModel();

        public IEnumerable<IDictionary<string, object>> Get(string report)
        {
            return _model.RunReport(report);
        }
    }
}
