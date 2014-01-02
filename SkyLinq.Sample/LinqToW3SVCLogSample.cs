using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SkyLinq.Linq;

namespace SkyLinq.Sample
{
    public class LinqToW3SVCLogSample : ISample
    {
        public void Run()
        {
            string directory = @"G:\codecamp\Linq\LogFiles";

            var lines = Directory.EnumerateFiles(directory)
                .SelectMany(path => LingToText.EnumLines(File.OpenText(path)));

            //QueryW3SVCLogUsingLinqToText(lines);
            //QueryW3SVCLogUsingLinqToDelimited(lines);
            QueryW3SVCLogUisngLingToW3SVCLog(lines);
        }

        private static void QueryW3SVCLogUsingLinqToText(IEnumerable<string> lines)
        {
            var records = lines.Where(l => l.StartsWith("2013"));

            var uriStems = records.Select(l => l.Split(' ')[5]);

            var uriStemsOrderByCount = uriStems
                //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
                .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => string.Format("{0} {1}", kv.Key, kv.Value))
                .Take(20);

            SampleUtil.Dump(uriStemsOrderByCount);
        }

        private static void QueryW3SVCLogUsingLinqToDelimited(IEnumerable<string> lines)
        {
            var records = lines.EnumRecords((n, s) =>
                {
                    if (s.StartsWith("#Fields:"))
                    {
                        return new Tuple<LineType, string[]>(LineType.Header, s.Substring(9).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else if (s.StartsWith("#"))
                    {
                        return new Tuple<LineType, string[]>(LineType.Comments, new string[] {s});
                    }
                    else
                    {
                        return new Tuple<LineType, string[]>(LineType.Data, s.Split(' '));
                    }
                });

            var uriStems = records.Select(l => l["cs-uri-stem"]);

            var uriStemsOrderByCount = uriStems
                //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
                .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => string.Format("{0} {1}", kv.Key, kv.Value))
                .Take(20);

            SampleUtil.Dump(uriStemsOrderByCount);
        }   

        private static void QueryW3SVCLogUisngLingToW3SVCLog(IEnumerable<string> lines)
        {
            var records = lines.AsW3SVCLogRecords();

            var uriStems = records.Select(r => r.URIStem);
            var uriStemsOrderByCount = uriStems
                //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
                .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => string.Format("{0} {1}", kv.Key, kv.Value))
                .Take(20);
            SampleUtil.Dump(uriStemsOrderByCount);
        }
    }
}
