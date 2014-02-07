using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SkyLinq.Linq;

namespace SkyLinq.Example
{
    public class LingToGraphExample : IExample
    {
        public void Run()
        {
            ExampleUtil.Dump(DirS(@"../../.."));
        }

        private static IEnumerable<string> DirS(string path)
        {
            return LinqToGraph.DFS(path,    //Or LingToGraph.BFS
                p => Directory.EnumerateDirectories(p),
                p => Directory.EnumerateFiles(p).Select(f => Path.GetFullPath(f)));
        }

    }
}
