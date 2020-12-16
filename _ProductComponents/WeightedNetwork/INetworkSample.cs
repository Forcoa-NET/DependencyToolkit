using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public interface INetworkSample
    {
        void CalculateRepresentativeness();
        Network GetReducedNetwork(double reductionRatio, int minEdges);
        Network GetReducedNetwork(double reductionRatio, int minEdges, List<string> classes);
        double[] GetAverageDegreeEstimation(double reductionRatio);
        Network GetRepresentativeNetwork(double reductionRatio);
    }
}
