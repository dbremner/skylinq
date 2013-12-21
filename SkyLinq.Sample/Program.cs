using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SkyLinq.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //new LingToGraphSample().Run();

            //new LinqToW3SVCLogSample().Run();

            new HttpClientSample().Run();

            Console.Read();
        }
    }
}
