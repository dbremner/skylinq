using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using SkyLinq.Linq;

namespace SkyLinq.Web.Models
{
    public class LogModel
    {
        private IEnumerable<string> _lines;
        public LogModel(string directory)
        {
            _lines = Directory.EnumerateFiles(directory)
                .SelectMany(path => LingToText.EnumLines(File.OpenText(path)));
        }

        public IEnumerable<IDictionary<string, object>> GetTop(string column, string direction = "DESC", int count = 20)
        {
            var records = _lines.EnumW3SVCLogRecords();

            var uriStems = records.Select(r => r.URIStem);
            var uriStemsOrderByCount = uriStems
                //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
                .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new Dictionary<string, object>() { { column, kv.Key} , { "count", kv.Value } })
                .Take(20);
            return uriStemsOrderByCount;
        }
    }
}