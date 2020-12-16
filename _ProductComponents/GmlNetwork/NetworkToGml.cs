using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightedNetwork;
using System.Globalization;

namespace GmlNetwork
{
    public class NetworkToGml
    {
        private CultureInfo invC = CultureInfo.InvariantCulture;
        private string TAB = "  ";

        public readonly Network Network;

        public NetworkToGml(Network network)
        {
            this.Network = network;
        }

        public void Write(string gmlFilename)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(gmlFilename);

            sw.WriteLine("graph");
            sw.WriteLine("[");

            this.writeVertices(sw);
            this.writeEdges(sw);

            sw.WriteLine("]");

            sw.Close();
        }

        private void writeVertices(System.IO.StreamWriter sw)
        {
            foreach (Vertex v in this.Network.Vertices.Values)
            {
                this.writeVertex(sw, v);
            }
        }

        private void writeVertex(System.IO.StreamWriter sw, Vertex v)
        {
            sw.WriteLine(TAB + "node");
            sw.WriteLine(TAB + "[");
            sw.WriteLine(TAB +TAB + "id " + v.Id);
            if (v.Id.ToString() != v.Name && !string.IsNullOrEmpty(v.Name))
            {
                sw.WriteLine(TAB + TAB + "label " + "\"" + v.Name  + "\"");
            }
            sw.WriteLine(TAB + "]");
        }

        private void writeEdges(System.IO.StreamWriter sw)
        {
            foreach (Edge e in this.Network.Edges)
            {
                this.writeEdge(sw, e);
            }
        }

        private void writeEdge(System.IO.StreamWriter sw, Edge e)
        {
            sw.WriteLine(TAB + "edge");
            sw.WriteLine(TAB + "[");
            sw.WriteLine(TAB + TAB + "source " + e.VertexA.Id);
            sw.WriteLine(TAB + TAB + "target " + e.VertexB.Id);
            if (this.Network.IsWeighted)
            {
                sw.WriteLine(TAB + TAB + "value " + e.Weight.ToString().Replace(",", "."));
            }
            sw.WriteLine(TAB + "]");
        }

    }
}
