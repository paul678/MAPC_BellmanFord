using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAPC_BellmanFord
{
    class BellmanFordSP
    {

        private double[] _distTo;
        private DirectedEdge[] _edgeTo;
        private bool[] _onQueue;
        private Queue<int> _queue;
        private int _cost;
        private IEnumerable<DirectedEdge> _cycle;

        public BellmanFordSP(EdgeWeightedDigraph G, int s)
        {
            _distTo = new double[G.V()];
            _edgeTo = new DirectedEdge[G.V()];
            _onQueue = new bool[G.V()];
            for (int v = 0; v < G.V(); v++)
            {
                _distTo[v] = Double.PositiveInfinity;
            }
            _distTo[s] = 0.0;

            // Bellman-Ford algorithm
            _queue = new Queue<int>();
            _queue.Enqueue(s);
            _onQueue[s] = true;
            while (_queue.Count > 0 && !HasNegativeCycle())
            {
                int v = _queue.Dequeue();
                _onQueue[v] = false;
                Relax(G, v);
            }

            Check(G, s);
        }

        // relax vertex v and put other endpoints on queue if changed
        private void Relax(EdgeWeightedDigraph G, int v)
        {
            foreach (DirectedEdge e in G.adj(v))
            {
                int w = e.To();
                if (_distTo[w] > _distTo[v] + e.Weight())
                {
                    _distTo[w] = _distTo[v] + e.Weight();
                    _edgeTo[w] = e;
                    if (!_onQueue[w])
                    {
                        _queue.Enqueue(w);
                        _onQueue[w] = true;
                    }
                }
                if (_cost++ % G.V() == 0)
                {
                    FindNegativeCycle();
                    if (HasNegativeCycle()) return;  // found a negative cycle
                }
            }
        }

        public bool HasNegativeCycle()
        {
            return _cycle != null;
        }

        public IEnumerable<DirectedEdge> NegativeCycle()
        {
            return _cycle;
        }

        // by finding a cycle in predecessor graph
        private void FindNegativeCycle()
        {
            int V = _edgeTo.Length;
            EdgeWeightedDigraph spt = new EdgeWeightedDigraph(V);
            for (int v = 0; v < V; v++)
                if (_edgeTo[v] != null)
                    spt.AddEdge(_edgeTo[v]);

            EdgeWeightedDirectedCycle finder = new EdgeWeightedDirectedCycle(spt);
            _cycle = finder.Cycle();
        }

        public double DistTo(int v)
        {
            if (HasNegativeCycle())
                throw new System.NotSupportedException("Negative cost cycle exists");
            return _distTo[v];
        }

        public bool HasPathTo(int v)
        {
            return _distTo[v] < Double.PositiveInfinity;
        }

        public IEnumerable<DirectedEdge> PathTo(int v)
        {
            if (HasNegativeCycle())
                throw new System.NotSupportedException("Negative cost cycle exists");
            if (!HasPathTo(v)) return null;
            Stack<DirectedEdge> path = new Stack<DirectedEdge>();
            for (DirectedEdge e = _edgeTo[v]; e != null; e = _edgeTo[e.From()])
            {
                path.Push(e);
            }
            return path;
        }

        // check optimality conditions: either 
        // (i) there exists a negative cycle reacheable from s
        //     or 
        // (ii)  for all edges e = v->w:            distTo[w] <= distTo[v] + e.weight()
        // (ii') for all edges e = v->w on the SPT: distTo[w] == distTo[v] + e.weight()
        private bool Check(EdgeWeightedDigraph G, int s)
        {

            // has a negative cycle
            if (HasNegativeCycle())
            {
                double weight = 0.0;
                foreach (DirectedEdge e in NegativeCycle())
                {
                    weight += e.Weight();
                }
                if (weight >= 0.0)
                {
                    Console.Error.WriteLine("error: weight of negative cycle = " + weight);
                    return false;
                }
            }
            else {

                // check that distTo[v] and edgeTo[v] are consistent
                if (_distTo[s] != 0.0 || _edgeTo[s] != null)
                {
                    Console.Error.WriteLine("distanceTo[s] and edgeTo[s] inconsistent");
                    return false;
                }
                for (int v = 0; v < G.V(); v++)
                {
                    if (v == s) continue;
                    if (_edgeTo[v] == null && _distTo[v] != Double.PositiveInfinity)
                    {
                        Console.Error.WriteLine("distTo[] and edgeTo[] inconsistent");
                        return false;
                    }
                }

                // check that all edges e = v->w satisfy distTo[w] <= distTo[v] + e.weight()
                for (int v = 0; v < G.V(); v++)
                {
                    foreach (DirectedEdge e in G.adj(v))
                    {
                        int w = e.To();
                        if (_distTo[v] + e.Weight() < _distTo[w])
                        {
                            Console.Error.WriteLine("edge " + e + " not relaxed");
                            return false;
                        }
                    }
                }

                // check that all edges e = v->w on SPT satisfy distTo[w] == distTo[v] + e.weight()
                for (int w = 0; w < G.V(); w++)
                {
                    if (_edgeTo[w] == null) continue;
                    DirectedEdge e = _edgeTo[w];
                    int v = e.From();
                    if (w != e.To()) return false;
                    if (_distTo[v] + e.Weight() != _distTo[w])
                    {
                        Console.Error.WriteLine("edge " + e + " on shortest path not tight");
                        return false;
                    }
                }
            }

            Console.WriteLine("Satisfies optimality conditions");
            return true;
        }
    }
}
