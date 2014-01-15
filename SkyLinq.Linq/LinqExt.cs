using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyLinq.Linq.Algoritms;
using System.Diagnostics;

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

        public static IEnumerable<TSource> Top<TSource>(this IEnumerable<TSource> source,
            int n)
        {
            return source.Top(n, item => item);
        }

        public static IEnumerable<TSource> Top<TSource, TKey>(this IEnumerable<TSource> source,
            int n,
            Func<TSource, TKey> keySelector)
        {
            return source.Top(n, keySelector, Comparer<TKey>.Default);
        }

        public static IEnumerable<TSource> Top<TSource, TKey>(this IEnumerable<TSource> source, 
            int n, 
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return source.TakeOrdered(n, keySelector, comparer, false);
        }


        public static IEnumerable<TSource> Bottom<TSource>(this IEnumerable<TSource> source, 
            int n)
        {
            return source.Bottom(n, item => item);
        }

        public static IEnumerable<TSource> Bottom<TSource, TKey>(this IEnumerable<TSource> source, 
            int n,
            Func<TSource, TKey> keySelector)
        {
            return source.Bottom(n, keySelector, Comparer<TKey>.Default);
        }

        public static IEnumerable<TSource> Bottom<TSource, TKey>(this IEnumerable<TSource> source, 
            int n,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return source.TakeOrdered(n, keySelector, comparer, true);
        }

        private static IEnumerable<TSource> TakeOrdered<TSource, TKey>(this IEnumerable<TSource> source, 
            int n,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer, 
            bool ascending)
        {
            Func<bool, bool> predicate;
            HeapProperty heapProperty;

            if (ascending)
            {
                heapProperty = HeapProperty.MaxHeap;
                predicate = (b) => !b;
            }
            else
            {
                heapProperty = HeapProperty.MinHeap;
                predicate = (b) => b;
            }

            BinaryHeap<TSource, TKey> heap = new BinaryHeap<TSource, TKey>(n, heapProperty, keySelector);
            foreach (TSource item in source)
            {
                if (heap.Size < heap.Capacity)
                    heap.Insert(item);
                else
                {
                    if (predicate(comparer.Compare(keySelector(heap.Peak()), keySelector(item)) < 0))
                    {
                        heap.Delete();
                        heap.Insert(item);
                    }
                }
            }
            TSource[] a = heap.Array;
            Debug.WriteLine(string.Join(", ", a));
            BinaryHeap<TSource, TKey>.SortHeapified(a, heap.Size, keySelector, comparer, predicate);
            for (int i = 0; i < heap.Size; i++)
                yield return a[i];
        }
    }
}
