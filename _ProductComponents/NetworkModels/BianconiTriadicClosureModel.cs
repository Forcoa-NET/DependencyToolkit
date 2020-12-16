using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightedNetwork;

namespace NetworkModels
{
    public class BianconiTriadicClosureModel : INetworkModel
    {
        private Network network;
        private Random random;

        public BianconiTriadicClosureModel(Random rnd)
        {
            this.random = rnd;
        }

        public string Name
        {
            get
            {
                return "Bianconi-Triadic-Closure";
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

            double m;
            if (values.Length < 2 || !double.TryParse(values[1], out m))
            {
                m = 2;
            }

            m = Math.Round(m);
            if (m < 2)
            {
                m = 2;
            }

            double[] parameters = { probability, m };
            return parameters;
        }

        //private Vertex getRandomVertex()
        //{
        //    int index = this.random.Next(this.network.Edges.Count);
        //    int i = 0;
        //    foreach (Edge e in this.network.Edges)
        //    {
        //        if (i == index)
        //        {
        //            if (e.VertexA.AdjacentEdges.Count >= e.VertexB.AdjacentEdges.Count)
        //            {
        //                return e.VertexA;
        //            }
        //            else
        //            {
        //                return e.VertexB;
        //            }
        //        }

        //        i += 1;
        //    }

        //    return null; //nenastane
        //}

        public Network GenerateNetwork(int n, double[] parameters)
        {
            double probability = parameters[0];
            int m = (int)parameters[1];

            int m0 = m + 1;
            List<Vertex> probabilityList = this.generateStartingNetwork(m0);

            for (int i = m0; i < n; i++)
            {
                List<Vertex> verticesToJoin = new List<Vertex>();

                int index = this.random.Next(this.network.Vertices.Count);
                Vertex randomVertex = this.network.Vertices[index];
                verticesToJoin.Add(randomVertex);

                for (int j = 0; j < m - 1; j++)
                {
                    bool added = false;
                    double x = this.random.NextDouble();
                    if (x < probability)
                    {
                        added = this.addNeighbouringVertex(randomVertex, verticesToJoin);
                    }

                    if (!added)
                    {
                        added = this.addRandomVertex(verticesToJoin);
                    }

                    if (!added)
                    {
                        break;
                    }
                }

                Vertex newVertex = this.network.CreateVertex(i, i.ToString(), 1);

                foreach (Vertex vertexToAdd in verticesToJoin)
                {
                    this.network.CreateEdge(newVertex, vertexToAdd, 1);
                }


            }

            return this.network;
        }

        private bool addNeighbouringVertex(Vertex v, List<Vertex> verticesToJoin)
        {
            List<Vertex> toAdd = new List<Vertex>(v.AdjacentVertices.Except(verticesToJoin));

            if (toAdd.Count == 0)
            {
                return false;
            }
            else
            {
                int index = this.random.Next(toAdd.Count);
                verticesToJoin.Add(toAdd[index]);
                return true;
            }
        }

        private bool addRandomVertex(List<Vertex> verticesToJoin)
        {
            if (verticesToJoin.Count == this.network.Vertices.Count)
            {
                return false;
            }
            else
            {
                Vertex vertexToAdd;
                do
                {
                    int index = this.random.Next(this.network.Vertices.Count);
                    vertexToAdd = this.network.Vertices[index];
                } while (verticesToJoin.Contains(vertexToAdd));

                verticesToJoin.Add(vertexToAdd);
                return true;
            }
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
