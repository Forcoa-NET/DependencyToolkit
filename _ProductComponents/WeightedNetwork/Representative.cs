using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public class Representative
    {
        public readonly NetworkSample Sample;
        public readonly Vertex Vertex;
        public readonly HashSet<Edge> RepresentativeEdges;
        public readonly List<Edge> AllSortedEdges;
        private double neighborhoodsWeight;
        private double nearestNeighborhoodsWeight;

        public Representative(Vertex v, NetworkSample ns)
        {
            this.Vertex = v;
            this.Sample = ns;
            this.RepresentativeEdges = new HashSet<Edge>();
            this.AllSortedEdges = new List<Edge>();
            this.neighborhoodsWeight = 0;
            this.nearestNeighborhoodsWeight = 0;
        }

        public Neighborhood GetNeighborhood()
        {
            return this.Sample.SimilarityStrategy.GetNeighborhood(this.Vertex);
        }

        public void IncNeighborsWeight(double inc)
        {
            this.neighborhoodsWeight += inc;
        }

        public void IncNearestNeighborsWeight(double inc)
        {
            this.nearestNeighborhoodsWeight += inc;
        }

        public double NeighborsWeight
        {
            get { return neighborhoodsWeight; }
        }

        public double NearestNeighborsWeight
        {
            get { return nearestNeighborhoodsWeight; }
        }

        public bool IsOutlier()
        {
            return this.NeighborsWeight == 0;        
        }

        public bool IsSignificant()
        {
            return this.NearestNeighborsWeight > 0;
        }

        public bool IsNonSignificant()
        {
            return (!(this.IsOutlier() || this.IsSignificant()));
        }

        public double GetRepresentativenessBase()
        {
            return this.Sample.RepresentativenessStrategy.GetRepresentativenessBase(this.NeighborsWeight, this.NearestNeighborsWeight);
        }

        public double GetRepresentativeness(double rBase)
        {
            return this.Sample.RepresentativenessStrategy.GetBaseRepresentativeness(rBase, this.NeighborsWeight, this.NearestNeighborsWeight);
        }

        public bool IsRepresentative(double rBase)
        {
            double eps = 10e-12;
            return (this.GetRepresentativeness(rBase) + eps >= 1);
        }

        public HashSet<Edge> GetRepresentativeEdges(double rBase)
        {
            HashSet<Edge> edges = new HashSet<Edge>();

            if (this.IsRepresentative(rBase))
            {
                foreach (Edge e in this.RepresentativeEdges)
                {
                    Vertex adj = e.GetAdjacent(this.Vertex);
                    if (this.Sample.Representatives[adj].IsRepresentative(rBase))
                    {
                        edges.Add(e);
                    }
                }
            }
            return edges;
        }

        //// Obsolete
        //public HashSet<Edge> GetReducedRepresentativeEdges(double averageDegree, int minEdges)
        //{
        //    double reductionRatio = 0;
        //    if (averageDegree > 1)
        //    {
        //        reductionRatio = averageDegree / this.Sample.GetAverageDegreeEstimation();
        //    }
        //    if (reductionRatio > 1)
        //    {
        //        reductionRatio = 1;
        //    }

        //    HashSet<Edge> edges = new HashSet<Edge>();
        //    foreach (Edge e in this.RepresentativeEdges)
        //    {
        //        edges.Add(e);
        //    }
        //    double q = this.GetGlobalRepresentativeness();
        //    int neighbors = this.AllSortedEdges.Count;
        //    int n = (int)Math.Round(reductionRatio * q * neighbors);
        //    if (n < this.RepresentativeEdges.Count)
        //    {
        //        n = this.RepresentativeEdges.Count;
        //    }
        //    if (minEdges > n)
        //    {
        //        n = Math.Min(minEdges, this.AllSortedEdges.Count);
        //    }
        //    for (int i = this.RepresentativeEdges.Count; i < n; i++)
        //    {
        //        Edge e = this.AllSortedEdges[i];
        //        edges.Add(e);
        //    }
        //    return edges;
        //}

        public double GetGlobalRepresentativeness()
        {
            return this.Sample.RepresentativenessStrategy.GetRepresentativeness(this.NeighborsWeight, this.NearestNeighborsWeight);
        }

        public double GetGlobalRepresentativeness(double maxWeight)
        {
            return this.Sample.RepresentativenessStrategy.GetRepresentativeness(this.NeighborsWeight, this.NearestNeighborsWeight, maxWeight);
        }

    }
}
