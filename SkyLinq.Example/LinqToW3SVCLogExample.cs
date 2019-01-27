using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SkyLinq.Linq;
using SkyLinq.Composition;

namespace SkyLinq.Example
{
    public sealed class LinqToW3SVCLogExample : IExample
    {
        public void Run()
        {
            string directory = @"G:\codecamp\Linq\LogFiles";

            var lines = Directory.EnumerateFiles(directory)
                .SelectMany(path => LinqToText.EnumLines(File.OpenText(path)));

            //QueryW3SVCLogUsingLinqToText(lines);
            //QueryW3SVCLogUsingLinqToDelimited(lines);
            //QueryW3SVCLogUsingLingToW3SVCLog(lines);
            QueryW3SVCLogUisngSkyLinqQueryable(lines);
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

            ExampleUtil.Dump(uriStemsOrderByCount);
        }

        private static void QueryW3SVCLogUsingLinqToDelimited(IEnumerable<string> lines)
        {
            var records = lines.EnumRecords((n, s) =>
                {
                    if (s.StartsWith("#Fields:"))
                    {
                        return Tuple.Create(LineType.Header, s.Substring(9).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else if (s.StartsWith("#"))
                    {
                        return Tuple.Create(LineType.Comments, new string[] {s});
                    }
                    else
                    {
                        return Tuple.Create(LineType.Data, s.Split(' '));
                    }
                });

            var uriStems = records.Select(l => l["cs-uri-stem"]);

            var uriStemsOrderByCount = uriStems
                //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
                .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => string.Format("{0} {1}", kv.Key, kv.Value))
                .Take(20);

            ExampleUtil.Dump(uriStemsOrderByCount);
        }

        private static void QueryW3SVCLogUisngLingToW3SVCLog(IEnumerable<string> lines)
        {
            var records = lines.AsW3SVCLogRecords();

            var uriStems = records.Select(r => r.cs_uri_stem);
            var uriStemsOrderByCount = uriStems
                //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
                .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => string.Format("{0} {1}", kv.Key, kv.Value))
                .Take(20);
            ExampleUtil.Dump(uriStemsOrderByCount);
        }

        private static void QueryW3SVCLogUisngSkyLinqQueryable(IEnumerable<string> lines)
        {
            var records = lines.AsW3SVCLogRecords().AsSkyLinqQueryable();

            var uriStemsOrderByCount = records.Select(r => r.cs_uri_stem)
                //.Where(us => us.EndsWith(".aspx") || us.EndsWith(".asp"))
                .GroupBy(us => us, (us, uss) => new KeyValuePair<string, int>(us, uss.Count()))
                .OrderByDescending(kv => kv.Value)
                .Take(20)
                .Select(kv => string.Format("{0} {1}", kv.Key, kv.Value));
            ExampleUtil.Dump(uriStemsOrderByCount);
        }
    }
}
