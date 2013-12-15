using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SkyLinq.Linq;

namespace SkyLinq.Sample
{
    public class LingToGraphSample : ISample
    {
        public void Run()
        {
            SampleUtil.Dump(DirS(@"../../.."));
        }

        private static IEnumerable<string> DirS(string path)
        {
            return LinqToGraph.DFS(path,    //Or LingToGraph.BFS
                p => Directory.EnumerateDirectories(p),
                p => Directory.EnumerateFiles(p).Select(f => Path.GetFullPath(f)));
        }

    }
}
