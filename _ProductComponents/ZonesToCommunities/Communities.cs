using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LouvainCommunityPL;
using WeightedNetwork;

namespace ZonesToCommunities
{
    public class Communities
    {
        private Graph graph;
        private Network network;

        public readonly Dictionary<int, List<int>> AllCommunities = new Dictionary<int, List<int>>();
        private double modularity;

        public Communities(string filename, Network net)
        {
            this.network = net;
            char[] sep = { ';', '\t' };

            System.IO.StreamReader sr = new System.IO.StreamReader(filename);

            int id = 0;
            int max = 0;
            while (!sr.EndOfStream)
            {
                id += 1;
                List<int> nodeset = new List<int>();

                string[] line = sr.ReadLine().TrimEnd(sep).Split(sep);
                foreach (string s in line)
                {
                    int nodeId = Convert.ToInt32(s);
                    if (this.network.Vertices.ContainsKey(nodeId))
                    {
                        nodeset.Add(nodeId);
                    }
                }

                if (max < nodeset.Count)
                {
                    max = nodeset.Count;
                }

                if (nodeset.Count > 0)
                {
                    this.AllCommunities.Add(id, nodeset);
                }
            }

            sr.Close();
        }

        public Communities(Graph g, Network net)
        {
            this.network = net;
            this.graph = g;
        }

        public void Detect()
        {
            if (this.AllCommunities.Count > 0)
            {
                return;
            }

            if (this.graph.Nodes.Count() > 0)
            {
                Dictionary<int, int> partition = Community.BestPartition(this.graph);
                foreach (var kvp in partition)
                {
                    int id = kvp.Value + 1;

                    List<int> nodeset;
                    if (!this.AllCommunities.TryGetValue(id, out nodeset))
                    {
                        nodeset = new List<int>();
                        this.AllCommunities.Add(id, nodeset);
                    }

                    nodeset.Add(kvp.Key);
                }

                this.modularity = Community.Modularity(this.graph, partition);
            }
            else
            {
                this.modularity = 0;
            }
        }

        public double Modularity
        {
            get
            {
                return this.modularity;
            }
        }
    }

}
