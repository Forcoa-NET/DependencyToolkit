//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace WeightedNetwork
//{
//    public class DependencySimilarityStrategy : ISimilarityStrategy 
//    {
//        private static Dictionary<int, double> DEPENDENCY_CACHE = null;
//        private static double GET_DEPENDENCY(Vertex v1, Vertex v2)
//        {
//            int h = GET_HASH(v1, v2);
//            double x;
//            if (!DEPENDENCY_CACHE.TryGetValue(h, out x))
//            {
//                x = v1.GetDependencyOn(v2);
//                DEPENDENCY_CACHE.Add(h, x);
//            }
//            return x;
//        }

//        private static int GET_HASH(Vertex v1, Vertex v2)
//        {
//            string h = v1.Id + "_" + v2.Id;
//            return h.GetHashCode();
//        }

//        private class VertexDependencyComparer : IComparer<Vertex>
//        {
//            public readonly Vertex Vertex;

//            public VertexDependencyComparer(Vertex v)
//            {
//                this.Vertex = v;
//            }

//            public int Compare(Vertex v1, Vertex v2)
//            {
//                double w1 = GET_DEPENDENCY(this.Vertex, v1);
//                double w2 = GET_DEPENDENCY(this.Vertex, v2);
//                return w2.CompareTo(w1);
//            }
//        }

//        public DependencySimilarityStrategy()
//        {
//            this.initData();
//        }

//        private void initData()
//        {
//            if (DEPENDENCY_CACHE == null)
//            {
//                DEPENDENCY_CACHE = new Dictionary<int, double>();
//            }
//        }
        
//        public double GetSimilarity(Vertex v, Vertex v2)
//        {
//            Edge e = v.GetEdge(v2);
//            if (e != null)
//            {
//                return GET_DEPENDENCY(v, v2);
//            }
//            return 0;
//        }

//        public Neighborhood GetNeighborhood(Vertex v)
//        {
//            List<Vertex> N = new List<Vertex>();
//            N.AddRange(v.GetAdjacents());
//            N.Sort(new VertexDependencyComparer(v));

//            HashSet<Vertex> NN = new HashSet<Vertex>();
//            if (N.Count > 0)
//            {
//                double maxSim = this.GetSimilarity(v, N[0]);
//                foreach (Vertex vv in N)
//                {
//                    if (this.GetSimilarity(v, vv) == maxSim)
//                    {
//                        NN.Add(vv);
//                    }
//                    else
//                    {
//                        break;
//                    }
//                }
//            }

//            Neighborhood nh = new Neighborhood(N, NN);
//            return nh;
//        }
//    }
//}
