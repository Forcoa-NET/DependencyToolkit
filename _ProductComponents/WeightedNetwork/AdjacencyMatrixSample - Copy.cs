//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace WeightedNetwork
//{
//    public class AdjacencyMatrixSample : INetworkSample
//    {
//        public static bool LRNET_DEFAULT_SETTING = true;

//        private double[][] adjacencyMatrix = null;
//        public readonly bool IsDirected;
//        public readonly ISimilarityStrategy SimilarityStrategy;
//        public readonly IRepresentativenessStrategy RepresentativenessStrategy;

//        private double[] degrees;
//        private double[] nnDegrees;
//        private double[] representativeness;

//        private object nDegreeLock = new object();
//        private object nnDegreeLock = new object();

//        public AdjacencyMatrixSample(ref double[][] adjacencyMatrix, bool isDirected, ISimilarityStrategy ss, IRepresentativenessStrategy rs,
//            bool weightedDegree)
//        {
//            this.adjacencyMatrix = adjacencyMatrix;
//            this.IsDirected = isDirected;
//            this.SimilarityStrategy = ss;
//            this.RepresentativenessStrategy = rs;
//        }

//        public void CalculateRepresentativeness()
//        {
//            int N = this.adjacencyMatrix.Length;

//            double[] maxWeights = new double[N];

//            this.degrees = new double[N];
//            Parallel.For(0, N, i =>
//            {
//               for (int j = 0; j < N; j++)
//               {
//                   double aij = this.adjacencyMatrix[i][j];
//                   if (i != j && aij > 0)
//                   {
//                        this.incDegree(i, 1);

//                        if (aij > maxWeights[i])
//                       {
//                            maxWeights[i] = aij;
//                       }
//                   }
//               }
//            });

//            this.nnDegrees = new double[N];
//            Parallel.For(0, N, i =>
//           {
//               for (int j = 0; j < N; j++)
//               {
//                   double aij = this.adjacencyMatrix[i][j];
//                   double maxWeight = Math.Max(maxWeights[i], 1 / double.MaxValue);
//                   if (i != j && aij == maxWeight)
//                   {
//                       this.incNnDegree(j, 1);
//                   }
//               }
//            });

//            this.representativeness = new double[N];
//            for (int i = 0; i < N; i++)
//            {
//                double Nweight = this.degrees[i];
//                double NNweight = nnDegrees[i];
//                if (LRNET_DEFAULT_SETTING)
//                {
//                    this.representativeness[i] = this.RepresentativenessStrategy.GetRepresentativeness(Nweight, NNweight);
//                }
//                else
//                {
//                    this.representativeness[i] = this.RepresentativenessStrategy.GetRepresentativeness(Nweight, NNweight, N);
//                }
//            }
//        }

//        private void incDegree(int indx, double weight)
//        {
//            lock (nDegreeLock)
//            {
//                this.degrees[indx] += weight;
//            }
//        }

//        private void incNnDegree(int indx, double weight)
//        {
//            lock (nnDegreeLock)
//            {
//                this.nnDegrees[indx] += weight;
//            }
//        }

//        public Network GetReducedNetwork(double reductionRatio, int minEdges)
//        {
//            return this.GetReducedNetwork(reductionRatio, minEdges, null);
//        }

//        public Network GetReducedNetwork(double reductionRatio, int minEdges, List<string> classes)
//        {
//            int N = this.adjacencyMatrix.Length;
//            Network network = new Network(this.IsDirected);

//            for (int i = 0; i < N; i++)
//            {
//                Vertex v = new Vertex(network, i, string.Empty, this.representativeness[i]);
//                network.AddVertex(v);
//            }

//            Dictionary<long, Edge> edges = new Dictionary<long, Edge>();
//            for (int i = 0; i < N; i++)
//            {
//                double r = reductionRatio * this.representativeness[i] * this.degrees[i];
//                r = Math.Round(r);

//                int nR = (int)Math.Max(minEdges, r);

//                double[] line = new double[N];
//                Array.Copy(this.adjacencyMatrix[i], line, N);
//                Array.Sort(line);
//                nR = Math.Min(nR, N);

//                double minWeight;
//                if (nR > 0)
//                {
//                    minWeight = Math.Max(line[N - nR], 1 / double.MaxValue);
//                    for (int j = 0; j < N; j++)
//                    {
//                        if (i != j && this.adjacencyMatrix[i][j] >= minWeight)
//                        {
//                            Vertex vA = network.Vertices[i];
//                            Vertex vB = network.Vertices[j];
//                            Edge e;
//                            if (network.IsDirected)
//                            {
//                                e = new Edge(vA, vB, this.adjacencyMatrix[i][j]);
//                                network.AddEdge(vA, e);
//                            }
//                            else
//                            {
//                                long key = network.GetUniqueEdgeKey(vA, vB);
//                                if (!edges.TryGetValue(key, out e))
//                                {
//                                    e = new Edge(vA, vB, this.adjacencyMatrix[i][j]);
//                                    edges.Add(key, e);
//                                    network.AddEdge(vA, e);
//                                    network.AddEdge(vB, e);
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            return network;
//        }

//        public double[] GetAverageDegreeEstimation(double reductionRatio)
//        {
//            double sum = 0;
//            for (int i = 0; i < this.degrees.Length; i++)
//            {
//                sum += this.representativeness[i] * this.degrees[i];
//            }
//            double avgDegree = sum / this.degrees.Length;

//            double estAvgDegree;
//            if (reductionRatio > 0)
//            {
//                estAvgDegree = avgDegree * reductionRatio;
//            }
//            else
//            {
//                estAvgDegree = Math.Log(this.degrees.Length);
//                reductionRatio = -1;
//                if (avgDegree > 0)
//                {
//                    reductionRatio = estAvgDegree / avgDegree;
//                }
//            }

//            double[] ret = { estAvgDegree, reductionRatio };
//            return ret;
//        }

//        public Network GetRepresentativeNetwork(double reductionRatio)
//        {
//            int N = this.adjacencyMatrix.Length;
//            Network network = new Network(this.IsDirected);

//            double[] line = new double[N];
//            Array.Copy(this.representativeness, line, N);
//            Array.Sort(line);

//            int indx = (int)(reductionRatio * N);
//            double minWeight = Math.Max(line[N - indx], 1 / double.MaxValue);

//            for (int i = 0; i < N; i++)
//            {
//                if (this.representativeness[i] >= minWeight)
//                {
//                    Vertex v = new Vertex(network, i, string.Empty, this.representativeness[i]);
//                    network.AddVertex(v);
//                }
//            }

//            Dictionary<long, Edge> edges = new Dictionary<long, Edge>();
//            for (int i = 0; i < N; i++)
//            {
//                for (int j = 0; j < N; j++)
//                {
//                    if (i != j && this.representativeness[i] >= minWeight && this.representativeness[j] >= minWeight)
//                    {
//                        Vertex vA = network.Vertices[i];
//                        Vertex vB = network.Vertices[j];
//                        Edge e;
//                        if (network.IsDirected)
//                        {
//                            e = new Edge(vA, vB, this.adjacencyMatrix[i][j]);
//                            network.AddEdge(vA, e);
//                        }
//                        else
//                        {
//                            long key = network.GetUniqueEdgeKey(vA, vB);
//                            if (!edges.TryGetValue(key, out e))
//                            {
//                                e = new Edge(vA, vB, this.adjacencyMatrix[i][j]);
//                                edges.Add(key, e);
//                                network.AddEdge(vA, e);
//                                network.AddEdge(vB, e);
//                            }
//                        }
//                    }
//                }

//            }

//            return network;
//        }

//    }
//}
