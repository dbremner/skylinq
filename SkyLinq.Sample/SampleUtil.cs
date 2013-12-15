using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyLinq.Sample
{
    public class SampleUtil
    {
        public static void Dump<T>(IEnumerable<T> seq)
        {
            foreach (var o in seq)
            {
                Console.WriteLine(o);
            }
        }
    }
}
