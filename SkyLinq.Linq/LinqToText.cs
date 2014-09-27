using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SkyLinq.Linq
{
    public static class LinqToText
    {
        public static IEnumerable<string> EnumLines(StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                yield return sr.ReadLine();
            }
        }
    }
}
