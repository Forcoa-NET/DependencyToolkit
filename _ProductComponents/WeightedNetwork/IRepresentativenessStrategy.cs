using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public interface IRepresentativenessStrategy
    {
        double GetRepresentativenessBase(double Nweight, double NNweight);
        double GetRepresentativenessBase(double Nweight, double NNweight, double maxWeight);
        double GetBaseRepresentativeness(double aBase, double Nweight, double NNweight);
        double GetRepresentativeness(double Nweight, double NNweight);
        double GetRepresentativeness(double Nweight, double NNweight, double maxWeight);
    }
}
