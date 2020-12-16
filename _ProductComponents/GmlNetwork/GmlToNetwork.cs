using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightedNetwork;
using System.Globalization;

namespace GmlNetwork
{
    public class GmlToNetwork
    {
        private CultureInfo invC = CultureInfo.InvariantCulture;
        private string[] SEP = { " " };

        private Network network;

        public GmlToNetwork(string gmlFilename)
        {
            this.network = new Network(false);

            System.IO.StreamReader sr = new System.IO.StreamReader(gmlFilename);
            this.readVertices(sr);
            sr.Close();

            sr = new System.IO.StreamReader(gmlFilename);
            this.readEdges(sr);
            sr.Close();
        }

        public Network Network
        {
            get
            {
                return this.network;
            }
        }

        private void readVertices(System.IO.StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.IndexOf("node") >= 0)
                {
                    this.readVertex(sr);
                }
            }
        }

        private void readVertex(System.IO.StreamReader sr)
        {
            sr.ReadLine();

            string[] line = sr.ReadLine().Split(SEP, StringSplitOptions.RemoveEmptyEntries);
            int id = Convert.ToInt32(line[1]);

            string allLine = sr.ReadLine();
            string label = string.Empty;
            if (allLine.IndexOf("label") >= 0)
            {
                string[] sep = { "\"" };
                line = allLine.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                label = line[1].Replace(",", string.Empty);
            }
            else
            {
                label = id.ToString();
            }

            this.network.CreateVertex(id, label, 1);
        }
        
        private void readEdges(System.IO.StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.IndexOf("edge") >= 0)
                {
                    this.readEdge(sr);
                }
            }
        }

        private void readEdge(System.IO.StreamReader sr)
        {
            sr.ReadLine();

            string[] line = sr.ReadLine().Split(SEP, StringSplitOptions.RemoveEmptyEntries);
            Vertex v1 = this.network.Vertices[Convert.ToInt32(line[1])];

            line = sr.ReadLine().Split(SEP, StringSplitOptions.RemoveEmptyEntries);
            Vertex v2 = this.network.Vertices[Convert.ToInt32(line[1])];

            double weight = 1;

            line = sr.ReadLine().Split(SEP, StringSplitOptions.RemoveEmptyEntries);
            if (line.Length > 0 && line[0] == "value")
            {
                weight = double.Parse(line[1], invC);
            }

            this.network.CreateEdge(v1, v2, weight);
        }

    }
}
