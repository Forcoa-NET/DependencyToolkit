using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightedNetwork;

namespace NetworkModels
{
    public class BarabasiAlbertModel : INetworkModel
    {
        private Network network;
        private Random random;

        public BarabasiAlbertModel(Random rnd)
        {
            this.random = rnd;
        }

        public string Name
        {
            get
            {
                return "Barabási–Albert";
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
            double m;
            if (values.Length == 0 || !double.TryParse(values[0], out m))
            {
                m = 1;
            }

            if (m < 1)
            {
                m = 1;
            }

            double[] parameters = { Math.Round(m) };
            return parameters;
        }

        public Network GenerateNetwork(int n, double[] parameters)
        {
            int m = (int)parameters[0];
            int m0 = (int)m + 1;
            List<Vertex> probabilityList = this.generateStartingNetwork(m0);

            for (int i = m0; i < n; i++)
            {
                Vertex newVertex = this.network.CreateVertex(i, i.ToString(), 1);

                HashSet<Vertex> adjacents = new HashSet<Vertex>();
                while (adjacents.Count < m)
                {
                    int index = this.random.Next(probabilityList.Count);
                    Vertex candidate = probabilityList[index];
                    adjacents.Add(candidate);
                }

                foreach (Vertex adj in adjacents)
                {
                    this.network.CreateEdge(newVertex, adj, 1);
                    probabilityList.Add(adj);
                }

                probabilityList.Add(newVertex);
            }

            return this.network;
        }

        private List<Vertex> generateStartingNetwork(int m0)
        {
            this.network = new Network(false);
            Vertex v0 = this.network.CreateVertex(0, 0.ToString(), 1);
            Vertex v1 = this.network.CreateVertex(1, 1.ToString(), 1);
            this.network.CreateEdge(v0, v1, 1);

            List<Vertex> probabilityList = new List<Vertex>(this.network.Vertices.Values);

            for (int i = 2; i < m0; i++)
            {
                Vertex newVertex = this.network.CreateVertex(i, i.ToString(), 1);

                int index = this.random.Next(probabilityList.Count);
                Vertex adjacent = probabilityList[index];

                this.network.CreateEdge(newVertex, adjacent, 1);

                probabilityList.Add(adjacent);
                probabilityList.Add(newVertex);
            }

            return probabilityList;
        }

    }

}
