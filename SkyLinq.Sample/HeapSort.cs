using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyLinq.Linq;
using SkyLinq.Linq.Algoritms;

namespace SkyLinq.Sample
{
    public class HeapSort : ISample
    {
        public void Run()
        {
            int[] a = new int[] { 6, 5, 3, 1, 8, 7, 2, 4 };
            Print("Original", a);
            BinaryHeap<int>.HeapSort(a, a.Length, true);
            Print("Ascending", a);
            BinaryHeap<int>.HeapSort(a, a.Length, false);
            Print("Descending", a);

            IEnumerable<int> top3 = a.Top(3);
            Print("Top 3", top3);

            IEnumerable<int> bottom3 = a.Bottom(3);
            Print("Bottom 3", bottom3);
            
            Console.Read();
        }

        private static void Print(string comment, IEnumerable<int> a)
        {
            Console.WriteLine(comment);
            Console.WriteLine(string.Join(", ", a));
        }
    }
}
