using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
    public static class LinqExt
    {
        /// <summary>
        /// Group by using accumulator thus reducing the memory footprint. Memory usage proportional to number of groups.
        /// See http://weblogs.asp.net/lichen/archive/2013/12/14/be-aware-of-the-memory-implication-of-groupby-in-linq.aspx for more information.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="seed"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TAccumulate> GroupBy<TSource, TKey, TAccumulate>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func
        )
        {
            IDictionary<TKey, TAccumulate> dict = new Dictionary<TKey, TAccumulate>();

            foreach (TSource src in source)
            {
                TKey key = keySelector(src);
                TAccumulate value;
                if (!dict.TryGetValue(key, out value))
                {
                    value = seed;
                }
                dict[key] = func(value, src);
            }
            return dict;
        }
    }
}
