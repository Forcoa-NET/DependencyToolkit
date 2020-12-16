using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightedNetwork
{
    public class ZoneGroup : IDependencyGroup
    {
        public readonly HashSet<DependencyZone> Zones;

        private int id = -1;
        private DependencyZone zone;
        private Vertex ego;
        private HashSet<Vertex> egos;
        private HashSet<Vertex> liaisons;
        private HashSet<Vertex> coLiaisons;
        private HashSet<Vertex> allVertices;
        private HashSet<Vertex> innerZone;
        private HashSet<Vertex> outerZone;

        private Network.GroupDependency dependency = null;
        private int maxZoneCount = 0;

        public ZoneGroup(DependencyZone zone)
        {
            this.zone = zone;
            this.ego = zone.Ego;
            this.Zones = new HashSet<DependencyZone>();

            this.egos = new HashSet<Vertex>();
            this.allVertices = new HashSet<Vertex>();
            this.innerZone = new HashSet<Vertex>();
            this.outerZone = new HashSet<Vertex>();
            this.liaisons = new HashSet<Vertex>();
            this.coLiaisons = new HashSet<Vertex>();
            this.AddZone(this.Zone);
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }
        public DependencyZone Zone
        {
            get
            {
                return this.zone;
            }
        }
        public Vertex Ego
        {
            get
            {
                return this.zone.Ego;
            }
        }
        public HashSet<Vertex> Egos
        {
            get { return this.egos; }
        }
        public HashSet<Vertex> Liaisons
        {
            get { return this.liaisons; }
        }
        public HashSet<Vertex> CoLiaisons
        {
            get { return this.coLiaisons; }
        }
        public HashSet<Vertex> AllVertices
        {
            get { return this.allVertices; }
        }
        public HashSet<Vertex> InnerZone
        {
            get { return this.innerZone; }
        }
        public HashSet<Vertex> OuterZone
        {
            get { return this.outerZone; }
        }

        public double GetEmbeddedness()
        {
            return Network.GetEmbeddedness(this.AllVertices);
        }
        public Network.GroupDependency Dependency
        {
            get
            {
                if (this.dependency == null)
                {
                    this.dependency = Network.GetGroupDependency(this.AllVertices);
                }

                return this.dependency;
            }
        }
        public double GetQuality()
        {
            double q = this.Dependency.DependencyScore;
            q = (1 + q) / 2;
            double d = 1 - (double)1 / this.allVertices.Count;
            q *= d;
            //q *= Math.Log(Math.Log(Math.E + this.InnerZone.Count));

            return q;
        }
        public double GetEgoExpansionRatio()
        {
            if (this.AllVertices.Count == 1)
            {
                return 0;
            }

            double num = 0;
            foreach (Vertex ego in this.Egos)
            {
                double newNum = 0;
                foreach (Vertex adj in ego.AdjacentVertices)
                {
                    if (this.AllVertices.Contains(adj))
                    {
                        newNum += 1;
                    }
                }
                num = Math.Max(num, newNum);
            }

            num += 1;
            double denom = this.AllVertices.Count;
            return 1 - num / denom;
        }

        public void AddZone(DependencyZone zone)
        {
            if (!this.Zones.Contains(zone))
            {
                this.Zones.Add(zone);

                this.Egos.UnionWith(zone.Egos);
                this.AllVertices.UnionWith(zone.AllVertices);

                foreach (Vertex v in zone.InnerZone)
                {
                    this.OuterZone.Remove(v);
                    this.Liaisons.Remove(v);
                    this.CoLiaisons.Remove(v);
                    this.InnerZone.Add(v);
                }

                foreach (Vertex v in zone.OuterZone)
                {
                    if (!this.InnerZone.Contains(v))
                    {
                        this.OuterZone.Add(v);
                        if (zone.Liaisons.Contains(v)) // Liaison
                        {
                            if (!this.CoLiaisons.Contains(v))
                            {
                                this.Liaisons.Add(v);
                            }
                        }
                        else // CoLiaison
                        {
                            this.Liaisons.Remove(v);
                            this.CoLiaisons.Add(v);
                        }
                    }
                }
            }

            if (zone.AllVertices.Count > this.maxZoneCount)
            {
                this.maxZoneCount = zone.AllVertices.Count;
            }

            if (this.isBestEgo(zone.Ego))
            {
                this.ego = zone.Ego;
            }

        }

        private bool isBestEgo(Vertex newEgo)
        {
            Vertex.Prominency prominency = new Vertex.Prominency(this.Ego);
            Vertex.Prominency newProminency = new Vertex.Prominency(newEgo);
            if (prominency.GetValue() < newProminency.GetValue())
            {
                return true;
            }
            else if (newEgo.AdjacentsCount > this.ego.AdjacentsCount)
            {
                return true;
            }
            else
            {
                return newEgo.GetWeightedDegree() > this.Ego.GetWeightedDegree();
            }
        }

        public bool IsZone
        {
            get
            {
                return this.AllVertices.Count == this.maxZoneCount;
            }
        }

        private class ZoneComparer : IComparer<DependencyZone>
        {
            public int Compare(DependencyZone x, DependencyZone y)
            {
                return y.AllVertices.Count.CompareTo(x.AllVertices.Count);
            }
        }
        public List<DependencyZone> GetZonesSortedBySize()
        {
            List<DependencyZone> sorted = new List<DependencyZone>(this.Zones);
            sorted.Sort(new ZoneComparer());

            return sorted;
        }

    }

}
