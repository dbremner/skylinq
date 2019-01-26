using LINQPadHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyLinq.Example
{
    public sealed class LINQPadHostExample : IExample
    {
        public void Run()
        {
            Host host = new Host();
            foreach (var file in Directory.GetFiles(".\\LINQPadQueries", "*.linq"))
            {
                host.Run<JsonTextSerializer>(file, Console.Out);
            }
        }
    }
}
