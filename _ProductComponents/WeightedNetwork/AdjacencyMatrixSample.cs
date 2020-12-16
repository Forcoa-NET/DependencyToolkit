using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeightedNetwork
{
    public class AdjacencyMatrixSample : INetworkSample
    {
        public static bool LRNET_EXTENDED_SETTING = false;

        private SparseMatrix<double> adjacencyMatrix = null;
        private int N;

        public readonly bool IsDirected;
        public readonly ISimilarityStrategy SimilarityStrategy;
        public readonly IRepresentativenessStrategy RepresentativenessStrategy;

        private double[] degrees;
        private double[] nnDegrees;
        private double[] representativeness;

        private object nDegreeLock = new object();
        private object nnDegreeLock = new object();

        public AdjacencyMatrixSample(ref double[][] adjacencyMatrix, bool isDirected, ISimilarityStrategy ss, IRepresentativenessStrategy rs)
        {
            this.IsDirected = isDirected;
            this.SimilarityStrategy = ss;
            this.RepresentativenessStrategy = rs;

            this.N = adjacencyMatrix.Length;
            this.adjacencyMatrix = new SparseMatrix<double>(0);
            for (int i = 0; i < this.N; i++)
            {
                for (int j = 0; j < this.N; j++)
                {
                    if (i != j)
                    {
                        this.adjacencyMatrix.SetValue(i, j, adjacencyMatrix[i][j]);
                        this.adjacencyMatrix.SetValue(j, i, adjacencyMatrix[j][i]);
                    }
                }
            }
        }

        public AdjacencyMatrixSample(bool isDirected, ISimilarityStrategy ss, IRepresentativenessStrategy rs)
        {
            this.IsDirected = isDirected;
            this.SimilarityStrategy = ss;
            this.RepresentativenessStrategy = rs;

            this.N = 0;
            this.adjacencyMatrix = new SparseMatrix<double>(0);
        }

        public void AddEdge(int i, int j, double weight)
        {
            if (this.N < i + 1)
            {
                this.N = i + 1;
            }
            if (this.N < j + 1)
            {
                this.N = j + 1;
            }
            this.adjacencyMatrix.SetValue(i, j, weight);
            if (!this.IsDirected)
            {
                this.adjacencyMatrix.SetValue(j, i, weight);
            }
        }


        private object maxWeightLock = new object();
        public void CalculateRepresentativeness()
        {
            double[] maxWeights = new double[this.N];

            this.degrees = new double[this.N];
            //for(int i = 0; i < this.N; i++)
            Parallel.For(0, this.N, i =>
            {
                Dictionary<int, double> row;
                if (this.adjacencyMatrix.Matrix.TryGetValue(i, out row))
                {
                    foreach (KeyValuePair<int, double> kvp in row)
                    {
                        if (i != kvp.Key)
                        {
                            double a_ij = kvp.Value;
                            if (a_ij > 0)
                            {
                                this.incDegree(i, 1);
                                if (a_ij > maxWeights[i])
                                {
                                    lock (maxWeightLock)
                                    {
                                        maxWeights[i] = a_ij;
                                    }
                                }
                            }
                        }
                    }
                }
                if (maxWeights[i] == 0)
                {
                    lock (maxWeightLock)
                    {
                        maxWeights[i] = 1 / double.MaxValue;
                    }
                }
            });

            this.nnDegrees = new double[N];
            Parallel.For(0, N, i =>
           {
               //double maxWeight = Math.Max(maxWeights[i], 1 / double.MaxValue);
               //for (int j = 0; j < N; j++)
               //{
               //    if (i != j)
               //    {
               //        double aij = this.adjacencyMatrix.GetValue(i, j);
               //        if (aij == maxWeight)
               //        {
               //            this.incNnDegree(j, 1);
               //        }
               //    }
               //}
               Dictionary<int, double> row;
               if (this.adjacencyMatrix.Matrix.TryGetValue(i, out row))
               {
                   foreach (KeyValuePair<int, double> kvp in row)
                   {
                       int j = kvp.Key;
                       if (i != j)
                       {
                           double a_ij = kvp.Value;
                           if (a_ij == maxWeights[i])
                           {
                               this.incNnDegree(kvp.Key, 1);
                           }
                       }
                   }
               }
                });

            this.representativeness = new double[N];
            for (int i = 0; i < N; i++)
            {
                double Nweight = this.degrees[i];
                double NNweight = nnDegrees[i];
                if (!LRNET_EXTENDED_SETTING)
                {
                    this.representativeness[i] = this.RepresentativenessStrategy.GetRepresentativeness(Nweight, NNweight);
                }
                else
                {
                    this.representativeness[i] = this.RepresentativenessStrategy.GetRepresentativeness(Nweight, NNweight, N);
                }
            }
        }

        private void incDegree(int indx, double weight)
        {
            lock (nDegreeLock)
            {
                this.degrees[indx] += weight;
            }
        }

        private void incNnDegree(int indx, double weight)
        {
            lock (nnDegreeLock)
            {
                this.nnDegrees[indx] += weight;
            }
        }

        public Network GetReducedNetwork(double reductionRatio, int minEdges)
        {
            return this.GetReducedNetwork(reductionRatio, minEdges, null);
        }

        public Network GetReducedNetwork(double reductionRatio, int minEdges, List<string> classes)
        {
            Network newNetwork = new Network(this.IsDirected);

            for (int i = 0; i < this.N; i++)
            {
                Vertex v = newNetwork.CreateVertex(i, string.Empty, this.representativeness[i]);
            }

            Dictionary<long, Edge> edges = new Dictionary<long, Edge>();
            for (int i = 0; i < this.N; i++)
            {
                double r = reductionRatio * this.representativeness[i] * this.degrees[i];
                r = Math.Round(r);

                int nR = (int)Math.Max(minEdges, r);

                List<double> rowList = this.getRow(i);
                rowList.Sort();
                nR = Math.Min(nR, rowList.Count);

                double minWeight;
                if (nR > 0)
                {
                    minWeight = Math.Max(rowList[rowList.Count - nR], 1 / double.MaxValue);
                    Dictionary<int, double> row;
                    if (this.adjacencyMatrix.Matrix.TryGetValue(i, out row))
                    {
                        foreach (KeyValuePair<int, double> kvp in row)
                        {
                            int j = kvp.Key;
                            double a_ij = kvp.Value;
                            if (i != j && a_ij >= minWeight)
                            {
                                Vertex vA = newNetwork.Vertices[i];
                                Vertex vB = newNetwork.Vertices[j];
                                if (newNetwork.IsDirected)
                                {
                                    newNetwork.CreateEdge(vA, vB, a_ij);
                                }
                                else
                                {
                                    long key = newNetwork.GetUniqueEdgeKey(vA, vB);
                                    Edge e;
                                    if (!edges.TryGetValue(key, out e))
                                    {
                                        e = newNetwork.CreateEdge(vA, vB, a_ij);
                                        edges.Add(key, e);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return newNetwork; 
        }

        private List<double> getRow(int i)
        {
            List<double> rowList;
            Dictionary<int, double> row;
            if (this.adjacencyMatrix.Matrix.TryGetValue(i, out row))
            {
                rowList = new List<double>(row.Values);
            }
            else
            {
                rowList = new List<double>();
            }

            return rowList;
        }

        public double[] GetAverageDegreeEstimation(double reductionRatio)
        {
            double sum = 0;
            for (int i = 0; i < this.degrees.Length; i++)
            {
                sum += this.representativeness[i] * this.degrees[i];
            }
            double avgDegree = sum / this.degrees.Length;

            double estAvgDegree;
            if (reductionRatio > 0)
            {
                estAvgDegree = avgDegree * reductionRatio;
            }
            else
            {
                estAvgDegree = Math.Log(this.degrees.Length);
                reductionRatio = -1;
                if (avgDegree > 0)
                {
                    reductionRatio = estAvgDegree / avgDegree;
                }
            }

            double[] ret = { estAvgDegree, reductionRatio };
            return ret;
        }

        public Network GetRepresentativeNetwork(double reductionRatio)
        {
            Network newNetwork = new Network(this.IsDirected);

            double[] line = new double[this.N];
            Array.Copy(this.representativeness, line, this.N);
            Array.Sort(line);

            int indx = (int)(reductionRatio * this.N);
            double minWeight = Math.Max(line[this.N - indx], 1 / double.MaxValue);

            for (int i = 0; i < this.N; i++)
            {
                if (this.representativeness[i] >= minWeight)
                {
                    Vertex v = newNetwork.CreateVertex(i, string.Empty, this.representativeness[i]);
                }
            }

            Dictionary<long, Edge> edges = new Dictionary<long, Edge>();
            for (int i = 0; i < this.N; i++)
            {
                for (int j = 0; j < this.N; j++)
                {
                    if (i != j && this.representativeness[i] >= minWeight && this.representativeness[j] >= minWeight)
                    {
                        Vertex vA = newNetwork.Vertices[i];
                        Vertex vB = newNetwork.Vertices[j];
                        if (newNetwork.IsDirected)
                        {
                            newNetwork.CreateEdge(vA, vB, this.adjacencyMatrix.GetValue(i, j));
                        }
                        else
                        {
                            long key = newNetwork.GetUniqueEdgeKey(vA, vB);
                            Edge e;
                            if (!edges.TryGetValue(key, out e))
                            {
                                e = newNetwork.CreateEdge(vA, vB, this.adjacencyMatrix.GetValue(i, j));
                                edges.Add(key, e);
                            }
                        }
                    }
                }

            }

            return newNetwork;
        }

    }
}
