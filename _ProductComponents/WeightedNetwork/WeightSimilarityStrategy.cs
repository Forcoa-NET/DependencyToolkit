using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public class WeightSimilarityStrategy : ISimilarityStrategy 
    {
        private class EdgeWeightComparer : IComparer<Vertex>
        {
            public readonly Vertex Vertex;

            public EdgeWeightComparer(Vertex v)
            {
                this.Vertex = v;
            }

            public int Compare(Vertex v1, Vertex v2)
            {
                Edge e1 = this.Vertex.GetEdge(v1);
                Edge e2 = this.Vertex.GetEdge(v2);
                int comp = e2.Weight.CompareTo(e1.Weight);
                return comp;
            }
        }

        public WeightSimilarityStrategy()
        {

        }

        public double GetSimilarity(Vertex v, Vertex v2)
        {
            Edge e = v.GetEdge(v2);
            if (e != null)
            {
                return e.Weight;
            }
            return 0;
        }

        public Neighborhood GetNeighborhood(Vertex v)
        {
            List<Vertex> SortedNeighbors = new List<Vertex>(v.AdjacentVertices);
            SortedNeighbors.Sort(new EdgeWeightComparer(v));

            HashSet<Vertex> NN = new HashSet<Vertex>();
            if (SortedNeighbors.Count > 0)
            {
                double w = v.GetEdge(SortedNeighbors[0]).Weight;
                double maxSim = this.GetSimilarity(v, SortedNeighbors[0]);
                foreach (Vertex vv in SortedNeighbors)
                {
                    if (this.GetSimilarity(v, vv) == maxSim) //NN jsou soudedi s maximalni vahou hrany
                    {
                        NN.Add(vv);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Neighborhood nh = new Neighborhood(SortedNeighbors, NN);
            return nh;
        }
    }
}
