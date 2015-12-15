using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAPC_BellmanFord
{
    class EdgeWeightedDirectedCycle
    {
        private bool[] _marked;             // marked[v] = has vertex v been marked?
        private DirectedEdge[] _edgeTo;        // edgeTo[v] = previous edge on path to v
        private bool[] _onStack;            // onStack[v] = is vertex on the stack?
        private Stack<DirectedEdge> _cycle;    // directed cycle (or null if no such cycle)

        public EdgeWeightedDirectedCycle(EdgeWeightedDigraph G)
        {
            _marked = new bool[G.V()];
            _onStack = new bool[G.V()];
            _edgeTo = new DirectedEdge[G.V()];
            for (int v = 0; v < G.V(); v++)
            {
                if (!_marked[v]) DFS(G, v);
            }

            // check that digraph has a cycle
            Check(G);
        }

        // check that algorithm computes either the topological order or finds a directed cycle
        private void DFS(EdgeWeightedDigraph G, int v)
        {
            _onStack[v] = true;
            _marked[v] = true;
            foreach (DirectedEdge e in G.adj(v))
            {
                int w = e.To();

                // short circuit if directed cycle found
                if (_cycle != null) return;

                //found new vertex, so recur
                else if (!_marked[w])
                {
                    _edgeTo[w] = e;
                    DFS(G, w);
                }

                // trace back directed cycle
                else if (_onStack[w])
                {
                    _cycle = new Stack<DirectedEdge>();
                    DirectedEdge traceEdge = e;
                    while (traceEdge.From() != w)
                    {
                        _cycle.Push(traceEdge);
                        traceEdge = _edgeTo[traceEdge.From()];
                    }
                    _cycle.Push(traceEdge);
                    return;
                }
            }

            _onStack[v] = false;
        }

        public bool HasCycle()
        {
            return _cycle != null;
        }

        public IEnumerable<DirectedEdge> Cycle()
        {
            return _cycle;
        }

        // certify that digraph is either acyclic or has a directed cycle
        private bool Check(EdgeWeightedDigraph G)
        {

            // edge-weighted digraph is cyclic
            if (HasCycle())
            {
                // verify cycle
                DirectedEdge first = null, last = null;
                foreach (DirectedEdge e in Cycle())
                {
                    if (first == null) first = e;
                    if (last != null)
                    {
                        if (last.To() != e.From())
                        {
                            Console.WriteLine("cycle edges {0} and {1} not incident\n", last, e);
                            return false;
                        }
                    }
                    last = e;
                }

                if (last.To() != first.From())
                {
                    Console.WriteLine("cycle edges {0} and {1} not incident\n", last, first);
                    return false;
                }
            }

            return true;
        }
    }
}
