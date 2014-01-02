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
    public class LogApiController : ApiController
    {
        private LogApiModel _model = new LogApiModel();

        public IEnumerable<IDictionary<string, object>> Get(string report)
        {
            return _model.RunReport(report);
        }

        //public IEnumerable<IDictionary<string, object>> GetTopUrls()
        //{
        //    var uriStems = _model.GetLogRecords()
        //        .Select(r => r.URIStem);
        //    return GetTopCounts(uriStems);
        //}

        //public IEnumerable<IDictionary<string, object>> GetTopUserAgents()
        //{
        //    var uriStems = _model.GetLogRecords()
        //        .Select(r => r.UserAgent);
        //    return GetTopCounts(uriStems);
        //}

        //public IEnumerable<IDictionary<string, object>> GetTopPages()
        //{
        //    var uriStems = _model.GetLogRecords()
        //        .Select(r => r.URIStem)
        //        .Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"));
        //    return GetTopCounts(uriStems);
        //}

        //public IEnumerable<IDictionary<string, object>> GetTopFileTypes()
        //{
        //    var records = _model.GetLogRecords();
        //    var filetypes = records.Select(r => Path.GetExtension(r.URIStem));
        //    return GetTopCounts(filetypes);
        //}

        //public IEnumerable<IDictionary<string, object>> GetTopClientIPs()
        //{
        //    var records = _model.GetLogRecords();
        //    var clientIPs = records.Select(r => r.ClientIP);
        //    return GetTopCounts(clientIPs);
        //}

        //public IEnumerable<IDictionary<string, object>> GetHitsByHour()
        //{
        //    var records = _model.GetLogRecords();
        //    var hours = records.Select(r => ((r.DateTime.Hour + 18) % 24).ToString("00"));
        //    return GetCounts(hours);
        //}

        //public IEnumerable<IDictionary<string, object>> GetHitsByMethods()
        //{
        //    var records = _model.GetLogRecords();
        //    var methods = records.Select(r => r.Method);
        //    return GetCounts(methods);
        //}

        //public IEnumerable<IDictionary<string, object>> GetTopErrors()
        //{
        //    var records = _model.GetLogRecords();
        //    var methods = records
        //        .Where(r => r.Status >= 400)
        //        .Select(r => r.Status + " " + r.URIStem);
        //    return GetTopCounts(methods);
        //}

        //private static IEnumerable<Dictionary<string, object>> GetCounts(IEnumerable<string> strings)
        //{
        //    return strings
        //        //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
        //        .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
        //        .OrderBy(kv => kv.Key)
        //        .Select(kv => new Dictionary<string, object>() { { "Url", kv.Key }, { "Hits", kv.Value } });
        //}

        //private static IEnumerable<Dictionary<string, object>> GetTopCounts(IEnumerable<string> strings)
        //{
        //    return strings
        //        //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
        //        .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
        //        .OrderByDescending(kv => kv.Value)
        //        .Select(kv => new Dictionary<string, object>() { { "Url", kv.Key }, { "Hits", kv.Value } })
        //        .Take(25);
        //}
    }
}
