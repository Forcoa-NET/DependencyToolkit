using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public class Network
    {
        public static int MAX_DEGREE_OF_PARALLELISM
        {
            get
            {
                return Environment.ProcessorCount * 2;
            }
        }

        public readonly SparseMatrix<double> DependencyMatrix = new SparseMatrix<double>(0);
        private SparseMatrix<bool> binaryDependencyMatrix = new SparseMatrix<bool>(false);
        private double dependencyThreshold = 0.5;

        public readonly Boolean IsDirected;
        public readonly Boolean IsWeighted;

        public readonly Dictionary<int, Vertex> Vertices = new Dictionary<int, Vertex>();
        public readonly HashSet<Edge> Edges = new HashSet<Edge>();
        public readonly Dictionary<int, Vertex> Outliers = new Dictionary<int, Vertex>();

        private List<HashSet<Vertex>> components = new List<HashSet<Vertex>>();

        public Network(Boolean isDirected)
        {
            this.IsDirected = isDirected;
            this.IsWeighted = true;
        }

        public Network(Boolean isDirected, Boolean isWeighted)
        {
            this.IsDirected = isDirected;
            this.IsWeighted = isWeighted;
        }

        public static double GetDensity(IEnumerable<Vertex> vertices)
        {
            HashSet<Vertex> hsVertices = new HashSet<Vertex>(vertices);

            double N = hsVertices.Count;
            if (N == 0)
            {
                return 0;
            }
            else if (hsVertices.Count == 1)
            {
                return 1;
            }

            HashSet<Edge> edges = new HashSet<Edge>();
            foreach (Vertex v in hsVertices)
            {
                foreach (Vertex adj in v.AdjacentVertices)
                {
                    if (hsVertices.Contains(adj))
                    {
                        edges.Add(v.GetEdge(adj));
                    }
                }
            }

            double M = N * (N - 1) / 2;
            return edges.Count / M;
        }

        public static double GetEmbeddedness(IEnumerable<Vertex> vertices)
        {
            HashSet<Edge> inEdges = new HashSet<Edge>();
            HashSet<Edge> outEdges = new HashSet<Edge>();
            foreach (Vertex v in vertices)
            {
                foreach (Edge e in v.AdjacentEdges)
                {
                    if (!vertices.Contains(e.VertexA) || !vertices.Contains(e.VertexB))
                    {
                        outEdges.Add(e);
                    }
                    else
                    {
                        inEdges.Add(e);
                    }
                }
            }

            double inZone = 2 * inEdges.Count;
            double outZone = outEdges.Count + inZone;
            if (outZone == 0)
            {
                return 0;
            }
            else
            {
                return inZone / outZone;
            }
        }

        public class GroupDependency
        {
            public readonly double InnerDependency;
            public readonly double OuterDependency;
            public GroupDependency(double inner, double outer)
            {
                this.InnerDependency = inner;
                this.OuterDependency = outer;
            }
            public double DependencyScore
            {
                get
                {
                    return this.InnerDependency - this.OuterDependency;
                    //if (this.InnerDependency == 0 && this.OuterDependency == 0)
                    //{
                    //    return -1;
                    //}
                    //else
                    //{
                    //    return this.InnerDependency - this.OuterDependency;
                    //}
                }
            }
        }
        public static GroupDependency GetGroupDependency(IEnumerable<Vertex> vertices)
        {
            HashSet<Edge> inEdges = new HashSet<Edge>();
            HashSet<Edge> outEdges = new HashSet<Edge>();
            int N = 0;
            foreach (Vertex v in vertices)
            {
                N += 1;
                foreach (Edge e in v.AdjacentEdges)
                {
                    if (!vertices.Contains(e.VertexA) || !vertices.Contains(e.VertexB))
                    {
                        outEdges.Add(e);
                    }
                    else
                    {
                        inEdges.Add(e);
                    }
                }
            }

            double depIn = 0;
            foreach (Edge e in inEdges)
            {
                if (e.VertexA.IsDependentOn(e.VertexB))
                {
                    depIn += 1;
                }
                if (e.VertexB.IsDependentOn(e.VertexA))
                {
                    depIn += 1;
                }
            }

            double depOut = 0;
            foreach (Edge e in outEdges)
            {
                if (vertices.Contains(e.VertexA) && e.VertexA.IsDependentOn(e.VertexB))
                {
                    depOut += 1;
                }
                else if (vertices.Contains(e.VertexB) && e.VertexB.IsDependentOn(e.VertexA))
                {
                    depOut += 1;
                }
            }

            if (depIn > 0)
            {
                depIn /= 2 * inEdges.Count;
            }
            if (depOut > 0)
            {
                depOut /= outEdges.Count;
            }

            GroupDependency d = new GroupDependency(depIn, depOut);
            return d;
        }

        public double GetNetDep() //pouze pro neorientovanou sit
        {
            double sumDep = 0;
            foreach (Edge edge in this.Edges)
            {
                if (edge.VertexA.IsDependentOn(edge.VertexB) || edge.VertexB.IsDependentOn(edge.VertexA))
                {
                    sumDep += 1;
                }
            }

            return sumDep / this.Edges.Count;
        }

        public class NetDependency
        {
            public readonly double NetDep;
            public readonly double AvgDependentOn;
            public readonly int MaxDependentOn;
            public Vertex MaxDependentOnVertex;

            public NetDependency(double netDep, double avgDependentOn, int maxDependentOn, Vertex maxDependentOnVertex)
            {
                this.NetDep = netDep;
                this.AvgDependentOn = avgDependentOn;
                this.MaxDependentOn = maxDependentOn;
                this.MaxDependentOnVertex = maxDependentOnVertex;
            }
            public int EstimatedLargeZoneSize
            {
                get
                {
                    double estSize = this.MaxDependentOn * (1 + this.NetDep * this.AvgDependentOn * AvgDependentOn);
                    return Convert.ToInt32(estSize);
                }
            }
        }
        public NetDependency GetNetDependency() //pouze pro neorientovanou sit
        {
            HashSet<Edge> dependentEdges = new HashSet<Edge>();
            int maxDependentOn = 0;
            Vertex maxVertex = null;
            int sumDependentOn = 0;
            foreach (Vertex v in this.Vertices.Values)
            {
                int dependentOn = 0;
                foreach (Edge adjEdge in v.AdjacentEdges)
                {
                    Vertex adj = adjEdge.GetAdjacent(v);
                    if (adj.IsDependentOn(v) || v.IsDependentOn(adj))
                    {
                        dependentEdges.Add(adjEdge);
                        if (adj.IsDependentOn(v))
                        {
                            dependentOn += 1;
                        }
                    }
                }

                sumDependentOn += dependentOn;
                if (dependentOn > maxDependentOn)
                {
                    maxDependentOn = dependentOn;
                    maxVertex = v;
                }
            }

            double netDep = (double)dependentEdges.Count / this.Edges.Count;
            double avg = (double)sumDependentOn / this.Vertices.Count;
            return new NetDependency(netDep, avg, maxDependentOn, maxVertex);
        }

        public SparseMatrix<bool> BinaryDependencyMatrix
        {
            get
            {
                return this.binaryDependencyMatrix;
            }
        }

        public double DependencyThreshold
        {
            get
            {
                return this.dependencyThreshold;
            }

            set
            {
                if (value != this.dependencyThreshold)
                {
                    this.dependencyThreshold = value;
                    this.binaryDependencyMatrix = new SparseMatrix<bool>(false);
                }
            }
        }

        public long GetUniqueEdgeKey(Vertex vertexA, Vertex vertexB)
        {
            long a = vertexA.Id;
            long b = vertexB.Id;

            long key;
            long N = this.Vertices.Count + this.Outliers.Count;
            if (a < b)
            {
                key = N * a + b;
            }
            else
            {
                key = N * b + a;
            }

            return key;
        }

        public Vertex CreateVertex(int id, string name)
        {
            return this.CreateVertex(id, name);
        }

        public Vertex CreateVertex(int id, string name, double weight)
        {
            Vertex vertex;
            if (!this.Vertices.TryGetValue(id, out vertex))
            {
                if (this.IsWeighted)
                {
                    vertex = new Vertex(this, id, name, weight);
                }
                else
                {
                    vertex = new Vertex(this, id, name);
                }
                this.Vertices.Add(vertex.Id, vertex);
            }

            return vertex;
        }

        //public void AddEdge(Vertex vertex, Edge edge)
        //{
        //    Vertex adjacent = edge.GetAdjacent(vertex);
        //    bool success = vertex.AddAdjacent(adjacent, edge);
        //    if (success)
        //    {
        //        this.Edges.Add(edge);
        //    }
        //}

        public Edge CreateEdge(Vertex vertexA, Vertex vertexB)
        {
            return this.CreateEdge(vertexA, vertexB, 1);
        }

        public Edge CreateEdge(Vertex vertexA, Vertex vertexB, double weight)
        {
            Edge edge = vertexA.GetEdge(vertexB);
            if (edge == null)
            {
                if (this.IsWeighted)
                {
                    edge = new Edge(vertexA, vertexB, weight);
                }
                else
                {
                    edge = new Edge(vertexA, vertexB);
                }

                vertexA.AddAdjacent(vertexB, edge);
                if (!this.IsDirected)
                {
                    vertexB.AddAdjacent(vertexA, edge);
                }

                this.Edges.Add(edge);
            }

            return edge;
        }

        public Vertex GetVertexByName(string name)
        {
            foreach (Vertex v in this.Vertices.Values)
            {
                if (v.Name == name)
                {
                    return v;
                }
            }
            return null;
        }

        public Vertex GetVertexById(int id)
        {
            Vertex v = null;
            if (this.Vertices.TryGetValue(id, out v))
            {
                return v;
            }
            return null;
        }

        public void RemoveOutliers()
        {
            List<int> ids = new List<int>();
            foreach (KeyValuePair<int, Vertex> p in this.Vertices)
            {
                Vertex v = p.Value;
                if (v.AdjacentsCount == 0)
                {
                    ids.Add(p.Key);
                }
            }
            foreach (int id in ids)
            {
                this.Outliers.Add(id, this.Vertices[id]);
                this.Vertices.Remove(id);
            }
        }

        public class NetworkProperties
        {
            public readonly int Vertices;
            public readonly double Degree;
            public readonly int MaxDegree;
            public readonly double WeightedDegree;
            public readonly double MaxWeightedDegree;
            public readonly double ClusteringCoefficient;
            public readonly double NodeWeight;
            public readonly double NodeEdgeWeight;
            public NetworkProperties(int v, double d, int maxd, double wd, double maxwd, double cc, double nw, double ew)
            {
                this.Vertices = v;
                this.Degree = d;
                this.MaxDegree = maxd;
                this.WeightedDegree = wd;
                this.MaxWeightedDegree = maxwd;
                this.ClusteringCoefficient = cc;
                this.NodeWeight = nw;
                this.NodeEdgeWeight = ew;
            }
        }

        internal void RemoveEdge(Edge edge)
        {
            this.Edges.Remove(edge);
        }

        public NetworkProperties GetProperties()
        {
            int vertices = this.Vertices.Count + this.Outliers.Count;

            double D = 0;
            int maxD = 0;
            double WD = 0;
            double maxWD = 0;
            double CC = 0;
            double NW = 0;
            double NEW = 0;
            foreach (Vertex v in this.Vertices.Values)
            {
                int sumD = 0;
                double sumWD = 0;
                double maxNEW = 0;
                foreach (Edge e in v.AdjacentEdges)
                {
                    sumD += 1;
                    sumWD += e.Weight;
                    if (e.Weight > maxNEW)
                    {
                        maxNEW = e.Weight;
                    }
                }

                if (sumD > maxD)
                {
                    maxD = sumD;
                }
                if (sumWD > maxWD)
                {
                    maxWD = sumWD;
                }

                D += sumD;
                WD += sumWD;
                CC += v.GetClusteringCoefficient();
                NW += v.Weight;
                NEW += maxNEW;
            }
            if (vertices > 0)
            {
                D /= vertices;
                WD /= vertices;
                CC /= vertices;
                NW /= vertices;
                NEW /= vertices;
            }

            //double EW = 0;
            //foreach (Edge e in this.Edges)
            //{
            //    EW += e.Weight;
            //}
            //if (this.Edges.Count > 0)
            //{
            //    EW /= this.Edges.Count;
            //}

            NetworkProperties p = new NetworkProperties(vertices, D, maxD, WD, maxWD, CC, NW, NEW);
            return p;
        }

        private HashSet<Vertex> processedVertices = new HashSet<Vertex>();
        public HashSet<Vertex> GetConnectedComponent(Vertex v)
        {
            HashSet<Vertex> component = null;
            foreach (HashSet<Vertex> c in this.components)
            {
                if (c.Contains(v))
                {
                    return c;
                }
            }

            this.processedVertices.Clear();
            component = new HashSet<Vertex>();
            this.components.Add(component);

            this.addToComponent(v, component);

            this.processedVertices.Clear();
            return component;
        }

        private void addToComponent(Vertex v, HashSet<Vertex> component)
        {
            component.Add(v);
            processedVertices.Add(v);
            foreach (Vertex adjacent in v.AdjacentVertices)
            {
                if (!processedVertices.Contains(adjacent))
                {
                    this.addToComponent(adjacent, component);
                }
            }
        }

        public HashSet<Vertex> GetLargestConnectedComponent()
        {
            foreach (Vertex v in this.Vertices.Values)
            {
                this.GetConnectedComponent(v);
            }

            int max = 0;
            HashSet<Vertex> lcc = null;
            foreach(HashSet<Vertex> cc in this.components)
            {
                if (cc.Count > max)
                {
                    max = cc.Count;
                    lcc = cc;
                }
            }

            return lcc;
        }

        public Network GetLargestComponentNetwork()
        {
            HashSet<Vertex> component = this.GetLargestConnectedComponent();
            Network newNetwork = new Network(this.IsDirected, this.IsWeighted);

            foreach (Vertex v in this.Vertices.Values)
            {
                if (component.Contains(v))
                {
                    newNetwork.CreateVertex(v.Id, v.Name, v.Weight);
                }
            }

            foreach (Edge e in this.Edges)
            {
                if (component.Contains(e.VertexA) && component.Contains(e.VertexB))
                {
                    Vertex vA = newNetwork.Vertices[e.VertexA.Id];
                    Vertex vB = newNetwork.Vertices[e.VertexB.Id];
                    newNetwork.CreateEdge(vA, vB, e.Weight);
                }
            }

            return newNetwork;
        }

        public Network GetNoOutliersNetwork()
        {
            Network newNetwork = new Network(this.IsDirected, this.IsWeighted);

            foreach (Vertex v in this.Vertices.Values)
            {
                if (v.AdjacentsCount > 0)
                {
                    newNetwork.CreateVertex(v.Id, v.Name, v.Weight);
                }
            }

            foreach (Edge e in this.Edges)
            {
                if (newNetwork.Vertices.ContainsKey(e.VertexA.Id) 
                    && newNetwork.Vertices.ContainsKey(e.VertexB.Id))
                {
                    Vertex vA = newNetwork.Vertices[e.VertexA.Id];
                    Vertex vB = newNetwork.Vertices[e.VertexB.Id];
                    newNetwork.CreateEdge(vA, vB, e.Weight);
                }
            }

            return newNetwork;
        }


    }
}
