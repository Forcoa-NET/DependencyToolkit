using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightedNetwork;

namespace NetworkModels
{
    public class ErdosRenyModel : INetworkModel
    {
        private Network network = null;
        private Random random;

        public ErdosRenyModel(Random rnd)
        {
            this.random = rnd;
        }

        public string Name
        {
            get
            {
                return "Erdős–Rényi";
            }
        }

        public Network Network
        {
            get
            {
                return this.network;
            }
        }

        public double[] StringParametersToDouble(string[] values)
        {
            double probability;
            if (values.Length == 0 || !double.TryParse(values[0], out probability))
            {
                probability = 0;
            }

            if (probability > 1)
            {
                probability = 1;
            }
            else if (probability < 0)
            {
                probability = 0;
            }

            double[] parameters = { probability };
            return parameters;
        }

        public Network GenerateNetwork(int n, double[] parameters)
        {
            double probability = parameters[0];

            this.network = new Network(false);
            for (int i = 0; i < n; i++)
            {
                Vertex v = this.network.CreateVertex(i, i.ToString(), 1);
            }

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double x = this.random.NextDouble();
                    if (x < probability)
                    {
                        Vertex vA = this.network.Vertices[i];
                        Vertex vB = this.network.Vertices[j];
                        this.network.CreateEdge(vA, vB, 1);
                    }
                }
            }

            return this.network;
        }

    }

}
