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
            Func<IEnumerable<IW3SVCLogRecord>, IEnumerable<IDictionary<string, object>>> report)
        {
            return records.AsW3SVCLogRecords().RunReport(report);
        }

        public static IEnumerable<IDictionary<string, object>> RunReport(this IEnumerable<IW3SVCLogRecord> records,
            Func<IEnumerable<IW3SVCLogRecord>, IEnumerable<IDictionary<string, object>>> report)
        {
            return report(records);
        }

        [Display(Description = "Get Top 25 URLs")]
        public static IEnumerable<IDictionary<string, object>> GetTopUrls(this IEnumerable<IW3SVCLogRecord> records)
        {
            var uriStems = records
                .Select(r => r.cs_uri_stem);
            return GetTopCounts(uriStems);
        }

        [Display(Description = "Get Top User Agents")]
        public static IEnumerable<IDictionary<string, object>> GetTopUserAgents(this IEnumerable<IW3SVCLogRecord> records)
        {
            var uriStems = records
                .Select(r => r.cs_User_Agent);
            return GetTopCounts(uriStems);
        }

        [Display(Description = "Get Top 25 ASP/ASP.NET Pages")]
        public static IEnumerable<IDictionary<string, object>> GetTopPages(this IEnumerable<IW3SVCLogRecord> records)
        {
            var uriStems = records
                .Select(r => r.cs_uri_stem)
                .Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp") || us.IndexOf('.') < 0);
            return GetTopCounts(uriStems);
        }

        [Display(Description = "Get Top 25 File Types")]
        public static IEnumerable<IDictionary<string, object>> GetTopFileTypes(this IEnumerable<IW3SVCLogRecord> records)
        {
            var filetypes = records.Select(r => Path.GetExtension(r.cs_uri_stem));
            return GetTopCounts(filetypes);
        }

        [Display(Description = "Get Top 25 Client IPs")]
        public static IEnumerable<IDictionary<string, object>> GetTopClientIPs(this IEnumerable<IW3SVCLogRecord> records)
        {
            var clientIPs = records.Select(r => r.c_ip);
            return GetTopCounts(clientIPs);
        }

        [Display(Description = "Get Hits By Hour")]
        public static IEnumerable<IDictionary<string, object>> GetHitsByHour(this IEnumerable<IW3SVCLogRecord> records)
        {
            var hours = records.Select(r => ((r.dateTime.Hour + 18) % 24).ToString("00"));
            return GetCounts(hours);
        }

        [Display(Description = "Get Hits By Methods")]
        public static IEnumerable<IDictionary<string, object>> GetHitsByMethods(this IEnumerable<IW3SVCLogRecord> records)
        {
            var methods = records.Select(r => r.cs_method);
            return GetCounts(methods);
        }

        [Display(Description = "Get Top Errors")]
        public static IEnumerable<IDictionary<string, object>> GetTopErrors(this IEnumerable<IW3SVCLogRecord> records)
        {
            var methods = records
                .Where(r => int.Parse(r.sc_status) >= 400)
                .Select(r => r.sc_status + " " + r.cs_uri_stem);
            return GetTopCounts(methods);
        }

        private static IEnumerable<Dictionary<string, object>> GetCounts(IEnumerable<string> strings)
        {
            return strings
                .GroupBy(us => us, 0, (a, us) => ++a)
                //.GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderBy(kv => kv.Key)
                .Select(kv => new Dictionary<string, object>() { { "Url", kv.Key }, { "Hits", kv.Value } });
        }

        private static IEnumerable<Dictionary<string, object>> GetTopCounts(IEnumerable<string> strings)
        {
            return strings
                .GroupBy(us => us, 0, (a, us) => ++a)
                //.GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                //.OrderByDescending(kv => kv.Value)
                //.Take(25)
                .Top(kv=> kv.Value, 25)
                .Select(kv => new Dictionary<string, object>() { { "Url", kv.Key }, { "Hits", kv.Value } });
        }
    }
}
