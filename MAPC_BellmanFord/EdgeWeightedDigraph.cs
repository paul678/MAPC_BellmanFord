using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace MAPC_BellmanFord
{
    class EdgeWeightedDigraph
    {
        private readonly int _V;                    // number of vertices in this digraph
        private int _E ;                            // number of edges in this digraph
        private ConcurrentBag<DirectedEdge>[] _adj; // adj[v] = adjacency list for vertex v
        private int[] _indegree;                    // indegree[v] = indegree of vertex v

        public EdgeWeightedDigraph(int V)
        {
            if (V < 0) throw new System.ArgumentException("Number of vertices in a Digraph must be nonnegative");
            this._V = V;
            this._E = 0;
            this._indegree = new int[V];
            _adj = new ConcurrentBag<DirectedEdge>[V];
            for (int v = 0; v < V; v++)
                _adj[v] = new ConcurrentBag<DirectedEdge>();
        }


        public EdgeWeightedDigraph(int V, int E) : this(V)
        {
            if (E < 0) throw new System.ArgumentException("Number of edges in a Digraph must be nonnegative");
            Random rand = new Random();
            for (int i = 0; i < E; i++)
            {
                int v = rand.Next(V);
                int w = rand.Next(V);
                double weight = .01 * rand.Next(100);
                DirectedEdge e = new DirectedEdge(v, w, weight);
                AddEdge(e);
            }
        }

        public EdgeWeightedDigraph(System.IO.TextReader reader) : this(int.Parse(reader.ReadLine()))
        {
            int E = int.Parse(reader.ReadLine());
            if (E < 0) throw new System.ArgumentException("Number of edges must be nonnegative");
            for (int i = 0; i < E; i++)
            {
                string text = reader.ReadLine().Trim();
                string[] bits = text.Split(' ');

                int v = int.Parse(bits[0]);
                int w = int.Parse(bits[1]);
                double weight = double.Parse(bits[2]);

                if (v < 0 || v >= _V) throw new System.IndexOutOfRangeException("vertex " + v + " is not between 0 and " + (_V - 1));
                if (w < 0 || w >= _V) throw new System.IndexOutOfRangeException("vertex " + w + " is not between 0 and " + (_V - 1));

                AddEdge(new DirectedEdge(v, w, weight));
            }
        }

        public EdgeWeightedDigraph(EdgeWeightedDigraph G) : this(G._V)
        {
            this._E = G._E;
            for (int v = 0; v < G._V; v++)
                this._indegree[v] = G.Indegree(v);
            for (int v = 0; v < G._V; v++)
            {
                // reverse so that adjacency list is in same order as original
                Stack<DirectedEdge> reverse = new Stack<DirectedEdge>();
                foreach(DirectedEdge e in G._adj[v])
                {
                    reverse.Push(e);
                }
                foreach (DirectedEdge e in reverse)
                {
                    this._adj[v].Add(e);
                }
            }
        }

        public int V()
        {
            return _V;
        }

        public int E()
        {
            return _E;
        }

        // throw an IndexOutOfBoundsException unless 0 <= v < V
        private void ValidateVertex(int v)
        {
            if (v < 0 || v >= this._V)
                throw new System.IndexOutOfRangeException("vertex " + v + " is not between 0 and " + (this._V - 1));
        }

        /// <summary>
        /// Adds the directed edge <tt>e</tt> to this edge-weighted digraph.
        /// IndexOutOfBoundsException unless endpoints of edge are between 0 and V-1
        /// </summary>
        /// <param name="e">e the edge</param>
        public void AddEdge(DirectedEdge e)
        {
            int v = e.From();
            int w = e.To();
            ValidateVertex(v);
            ValidateVertex(w);
            this._adj[v].Add(e);
            this._indegree[w]++;
            this._E++;
        }

        /// <summary>
        /// Returns the directed edges incident from vertex
        /// </summary>
        /// <param name="v"></param>
        /// <returns>the directed edges incident from vertex <tt>v</tt> as an Iterable</returns>
        public IEnumerable<DirectedEdge> adj(int v)
        {
            ValidateVertex(v);
            return this._adj[v];
        }

        /// <summary>
        /// Returns the number of directed edges incident from vertex
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int Outdegree(int v)
        {
            ValidateVertex(v);
            return this._adj[v].Count;
        }

        /// <summary>
        /// Returns the number of directed edges incident to vertex
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int Indegree(int v)
        {
            ValidateVertex(v);
            return this._indegree[v];
        }

        /// <summary>
        ///  Returns all directed edges in this edge-weighted digraph.
        ///  To iterate over the edges in this edge-weighted digraph
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DirectedEdge> Edges()
        {
            ConcurrentBag<DirectedEdge> list = new ConcurrentBag<DirectedEdge>();
            for (int v = 0; v < this._V; v++)
            {
                foreach (DirectedEdge e in adj(v))
                {
                    list.Add(e);
                }
            }
            return list;
        }

        /// <summary>
        ///  Returns a string representation of this edge-weighted digraph
        ///      the number of vertices<em> V</em>, followed by the number of edges <em>E</em>,
        ///      followed by the <em>V</em> adjacency lists of edges
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.Append(this._V + " " + this._E + "\n");
            for (int v = 0; v < this._V; v++)
            {
                s.Append(v + ": ");
                foreach (DirectedEdge e in this._adj[v])
                {
                    s.Append(e + "  ");
                }
                s.Append("\n");
            }
            return s.ToString();
        }
    }
}
