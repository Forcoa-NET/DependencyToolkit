using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public interface ISimilarityStrategy
    {
        double GetSimilarity(Vertex v, Vertex v2);
        Neighborhood GetNeighborhood(Vertex v);
    }
}
