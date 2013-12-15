using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyLinq.Linq
{
    /// <summary>
    /// Reusable DFS and BFS implementations. See http://weblogs.asp.net/lichen/archive/2013/11/16/linq-to-graph.aspx for more information.
    /// </summary>
    public static class LinqToGraph
    {
        public static IEnumerable<TLeaf> DFS<TInner, TLeaf>(TInner node, Func<TInner, IEnumerable<TInner>> getInners, 
            Func<TInner, IEnumerable<TLeaf>> getLeafs)
        {
            return getLeafs(node).Concat(getInners(node).SelectMany(n => DFS(n, getInners, getLeafs)));
        }

        public static IEnumerable<TLeaf> BFS<TInner, TLeaf>(TInner node, Func<TInner, IEnumerable<TInner>> getInners, 
            Func<TInner, IEnumerable<TLeaf>> getLeafs)
        {
            return BFS(node, getInners, getLeafs, new Queue<IEnumerable<TInner>>());
        }

        private static IEnumerable<TLeaf> BFS<TInner, TLeaf>(TInner node, Func<TInner, IEnumerable<TInner>> getInners, 
            Func<TInner, IEnumerable<TLeaf>> getLeafs, Queue<IEnumerable<TInner>> nodeQueue)
        {
            nodeQueue.Enqueue(getInners(node));
            return getLeafs(node).Concat(
                ExecuteQueue(nodeQueue,
                    getInners, getLeafs)
                .SelectMany(leafs => leafs));
        }

        private static IEnumerable<IEnumerable<TLeaf>> ExecuteQueue<TNode, TLeaf>(Queue<IEnumerable<TNode>> nodeQueue, 
            Func<TNode, IEnumerable<TNode>> getInners, Func<TNode, IEnumerable<TLeaf>> getLeafs)
        {
            while (nodeQueue.Count > 0)
            {
                var nodes = nodeQueue.Dequeue();
                foreach(var node in nodes)
                { 
                    yield return BFS(node, getInners, getLeafs, nodeQueue);
                }
            }
        }
    }
}
