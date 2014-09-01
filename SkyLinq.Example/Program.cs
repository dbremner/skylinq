using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SkyLinq.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //new LingToGraphExample().Run();

            //new LinqToW3SVCLogExample().Run();

            //new HttpClientExample().Run();

            new DuckTypingExample().Run();

            new HeapSort().Run();

            new LINQPadHostExample().Run();

            Console.Read();
        }
    }
}
