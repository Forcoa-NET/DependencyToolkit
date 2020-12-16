using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public class Neighborhood
    {
        public readonly List<Vertex> Neighbors;
        public readonly HashSet<Vertex> NearestNeighbors;

        public Neighborhood(List<Vertex> N, HashSet<Vertex> NN)
        {
            this.Neighbors = N;
            this.NearestNeighbors = NN;
        }
    }
}
