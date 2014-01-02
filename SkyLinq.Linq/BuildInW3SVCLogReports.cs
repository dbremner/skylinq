using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace SkyLinq.Linq
{
    public static class BuildInW3SVCLogReports
    {
        public static IEnumerable<IDictionary<string, object>> RunReport(this IEnumerable<string> records, 
            Func<IEnumerable<W3SVCLogRecord>, IEnumerable<IDictionary<string, object>>> report)
        {
            return records.AsW3SVCLogRecords().RunReport(report);
        }

        public static IEnumerable<IDictionary<string, object>> RunReport(this IEnumerable<W3SVCLogRecord> records, 
            Func<IEnumerable<W3SVCLogRecord>, IEnumerable<IDictionary<string, object>>> report)
        {
            return report(records);
        }

        [Display(Description = "Get top 25 URLs")]
        public static IEnumerable<IDictionary<string, object>> GetTopUrls(this IEnumerable<W3SVCLogRecord> records)
        {
            var uriStems = records
                .Select(r => r.URIStem);
            return GetTopCounts(uriStems);
        }

        [Display(Description = "Get top user agents")]
        public static IEnumerable<IDictionary<string, object>> GetTopUserAgents(this IEnumerable<W3SVCLogRecord> records)
        {
            var uriStems = records
                .Select(r => r.UserAgent);
            return GetTopCounts(uriStems);
        }

        [Display(Description = "Get top 25 ASP/ASP.NET Pages")]
        public static IEnumerable<IDictionary<string, object>> GetTopPages(this IEnumerable<W3SVCLogRecord> records)
        {
            var uriStems = records
                .Select(r => r.URIStem)
                .Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp") || us.IndexOf('.') < 0);
            return GetTopCounts(uriStems);
        }

        [Display(Description = "Get top 25 File Types")]
        public static IEnumerable<IDictionary<string, object>> GetTopFileTypes(this IEnumerable<W3SVCLogRecord> records)
        {
            var filetypes = records.Select(r => Path.GetExtension(r.URIStem));
            return GetTopCounts(filetypes);
        }

        [Display(Description = "Get top 25 Client IPs")]
        public static IEnumerable<IDictionary<string, object>> GetTopClientIPs(this IEnumerable<W3SVCLogRecord> records)
        {
            var clientIPs = records.Select(r => r.ClientIP);
            return GetTopCounts(clientIPs);
        }

        [Display(Description = "Get Hits by hour")]
        public static IEnumerable<IDictionary<string, object>> GetHitsByHour(this IEnumerable<W3SVCLogRecord> records)
        {
            var hours = records.Select(r => ((r.DateTime.Hour + 18) % 24).ToString("00"));
            return GetCounts(hours);
        }

        [Display(Description = "Get Hits by methods")]
        public static IEnumerable<IDictionary<string, object>> GetHitsByMethods(this IEnumerable<W3SVCLogRecord> records)
        {
            var methods = records.Select(r => r.Method);
            return GetCounts(methods);
        }

        [Display(Description = "Get top errors")]
        public static IEnumerable<IDictionary<string, object>> GetTopErrors(this IEnumerable<W3SVCLogRecord> records)
        {
            var methods = records
                .Where(r => r.Status >= 400)
                .Select(r => r.Status + " " + r.URIStem);
            return GetTopCounts(methods);
        }

        private static IEnumerable<Dictionary<string, object>> GetCounts(IEnumerable<string> strings)
        {
            return strings
                //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
                .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderBy(kv => kv.Key)
                .Select(kv => new Dictionary<string, object>() { { "Url", kv.Key }, { "Hits", kv.Value } });
        }

        private static IEnumerable<Dictionary<string, object>> GetTopCounts(IEnumerable<string> strings)
        {
            return strings
                //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
                .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new Dictionary<string, object>() { { "Url", kv.Key }, { "Hits", kv.Value } })
                .Take(25);
        }
    }
}
