using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
    public static class Util
    {
        public static IDictionary<T, int> ToPositionDictionary<T>(this IEnumerable<T> ks)
        {
            return ks.Select((s, i) => new KeyValuePair<T, int>(s, i)).ToDictionary(p => p.Key, p => p.Value);
        }
    }
}
