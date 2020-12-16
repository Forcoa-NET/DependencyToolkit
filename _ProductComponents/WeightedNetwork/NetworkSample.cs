using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightedNetwork
{
    public class NetworkSample : INetworkSample
    {
        private Network network = null;
        public readonly ISimilarityStrategy SimilarityStrategy;
        public readonly IRepresentativenessStrategy RepresentativenessStrategy;
        public readonly Dictionary<Vertex, Representative> Representatives;

        public NetworkSample(Network network, ISimilarityStrategy ss, IRepresentativenessStrategy rs,
            bool weightedDegree)
        {
            this.network = network;
            this.SimilarityStrategy = ss;
            this.RepresentativenessStrategy = rs;
            this.Representatives = new Dictionary<Vertex, Representative>();
        }

            private Representative getRepresentative(Vertex v)
        {
            Representative r;
            if (!this.Representatives.TryGetValue(v, out r))
            {
                return null;
            }
            return r;
        }

        public List<Representative> GetSortedrepresentatives()
        {
            List<Representative> sorted = new List<Representative>();
            foreach (Representative r in this.Representatives.Values)
            {
                if (sorted.Count == 0)
                {
                    sorted.Add(r);
                }
                else
                {
                    int indx = 0;
                    while (indx < sorted.Count)
                    {
                        if (r.GetGlobalRepresentativeness() < sorted[indx].GetGlobalRepresentativeness())
                        {
                            indx += 1;
                        }
                        else break;
                    }
                    sorted.Insert(indx, r);
                }
            }
            return sorted;
        }

        public HashSet<Vertex> GetRepresentativeVertices(double rBase)
        {
            HashSet<Vertex> vertices = new HashSet<Vertex>();

            foreach (Representative r in this.Representatives.Values)
            {
                if (r.IsRepresentative(rBase))
                {
                    vertices.Add(r.Vertex);
                }
            }
            return vertices;
        }

        public HashSet<Vertex> GetAllRepresentativeVertices()
        {
            HashSet<Vertex> vertices = new HashSet<Vertex>();

            foreach (Representative r in this.Representatives.Values)
            {
                if (r.GetRepresentativenessBase() > 0)
                {
                    vertices.Add(r.Vertex);
                }
            }
            return vertices;
        }

        public HashSet<Edge> GetRepresentativeEdges(double rBase)
        {
            HashSet<Edge> edges = new HashSet<Edge>();

            foreach (Representative r in this.Representatives.Values)
            {
                foreach (Edge e in r.GetRepresentativeEdges(rBase))
                {
                    edges.Add(e);
                }
            }
            return edges;
        }

        public HashSet<Edge> GetAllRepresentativeEdges()
        {
            return this.GetAllRepresentativeEdges(1, 1, null);
        }

        public HashSet<Edge> GetAllRepresentativeEdges(double reductionRatio, int minEdges, List<string> classes)
        {
            if (reductionRatio < 0)
            {
                reductionRatio = 0;
            }

            HashSet<Edge> edges = new HashSet<Edge>();
            if (minEdges > 0)
            {
                foreach (Representative r in this.Representatives.Values)
                {
                    foreach (Edge e in r.RepresentativeEdges) //vsechny reprezentativni hrany maji stejnou vahu
                    {
                        //bool add = true; // pouze hrany mezi vrcholy stejne tridy
                        //if (classes != null && (classes[e.VertexA.Id] != classes[e.VertexB.Id]))
                        //{
                        //    add = false;
                        //}
                        //if (add)
                        //{
                        //    edges.Add(e);
                        //}
                        edges.Add(e);
                    }
                }
            }

            foreach (Representative r in this.Representatives.Values)
            {
                int neighbors = r.AllSortedEdges.Count;
                
                //pocet hran, ktere se pridaji (vcetne jiz pridanych reprezentativnich)
                int N = (int)Math.Round(reductionRatio * r.GetGlobalRepresentativeness() * neighbors);

                if (N < minEdges)
                {
                    N = minEdges;
                }
                if (N > neighbors)
                {
                    N = neighbors;
                }

                if (N > 0)
                {
                    // pridaji se hrany odpovidajici zadani (nejmene minEdges resp. N vychazejiciho z reductionRatio)
                    int addedEdges = 0;
                    double lastWeight = double.MaxValue; //muze se pridat i vice, pokud maji stejnou vahu

                    foreach (Edge e in r.AllSortedEdges)
                    {
                        if (addedEdges >= N && e.Weight < lastWeight)
                        {
                            break;
                        }

                        bool add = true;
                        if (classes != null && (classes[e.VertexA.Id] != classes[e.VertexB.Id]) && addedEdges > minEdges)
                        {
                            //odfitruji se hrany mezi vrcholy ruzne tridy (az na minEdge nejblizsich sousedu)
                            add = false;
                        }

                        if (add && !edges.Contains(e))
                        {
                            edges.Add(e);
                            //lastWeight = e.Weight; //chyba do 19.6.2018
                        }

                        addedEdges += 1;
                        lastWeight = e.Weight; //spravne
                    }
                }

            }

            return edges;
        }
        //public HashSet<Edge> getAllRepresentativeEdges(double reductionRatio, int minEdges, List<string> classes)
        //{
        //    if (reductionRatio < 0)
        //    {
        //        reductionRatio = 0;
        //    }

        //    HashSet<Edge> edges = new HashSet<Edge>();

        //    foreach (Representative r in this.Representatives.Values)
        //    {
        //        int k = 0;
        //        foreach (Edge e in r.AllSortedEdges)
        //        {
        //            if (k == minEdges)
        //            {
        //                break;
        //            }

        //            if (k < minEdges && r.RepresentativeEdges.Contains(e))
        //            {
        //                edges.Add(e);
        //                k += 1;
        //            }
        //        }
        //    }

        //    foreach (Representative r in this.Representatives.Values)
        //    {
        //        int neighbors = r.AllSortedEdges.Count;
        //        int n = (int)Math.Round(reductionRatio * r.GetGlobalRepresentativeness() * neighbors);
        //        if (n < minEdges)
        //        {
        //            n = minEdges;
        //        }
        //        if (n > neighbors)
        //        {
        //            n = neighbors;
        //        }

        //        int rCount = 0;
        //        double maxWeight = 0;
        //        foreach (Edge e in r.AllSortedEdges)
        //        {
        //            if (maxWeight == 0)
        //            {
        //                maxWeight = e.Weight;
        //            }

        //            if (rCount >= n && e.Weight < maxWeight)
        //            {
        //                break;
        //            }

        //            if (!r.RepresentativeEdges.Contains(e))
        //            {
        //                bool add = true;
        //                if (classes != null && (classes[e.VertexA.Id] != classes[e.VertexB.Id]) && rCount >= minEdges)
        //                {
        //                    //pouze hrany mezi vrcholy stejne tridy (az na minEdge nejblizsich sousedu, kteri se berou vsichni bez rozdilu)
        //                    add = false;
        //                }
        //                if (add)
        //                {
        //                    edges.Add(e);
        //                    maxWeight = e.Weight;
        //                }
        //                rCount += 1;
        //            }
        //        }
        //    }
        //    return edges;
        //}

        //private class countingBox
        //{
        //    public int Count = 0;
        //}
        //private class representativeComparer : IComparer<Representative>
        //{
        //    int IComparer<Representative>.Compare(Representative x, Representative y)
        //    {
        //        double xx = x.GetGlobalRepresentativeness() * x.Vertex.GetEdges().Count;
        //        double yy = y.GetGlobalRepresentativeness() * y.Vertex.GetEdges().Count;
        //        return xx.CompareTo(yy);
        //    }
        //}
        //private HashSet<Edge> getAllRepresentativeEdges(double averageDegree, bool triadic)
        //{
        //    double reductionRatio = 0;
        //    if (averageDegree > 1)
        //    {
        //        reductionRatio = averageDegree / this.GetAverageDegreeEstimation();
        //    }
        //    if (reductionRatio > 1)
        //    {
        //        reductionRatio = 1;
        //    }

        //    List<Representative> representatives = new List<Representative>(this.Representatives.Values);
        //    representatives.Sort(new representativeComparer());
        //    Dictionary<Vertex, countingBox> edgeCounts = new Dictionary<Vertex, countingBox>();
        //    foreach (Representative r in representatives)
        //    {
        //        edgeCounts.Add(r.Vertex, new countingBox());
        //    }
        //    HashSet<Edge> edges = new HashSet<Edge>();
        //    foreach (Representative r in representatives)
        //    {
        //        //if (r.GetGlobalRepresentativeness() > 0)
        //        if (true)
        //        {
        //            foreach (Edge e in r.RepresentativeEdges)
        //            {
        //                if (!edges.Contains(e))
        //                {
        //                    edges.Add(e);
        //                    edgeCounts[e.VertexA].Count += 1;
        //                    edgeCounts[e.VertexB].Count += 1;
        //                }
        //            }
        //        }
        //    }

        //    foreach (Representative r in representatives)
        //    {
        //        //if (r.GetGlobalRepresentativeness() > 0)
        //        if (true)
        //        {
        //            int neighbors = r.AllSortedEdges.Count;
        //            int n = (int)Math.Round(reductionRatio * r.GetGlobalRepresentativeness() * neighbors);
        //            if (n < r.RepresentativeEdges.Count)
        //            {
        //                n = r.RepresentativeEdges.Count;
        //            }
        //            if (n > neighbors)
        //            {
        //                n = neighbors;
        //            }
        //            if (triadic && (n <= 1) && r.AllSortedEdges.Count > 1)
        //            {
        //                n = 2;
        //            }

        //            foreach (Edge e in r.AllSortedEdges)
        //            {
        //                if (edgeCounts[r.Vertex].Count >= n)
        //                    break;

        //                if (!edges.Contains(e))
        //                {
        //                    edges.Add(e);
        //                    edgeCounts[e.VertexA].Count += 1;
        //                    edgeCounts[e.VertexB].Count += 1;
        //                }
        //            }
        //        }
        //    }
        //    return edges;
        //}

        //private HashSet<Edge> getAllRepresentativeEdges(double averageDegree, bool triadic)
        //{
        //    double reductionRatio = 0;
        //    if (averageDegree > 1)
        //    {
        //        reductionRatio = averageDegree / this.GetAverageDegreeEstimation();
        //    }
        //    if (reductionRatio > 1)
        //    {
        //        reductionRatio = 1;
        //    }

        //    Dictionary<Vertex, HashSet<Edge>> vEdges = new Dictionary<Vertex, HashSet<Edge>>();
        //    HashSet<Edge> edges = new HashSet<Edge>();
        //    foreach (Representative r in this.Representatives.Values)
        //    {
        //        foreach (Edge e in r.RepresentativeEdges)
        //        {
        //            edges.Add(e);
        //        }
        //    }

        //    foreach (Representative r in this.Representatives.Values)
        //    {
        //        if (r.GetGlobalRepresentativeness() > 0)
        //        {
        //            int neighbors = r.AllSortedEdges.Count;
        //            int n = (int)Math.Round(reductionRatio * r.GetGlobalRepresentativeness() * neighbors);
        //            if (n < r.RepresentativeEdges.Count)
        //            {
        //                n = r.RepresentativeEdges.Count;
        //            }
        //            int min = 2;
        //            if (triadic && (n < min) && r.AllSortedEdges.Count > 1)
        //            {
        //                n = min;
        //            }
        //            if (n > neighbors)
        //            {
        //                n = neighbors;
        //            }

        //            int i = r.RepresentativeEdges.Count;
        //            foreach (Edge e in r.AllSortedEdges)
        //            {
        //                if (i >= n)
        //                    break;

        //                if (!edges.Contains(e))
        //                {
        //                    HashSet<Edge> ve;
        //                    if (!vEdges.TryGetValue(r.Vertex, out ve))
        //                    {
        //                        vEdges.Add(r.Vertex, new HashSet<Edge>());
        //                    }

        //                    vEdges[r.Vertex].Add(e);
        //                    i += 1;
        //                }
        //            }
        //        }
        //    }

        //    foreach (KeyValuePair<Vertex, HashSet<Edge>> p in vEdges)
        //    {
        //        foreach (Edge e in p.Value)
        //        {
        //            Vertex key = e.GetAdjacent(p.Key);
        //            if (vEdges.ContainsKey(key) && vEdges[key].Contains(e))
        //            {
        //                edges.Add(e);
        //            }
        //        }
        //    }

        //    return edges;
        //}

        public void CalculateRepresentativeness()
        {
            this.Representatives.Clear();
            if (this.network != null)
            {
                this.calculateNetworkRepresentativeness();
            }
        }

        private void calculateNetworkRepresentativeness()
        {
            foreach (Vertex v in this.network.Vertices.Values)
            {
                this.Representatives.Add(v, new Representative(v, this));
            }

            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(this.network.Vertices.Values, options, v => //parallel
            {
                Neighborhood nh = this.SimilarityStrategy.GetNeighborhood(v);
                foreach (Vertex v2 in nh.Neighbors)
                {
                    this.Representatives[v].AllSortedEdges.Add(v.GetEdge(v2));
                    this.Representatives[v2].IncNeighborsWeight(1);

                    if (nh.NearestNeighbors.Contains(v2)) //nejblizsi sousede
                    {
                        this.Representatives[v2].IncNearestNeighborsWeight(1);
                        this.Representatives[v].RepresentativeEdges.Add(v.GetEdge(v2)); //hrany s maximalni vahou
                    }
                }
            });
        }

        // // Nepouzivane veci
        //public class RBaseFrequency
        //{
        //    public readonly double Base;
        //    public int Frequency;
        //    public RBaseFrequency(double rBase)
        //    {
        //        this.Base = rBase;
        //        Frequency = 1;
        //    }
        //}
        
        //public List<RBaseFrequency> GetSortedRBases()
        //{
        //    List<double> bases = new List<double>();
        //    foreach (Representative r in this.Representatives.Values)
        //    {
        //        bases.Add(r.GetRepresentativenessBase());
        //    }
        //    bases.Sort();

        //    List<RBaseFrequency> retList = new List<RBaseFrequency>();
        //    double last = -2.0;
        //    foreach (double b in bases)
        //    {
        //        if (b == last)
        //        {
        //            retList.Last().Frequency++;
        //        }
        //        else
        //        {
        //            retList.Add(new RBaseFrequency(b));
        //            last = b;
        //        }
        //    }

        //    return retList;
        //}

        //public Dictionary<Vertex, HashSet<Vertex>> GetAllRepresentativeClusters()
        //{
        //    Dictionary<Vertex, HashSet<Vertex>> clusters = new Dictionary<Vertex, HashSet<Vertex>>();

        //    foreach (Representative r in this.Representatives.Values)
        //    {
        //        clusters.Add(r.Vertex, new HashSet<Vertex>());
        //    }

        //    foreach (Representative r in this.Representatives.Values)
        //    {
        //        foreach (Vertex v in r.GetNeighborhood().NearestNeighbors)
        //        {
        //            clusters[v].Add(r.Vertex);
        //        }
        //    }

        //    return clusters;
        //}

        //public Dictionary<Vertex, HashSet<Vertex>> GetAllDependencyClusters()
        //{
        //    Dictionary<Vertex, HashSet<Vertex>> clusters = new Dictionary<Vertex, HashSet<Vertex>>();

        //    foreach (Representative r in this.Representatives.Values)
        //    {
        //        clusters.Add(r.Vertex, new HashSet<Vertex>());
        //    }

        //    foreach (Representative r in this.Representatives.Values)
        //    {
        //        foreach (Vertex v in r.GetNeighborhood().Neighbors)
        //        {
        //            if (r.Vertex.GetDependencyOn(v) >= 0.5)
        //            {
        //                clusters[v].Add(r.Vertex);
        //            }
        //        }
        //    }

        //    return clusters;
        //}

        public Network GetReducedNetwork(double reductionRatio, int minEdges)
        {
            return this.GetReducedNetwork(reductionRatio, minEdges, null);
        }

        public Network GetReducedNetwork(double reductionRatio, int minEdges, List<string> classes)
        {
            Network newNetwork = new Network(this.network.IsDirected);
            foreach (Representative r in this.Representatives.Values)
            {
                Vertex v = r.Vertex;
                Vertex vertex = newNetwork.CreateVertex(v.Id, v.Name, this.getRepresentative(v).GetGlobalRepresentativeness());
            }
            foreach (Edge e in this.GetAllRepresentativeEdges(reductionRatio, minEdges, classes))
               {
                   if (e.Weight > 0)
                   {
                       Vertex vA = newNetwork.Vertices[e.VertexA.Id];
                       Vertex vB = newNetwork.Vertices[e.VertexB.Id];
                       newNetwork.CreateEdge(vA, vB, e.Weight);
                   }
               }
            return newNetwork;
        }

        public double[] GetAverageDegreeEstimation(double reductionRatio)
        {
            double sum = 0;
            foreach (Representative r in this.Representatives.Values)
            {
                double estimation = r.GetGlobalRepresentativeness() * r.Vertex.AdjacentsCount;
                //int repr = r.RepresentativeEdges.Count;
                //if (estimation < repr)
                //{
                //    estimation = repr;
                //}
                sum += estimation;
            }

            if (reductionRatio == 0)
            {
                reductionRatio = Math.Log(this.network.Vertices.Count);
            }

            double avg = sum / this.network.Vertices.Count;
            avg *= reductionRatio;

            double[] ret = { avg, reductionRatio };
            return ret;
        }

        public Network GetRepresentativeNetwork(double representativeRatio)
        {
            return null;
        }
    }
}
