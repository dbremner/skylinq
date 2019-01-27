using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyLinq.Composition
{
    internal static class ReadOnlyCollectionExtensions
    {
        internal static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                return ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>.Empty;
            }

            if (sequence is ReadOnlyCollection<T> ts)
            {
                return ts;
            }
            return new ReadOnlyCollection<T>(sequence.ToList());
        }

        private static class DefaultReadOnlyCollection<T>
        {
            private static volatile ReadOnlyCollection<T> _defaultCollection;

            internal static ReadOnlyCollection<T> Empty
            {
                get
                {
                    if (ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>._defaultCollection == null)
                    {
                        ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>._defaultCollection = new ReadOnlyCollection<T>(new T[0]);
                    }
                    return ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>._defaultCollection;
                }
            }
        }
    }
}
