using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyLinq.Composition
{
    public static class QueryExt
    {
        public static IQueryable<TSource> AsSkyLinqQueryable<TSource>(this IEnumerable<TSource> source)
        {
            return new SkyLinqQuery<TSource>(source);
        }
    }
}
