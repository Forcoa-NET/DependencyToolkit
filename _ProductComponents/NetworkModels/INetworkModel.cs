using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightedNetwork;

namespace NetworkModels
{
    public interface INetworkModel
    {
        string Name { get; }
        Network Network { get; }
        double[] StringParametersToDouble(string[] values);
        Network GenerateNetwork(int n, double[] p);
    }
}
