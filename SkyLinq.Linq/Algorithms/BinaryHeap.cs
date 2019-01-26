using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SkyLinq.Linq.Algoritms
{
    public enum HeapProperty
    {
        MaxHeap,
        MinHeap
    }

    public sealed class BinaryHeap<TSource, TKey>
    {
        private readonly TSource[] _a;
        private int _size;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IComparer<TKey> _comparer;
        private Func<bool, bool> _heapPropertyPredicate;

        /// <summary>
        /// Constructing a binary heap by heapify an array.
        /// </summary>
        /// <param name="a">The array to heapify.</param>
        /// <param name="heapProperty">Indicate whether it is a max-heap or min-heap.</param>
        public BinaryHeap(TSource[] a, HeapProperty heapProperty, Func<TSource, TKey> keySelector)
            : this(a, heapProperty, keySelector, Comparer<TKey>.Default)
        { }

        /// <summary>
        /// Constructing a binary heap by heapify an array.
        /// </summary>
        /// <param name="a">The array to heapify.</param>
        /// <param name="heapProperty">Indicate whether it is a max-heap or min-heap.</param>
        /// <param name="comparer">An ICompare</param>
        public BinaryHeap(TSource[] a, HeapProperty heapProperty, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            _a = a;
            _size = a.Length;
            _keySelector = keySelector;
            _comparer = comparer;
            SetHeapProperty(heapProperty);

            Heapify(a, _size, keySelector, _comparer, _heapPropertyPredicate);
        }

        /// <summary>
        /// Constructing an empty heap
        /// </summary>
        /// <param name="capacity">Capacity of the Heap</param>
        /// <param name="heapProperty">Indicates whether it is a max-heap or min-heap.</param>
        public BinaryHeap(int capacity, HeapProperty heapProperty, Func<TSource, TKey> keySelector)
            : this(capacity, heapProperty, keySelector, Comparer<TKey>.Default)
        { }

        /// <summary>
        /// Constructing an empty heap
        /// </summary>
        /// <param name="capacity">Capacity of the Heap</param>
        /// <param name="heapProperty">Indicates whether it is a max-heap or min-heap.</param>
        /// <param name="comparer">An ICompare</param>
        public BinaryHeap(int capacity, HeapProperty heapProperty, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            _a = new TSource[capacity];
            _size = 0;
            _keySelector = keySelector;
            _comparer = comparer;
            SetHeapProperty(heapProperty);
        }

        public void Insert(TSource newItem)
        {
            if (_size == _a.Length)
            {
                throw new InvalidOperationException("Heap is full.");
            }

            _a[_size] = newItem;
            if (_size > 0)
            {
                SiftUp(_a, 0, _size, _keySelector, _comparer, _heapPropertyPredicate);
            }

            _size++;
        }

        public TSource Peak()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Heap is empty.");
            }

            return _a[0];
        }

        public TSource Delete()
        {
            TSource ret = Peak();
            _size--;
            if (_size > 1)
            {
                _a[0] = _a[_size];
                SiftDown(_a, 0, _size - 1, _keySelector, _comparer, _heapPropertyPredicate);
            }
            return ret;
        }

        public int Size
        {
            get { return _size; }
        }

        public int Capacity
        {
            get { return _a.Length; }
        }

        public TSource[] Array
        {
            get { return _a; }
        }

        private void SetHeapProperty(HeapProperty heapProperty)
        {
            if (heapProperty == HeapProperty.MaxHeap)
            {
                _heapPropertyPredicate = (b) => !b;
            }
            else
            {
                _heapPropertyPredicate = (b) => b;
            }
        }

        #region static methods

        public static void Heapify(TSource[] a, int size, Func<TSource, TKey> keySelector, Func<bool, bool> predicate)
        {
            Heapify(a, size, keySelector, Comparer<TKey>.Default, predicate);
        }

        public static void Heapify(TSource[] a, int size, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, Func<bool, bool> predicate)
        {
            //The siftUp version which is O(n log n) while the siftDown version is O(n)
            //for (int i = 1; i < _a.Length; i++)
            //{
            //    SiftUp(i);
            //    _size = i + 1;
            //}
            int start = (size - 2) / 2; //Start with the last parent node

            while (start >= 0)
            {
                SiftDown(a, start, size - 1, keySelector, comparer, predicate);
                start--;
                Print(a);
            }
        }

        /// <summary>
        /// Sort an heapified array
        /// </summary>
        /// <param name="a"></param>
        /// <param name="size"></param>
        /// <param name="predicate"></param>
        public static void SortHeapified(TSource[] a,
            int size,
            Func<TSource, TKey> keySelector,
            Func<bool, bool> predicate)
        {
            SortHeapified(a, size, keySelector, Comparer<TKey>.Default, predicate);
        }

        /// <summary>
        /// Sort an heapified array
        /// </summary>
        /// <param name="a"></param>
        /// <param name="size"></param>
        /// <param name="comparer"></param>
        /// <param name="predicate"></param>
        public static void SortHeapified(TSource[] a,
            int size,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            Func<bool, bool> predicate)
        {
            int end = size - 1;
            while (end > 0)
            {
                Swap(a, end, 0);
                end--;
                SiftDown(a, 0, end, keySelector, comparer, predicate);
            }
        }

        public static void HeapSort(TSource[] a, int size, Func<TSource, TKey> keySelector, bool ascending)
        {
            HeapSort(a, size, keySelector, Comparer<TKey>.Default, ascending);
        }


        public static void HeapSort(TSource[] a, int size, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool ascending)
        {
            Func<bool, bool> predicate;
            
            if (ascending)
            {
                predicate = (b) => !b;
            }
            else
            {
                predicate = (b) => b;
            }

            Heapify(a, size, keySelector, comparer, predicate);
            SortHeapified(a, size, keySelector, comparer, predicate);
        }

        //Move up the element at i until it satisfies the heap property
        private static void SiftUp(TSource[]a, int start, int end, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, Func<bool, bool> predicate)
        {
            int child = end;
            while (child > start)
            {
                int parent = (child - 1) / 2;
                if (predicate(comparer.Compare(keySelector(a[parent]), keySelector(a[child])) > 0))
                {
                    Swap(a, parent, child);
                    child = parent;
                }
                else
                {
                    return;
                }
            }
        }

        private static void SiftDown(TSource[] a, int start, int end, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, Func<bool, bool> predicate)
        {
            int root = start;
            while (root * 2 + 1 <=end) //while the root has at least one child
            {
                int child = root * 2 + 1; //left child
                int swap = root;

                if (predicate(comparer.Compare(keySelector(a[swap]), keySelector(a[child])) > 0))
                {
                    swap = child;
                }

                if (child + 1 <= end && predicate(comparer.Compare(keySelector(a[swap]), keySelector(a[child + 1])) > 0)) //right child
                {
                    swap = child + 1;
                }

                if (swap != root)
                {
                    Swap(a, root, swap);
                    root = swap; //Repeat to continue shifting down the child now
                }
                else
                {
                    return;
                }
            }
        }

        private static void Swap(TSource[] a, int i, int j)
        {
            TSource swap = a[i];
            a[i] = a[j];
            a[j] = swap;
        }

        private static void Print(TSource[] a)
        {
            Debug.WriteLine(string.Join(",", a));
        }
        #endregion
    }
}
