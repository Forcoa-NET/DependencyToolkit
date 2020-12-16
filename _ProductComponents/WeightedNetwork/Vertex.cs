using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace WeightedNetwork
{
    public class Vertex
    {
        public static bool ASSERT = false;//pro testovani

        public static bool SAVE_NAMES = true;//vypnout jen pro opravdu velke site

        public readonly Network Network;

        public readonly int Id;
        public readonly string Name = string.Empty;
        public readonly double Weight;

        private Prominency prominency = null;

        private readonly Dictionary<Vertex, Edge> adjacents = new Dictionary<Vertex, Edge>();

        public Vertex(Network net, int id, string name, double weight)
        {
            this.Network = net;

            this.Id = id;
            if (SAVE_NAMES)
            {
                this.Name = name;
            }
            this.Weight = weight;
        }

        public Vertex(Network net, int id, string name)
        {
            this.Network = net;

            this.Id = id;
            if (SAVE_NAMES)
            {
                this.Name = name;
            }
            this.Weight = 1;
        }

        public bool AddAdjacent(Vertex adjacent, Edge edge)
        {
            if (ASSERT)
            {
                System.Diagnostics.Debug.Assert(!this.adjacents.ContainsKey(adjacent), "Edge duplicity...");
                System.Diagnostics.Debug.Assert(this != adjacent, "Self-loop...");
            }
            if (!this.adjacents.ContainsKey(adjacent) && this != adjacent)
            {
                this.adjacents.Add(adjacent, edge);
                this.prominency = null;

                return true;
            }
            else
            {
                return false;
            }
        }

        public Edge GetEdge(Vertex v)
        {
            Edge e;
            if (!this.adjacents.TryGetValue(v, out e))
            {
                return null;
            }
            return e;
        }

        public IEnumerable<Vertex> AdjacentVertices
        {
            get
            {
                return this.adjacents.Keys;
            }
        }

        public IEnumerable<Edge> AdjacentEdges
        {
            get
            {
                return this.adjacents.Values;
            }
        }

        public int AdjacentsCount
        {
            get
            {
                return this.adjacents.Count;
            }
        }

        public double GetDependencyOn(Vertex v, double threshold)
        {
            return this.getDependencyOn(v, threshold);
        }

        public double GetDependencyOn(Vertex v)
        {
            return this.getDependencyOn(v, 1);
        }

        public bool IsDependentOn(Vertex v)
        {
            return this.getDependencyOn(v, this.Network.DependencyThreshold) >= this.Network.DependencyThreshold;
        }

        private double getDependencyOn(Vertex v, double threshold)
        {
            if (this.adjacents.Count() == 0)
            {
                return 0;
            }

            Edge adjEdge;
            if (!this.adjacents.TryGetValue(v, out adjEdge))
            {
                return 0;
            }

            if (threshold == 1 && 
                this.Network.DependencyMatrix.ContainsValue(this.Id, v.Id))
            {
                return this.Network.DependencyMatrix.GetValue(this.Id, v.Id);
            }
            else if (threshold == this.Network.DependencyThreshold &&
                this.Network.BinaryDependencyMatrix.ContainsValue(this.Id, v.Id))
            {
                if (this.Network.BinaryDependencyMatrix.GetValue(this.Id, v.Id))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            double sum = 0;
            foreach (Edge e in this.AdjacentEdges)
            {
                sum += e.Weight;
            }

            IEnumerable<Vertex> adjIntersection = this.AdjacentVertices.Intersect(v.AdjacentVertices);

            double sum1 = adjEdge.Weight;
            double dep;
            foreach (Vertex av in adjIntersection)
            {
                Edge e = this.GetEdge(av);
                double w = e.Weight;
                double w2 = av.GetEdge(v).Weight;

                double depRatio = w2 / (w + w2);
                sum1 += depRatio * w;

                dep = sum1 / sum;
                if (dep == 1)
                {
                    if (threshold == 1)
                    {
                        this.Network.DependencyMatrix.SetValue(this.Id, v.Id, dep);
                    }
                    else if (threshold == this.Network.DependencyThreshold)
                    {
                        this.Network.DependencyMatrix.SetValue(this.Id, v.Id, dep);
                        if (dep >= this.Network.DependencyThreshold)
                        {
                            this.Network.BinaryDependencyMatrix.SetValue(this.Id, v.Id, true);
                        }
                        else
                        {
                            this.Network.BinaryDependencyMatrix.SetValue(this.Id, v.Id, false);
                        }
                    }

                    return dep;
                }
                else if (dep >= threshold)
                {
                    if (threshold == this.Network.DependencyThreshold)
                    {
                        if (dep >= this.Network.DependencyThreshold)
                        {
                            this.Network.BinaryDependencyMatrix.SetValue(this.Id, v.Id, true);
                        }
                        else
                        {
                            this.Network.BinaryDependencyMatrix.SetValue(this.Id, v.Id, false);
                        }
                        return dep;
                    }
                    else 
                    {
                        return dep;
                    }
                }
            }
            //foreach (Vertex av in this.AdjacentEdges.Keys)
            //{
            //    if (this.AdjacentEdges.Keys.Contains(v)) //zadany vrchol je mym sousedem
            //    {

            //        Edge e = this.GetEdge(av);
            //        double w = e.Weight;
            //        if (av == v)
            //        {
            //            sum1 += w;
            //        }
            //        else if (av.AdjacentEdges.Keys.Contains(v))
            //        {
            //            double w2 = av.GetEdge(v).Weight;
            //            double depRatio = w2 / (w + w2);
            //            sum1 += depRatio * w;
            //        }
            //    }

            //    double d = sum1 / sum;
            //    if (d >= threshold)
            //    {
            //        return d;
            //    }
            //}

            dep = sum1 / sum; // nemuze byt dep == 1 ani dep >= threshold (reseno nahore)
            if (threshold == 1)
            {
                this.Network.DependencyMatrix.SetValue(this.Id, v.Id, dep);
            }
            else if (threshold == this.Network.DependencyThreshold)
            {
                //this.Network.DependencyMatrix.SetValue(this.Id, v.Id, dep);
                if (dep >= this.Network.DependencyThreshold)
                {
                    this.Network.BinaryDependencyMatrix.SetValue(this.Id, v.Id, true);
                }
                else
                {
                    this.Network.BinaryDependencyMatrix.SetValue(this.Id, v.Id, false);
                }
            }

            return dep;
        }

        public int GetIndependency()
        {
            double dep = 0;
            foreach (Vertex v in this.AdjacentVertices)
            {
                double d = this.GetDependencyOn(v);
                if (d > this.Network.DependencyThreshold)
                {
                    return -1;
                }
                else if (d > dep)
                {
                    dep = d;
                }
            }

            if (dep < this.Network.DependencyThreshold)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public enum ProminencyType
        {
            StronglyProminent, WeaklyProminent, NonProminent
        }
        public class Prominency
        {
            public readonly Vertex Vertex;

            public readonly int  OneWayDependency;
            public readonly int OneWayIndependency;
            public readonly int TwoWayDependency;
            public readonly int TwoWayIndependency;

            public Prominency(Vertex vertex)
            {
                this.Vertex = vertex;

                int owDep;
                int owIndep;
                int twDep;
                int twIndep;

                this.calculate(out owDep, out owIndep, out twDep, out twIndep);

                this.OneWayDependency = owDep;
                this.OneWayIndependency = owIndep;
                this.TwoWayDependency = twDep;
                this.TwoWayIndependency = twIndep;
            }

            private void calculate(out int owDep, out int owIndep, out int twDep, out int twIndep)
            {
                owDep = 0;
                owIndep = 0;
                twDep = 0;
                twIndep = 0;
                foreach (Vertex adj in this.Vertex.AdjacentVertices)
                {
                    bool thisD = this.Vertex.IsDependentOn(adj);
                    bool adjD = adj.IsDependentOn(this.Vertex);
                    if (thisD && !adjD)
                    {
                        owDep += 1;
                    }
                    else if (!thisD && adjD)
                    {
                        owIndep += 1;
                    }
                    else if (thisD && adjD)
                    {
                        twDep += 1;
                    }
                    else if (!thisD && !adjD)
                    {
                        twIndep += 1;
                    }
                }
            }

            public double GetValue()
            {
                double TP = this.OneWayIndependency;
                double FP = this.TwoWayDependency;
                double FN = this.OneWayDependency;
                double TN = this.TwoWayIndependency;

                double P = TP + FP;
                if (P > 0)
                {
                    P = TP / P;
                }
                double R = TP + FN;
                if (R > 0)
                {
                    R = TP / R;
                }
                double denom = P + R;
                double F1score;
                if (denom > 0)
                {
                    F1score = 2 * P * R / denom;
                }
                else
                {
                    F1score = 0;
                }

                return F1score;
            }
        }
        public Prominency GetProminency()
        {
            if (this.prominency == null)
            {
                this.prominency = new Prominency(this);
            }
            return this.prominency;
        }
        public ProminencyType GetProminencyType()
        {
            double p = this.GetProminency().GetValue();
            if (p == 0)
            {
                return ProminencyType.NonProminent;
            }
            else if (p == 1)
            {
                return ProminencyType.StronglyProminent;
            }
            else
            {
                return ProminencyType.WeaklyProminent;
            }
        }

        public double GetClusteringCoefficient()
        {
            List<Vertex> vertices = new List<Vertex>(this.AdjacentVertices);
            if (vertices.Count < 2)
            {
                return 0;
            }

            int K = vertices.Count;
            double sum = 0;
            for (int i = 0; i < K - 1; i++)
            {
                for (int j = i + 1; j < K; j++)
                {
                    if (vertices[i].GetEdge(vertices[j]) != null)
                    {
                        sum += 1;
                    }
                }
            }
            double k = Math.Max(1, K * (K - 1));
            double cc = 2 * sum / k;
            return cc;
        }

        internal void ResetProminency()
        {
            this.prominency = null;
        }

        public double GetDependencyDegree()
        {
            double dd = 0;
            foreach (Vertex v in this.AdjacentVertices)
            {
                dd += v.GetDependencyOn(this);
            }
            return dd;
        }

        internal void RemoveAdjancent(Vertex vertex)
        {
            this.adjacents.Remove(vertex);
        }

        public double GetDirectedDependencyDegree()
        {
            double bdd = 0;
            foreach (Vertex v in this.AdjacentVertices)
            {
                if (v.IsDependentOn(this))
                {
                    bdd += 1;
                }
            }
            return bdd;
        }

        public double GetWeightedDependencyDegree()
        {
            double sum = 0;
            double weightSum = 0;
            double degree = 0;
            foreach (Edge e in this.AdjacentEdges)
            {
                Vertex v = e.GetAdjacent(this);
                sum += e.Weight * v.GetDependencyOn(this);

                weightSum += e.Weight;
                degree += 1;
            }

            double degDegree = 0;
            if (weightSum > 0)
            {
                degDegree = degree * sum / weightSum;
            }

            return degDegree;
        }

        public double GetDependencyOn(HashSet<Vertex> vertices, double threshold)
        {
            return getDependencyOn(vertices, threshold);
        }

        public double GetDependencyOn(HashSet<Vertex> vertices)
        {
            return getDependencyOn(vertices, 1);
        }

        public bool IsDependentOn(HashSet<Vertex> vertices)
        {
            return this.getDependencyOn(vertices, this.Network.DependencyThreshold) >= this.Network.DependencyThreshold;
        }

        //// Pouze test
        //public double GetStrongClusteringCoefficient()
        //{
        //    if (this.GetEdges().Count < 2)
        //    {
        //        return 0;
        //    }

        //    List<Vertex> neighbors = new List<Vertex>();
        //    foreach (Edge edge in this.GetEdges())
        //    {
        //        neighbors.Add(edge.GetAdjacent(this));
        //    }

        //    int K = neighbors.Count;
        //    double sum = 0;
        //    for (int i = 0; i < neighbors.Count - 1; i++)
        //    {
        //        for (int j = i + 1; j < neighbors.Count; j++)
        //        {
        //            if (neighbors[i].GetEdge(neighbors[j]) != null)
        //            {
        //                sum += 1;
        //            }
        //        }
        //    }

        //    HashSet<Vertex> hNeighbors = new HashSet<Vertex>(neighbors);
        //    hNeighbors.Add(this);
        //    HashSet<Vertex> hFoafs = new HashSet<Vertex>();
        //    foreach (Vertex node in neighbors)
        //    {
        //        foreach (Edge edge in node.GetEdges())
        //        {
        //            Vertex adj = edge.GetAdjacent(node);
        //            if (!hNeighbors.Contains(adj))
        //            {
        //                hFoafs.Add(adj);
        //            }
        //        }
        //    }

        //    List<Vertex> foafs = new List<Vertex>(hFoafs);
        //    int F = foafs.Count;
        //    for (int i = 0; i < neighbors.Count - 1; i++)
        //    {
        //        for (int j = i + 1; j < foafs.Count; j++)
        //        {
        //            if (neighbors[i].GetEdge(foafs[j]) != null)
        //            {
        //                sum += 1;
        //            }
        //        }
        //    }


        //    double k = Math.Max(1, K * (K - 1)) / 2;
        //    double f = F * K;
        //    double scc = sum / (k + f);
        //    return scc;
        //}

        //Prevedeno z VB.Net
        private double getDependencyOn(HashSet<Vertex> vertices, double threshold)
        {
	        if (vertices == null || vertices.Count == 0)
            {
		        return 0;
        	}

            //HashSet<Vertex> otherVertices = new HashSet<Vertex>(vertices);
            HashSet<Vertex> otherVertices = new HashSet<Vertex>();
            foreach (Vertex v in vertices)
            {
                if (this.AdjacentVertices.Contains(v))
                {
                    otherVertices.Add(v);
                }
            }
	        //otherVertices.Remove(this);

	        if (otherVertices.Count == 0)
            {
		        return 0;
	        }

            double max = 0;
            foreach (Vertex v in otherVertices)
            {
                double d = this.GetDependencyOn(v, threshold);
                if (d >= threshold)
                {
                    return d;
                }
                else if (d > max)
                {
                    max = d;
                }
            }

            return max;
        }

        private HashSet<Vertex> getAdjacentsOfVertices(HashSet<Vertex> vertices)
        {
            HashSet<Vertex> adjacentsOfVertices = new HashSet<Vertex>();
            //Sousedi vsech zadanych krome me
            foreach (Vertex v in vertices)
            {
                foreach (Vertex av in v.AdjacentVertices)
                {
                    adjacentsOfVertices.Add(av);
                    //if (av != this && !vertices.Contains(av))
                    //{
                    //    adjacentsOfVertices.Add(av);
                    //}
                }
            }
            return adjacentsOfVertices;
        }

        public double GetWeightedDegree()
        {
            double wd = 0;
            foreach (Edge e in this.adjacents.Values)
            {
                wd += e.Weight;
            }

            return wd;
        }

    }
}
