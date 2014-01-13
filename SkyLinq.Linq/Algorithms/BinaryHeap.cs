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

    public class BinaryHeap<T>
    {
        private T[] _a;
        private int _size;
        private IComparer<T> _comparer;
        private Func<bool, bool> _heapPropertyPredicate;

        /// <summary>
        /// Constructing a binary heap by heapify an array.
        /// </summary>
        /// <param name="a">The array to heapify.</param>
        /// <param name="heapProperty">Indicate whether it is a max-heap or min-heap.</param>
        public BinaryHeap(T[] a, HeapProperty heapProperty) 
            : this(a, heapProperty, Comparer<T>.Default)
        { }

        /// <summary>
        /// Constructing a binary heap by heapify an array.
        /// </summary>
        /// <param name="a">The array to heapify.</param>
        /// <param name="heapProperty">Indicate whether it is a max-heap or min-heap.</param>
        /// <param name="comparer">An ICompare</param>
        public BinaryHeap(T[] a, HeapProperty heapProperty, IComparer<T> comparer)
        {
            _a = a;
            _size = a.Length;
            _comparer = comparer;
            SetHeapProperty(heapProperty);

            Heapify(a, _size, _comparer, _heapPropertyPredicate);
        }

        /// <summary>
        /// Construting an empty heap
        /// </summary>
        /// <param name="capacity">Capacity of the Heap</param>
        /// <param name="heapProperty">Indicates whether it is a max-heap or min-heap.</param>
        public BinaryHeap(int capacity, HeapProperty heapProperty)
            : this(capacity, heapProperty, Comparer<T>.Default)
        { }

        /// <summary>
        /// Construting an empty heap
        /// </summary>
        /// <param name="capacity">Capacity of the Heap</param>
        /// <param name="heapProperty">Indicates whether it is a max-heap or min-heap.</param>
        /// <param name="comparer">An ICompare</param>
        public BinaryHeap(int capacity, HeapProperty heapProperty, IComparer<T> comparer)
        {
            _a = new T[capacity];
            _size = 0;
            _comparer = comparer;
            SetHeapProperty(heapProperty);
        }

        public void Insert(T newItem)
        {
            if (_size == _a.Length)
                throw new InvalidOperationException("Heap is full.");

            _a[_size] = newItem;
            if (_size > 0)
                SiftUp(_a, 0, _size, _comparer, _heapPropertyPredicate);
            _size++;
        }

        public T Peak()
        {
            if (_size == 0)
                throw new InvalidOperationException("Heap is empty.");

            return _a[0];
        }

        public T Delete()
        {
            T ret = Peak();
            _size--;
            if (_size > 1)
            { 
                _a[0] = _a[_size];
                SiftDown(_a, 0, _size - 1, _comparer, _heapPropertyPredicate);
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

        public T[] Array
        {
            get { return _a; }
        }

        private void SetHeapProperty(HeapProperty heapProperty)
        {
            if (heapProperty == HeapProperty.MaxHeap)
                _heapPropertyPredicate = (b) => !b;
            else
                _heapPropertyPredicate = (b) => b;
        }

        #region static methods

        public static void Heapify(T[] a, int size, Func<bool, bool> predicate)
        {
            Heapify(a, size, Comparer<T>.Default, predicate);
        }

        public static void Heapify(T[] a, int size, IComparer<T> comparer, Func<bool, bool> predicate)
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
                SiftDown(a, start, size - 1, comparer, predicate);
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
        public static void SortHeapified(T[] a, int size, Func<bool, bool> predicate)
        {
            SortHeapified(a, size, Comparer<T>.Default, predicate);
        }

        /// <summary>
        /// Sort an heapified array
        /// </summary>
        /// <param name="a"></param>
        /// <param name="size"></param>
        /// <param name="comparer"></param>
        /// <param name="predicate"></param>
        public static void SortHeapified(T[] a, int size, IComparer<T> comparer, Func<bool, bool> predicate)
        {
            int end = size - 1;
            while (end > 0)
            {
                Swap(a, end, 0);
                end--;
                SiftDown(a, 0, end, comparer, predicate);
            }
        }

        public static void HeapSort(T[] a, int size, bool ascending)
        {
            HeapSort(a, size, Comparer<T>.Default, ascending);
        }


        public static void HeapSort(T[] a, int size, IComparer<T> comparer, bool ascending)
        {
            Func<bool, bool> predicate;
            
            if (ascending)
                predicate = (b) => !b;
            else
                predicate = (b) => b;

            Heapify(a, size, comparer, predicate);
            SortHeapified(a, size, comparer, predicate);
        }

        //Move up the element at i until it satifies the heap property
        private static void SiftUp(T[]a, int start, int end, IComparer<T> comparer, Func<bool, bool> predicate)
        {
            int child = end;
            while (child > start)
            {
                int parent = (child - 1) / 2;
                if (predicate(comparer.Compare(a[parent], a[start]) > 0))
                {
                    Swap(a, parent, start);
                    child = parent;
                }
                else
                    return;
            }
        }

        private static void SiftDown(T[] a, int start, int end, IComparer<T> comparer, Func<bool, bool> predicate) 
        {
            int root = start;
            while (root * 2 + 1 <=end) //while the root has at least one child
            {
                int child = root * 2 + 1; //left child
                int swap = root;

                if (predicate(comparer.Compare(a[swap], a[child]) > 0))
                    swap = child;
                if (child + 1 <= end && predicate(comparer.Compare(a[swap], a[child + 1]) > 0)) //right child
                    swap = child + 1;
                if (swap != root)
                {
                    Swap(a, root, swap);
                    root = swap; //Repeart to continue sifitng down the child now
                }
                else
                    return;
            }
        }

        private static void Swap(T[] a, int i, int j)
        {
            T swap = a[i];
            a[i] = a[j];
            a[j] = swap;
        }

        private static void Print(T[] a)
        {
            Debug.WriteLine(string.Join(",", a));
        }
        #endregion
    }
}
