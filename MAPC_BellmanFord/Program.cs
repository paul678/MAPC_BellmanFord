using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAPC_BellmanFord
{
    class Program
    {
        static void Main(string[] args)
        {
            int s = 0;
            EdgeWeightedDigraph G = new EdgeWeightedDigraph(File.OpenText("E:/VisualStudioWorkspace/Projects/MAPC_BellmanFord/MAPC_BellmanFord/test.txt"));

            BellmanFordSP sp = new BellmanFordSP(G, s);

            // print negative cycle
            if (sp.HasNegativeCycle())
            {
                foreach (DirectedEdge e in sp.NegativeCycle())
                    Console.WriteLine(e);
            }

            // print shortest paths
            else {
                for (int v = 0; v < G.V(); v++)
                {
                    if (sp.HasPathTo(v))
                    {
                        Console.WriteLine("{0} to {1} ({2})  ", s, v, sp.DistTo(v));
                        foreach (DirectedEdge e in sp.PathTo(v))
                        {
                            Console.WriteLine(e + "   ");
                        }
                        Console.WriteLine();
                    }
                    else {
                        Console.WriteLine("{0:d} to {1:d}     no path\n", s, v);
                    }
                }
            }
        }
    }
}
