using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public class DependencyZone : IDependencyGroup
    {
        public enum DuplicityFilter
        {
            None, MultiEgo, All
        }

        private int id = -1;

        public readonly Network Network;
        private Vertex ego;

        private ZoneModularity modularity = null;
        private Network.GroupDependency dependency = null;

        private HashSet<Vertex> egos;
        private HashSet<Vertex> liaisons;
        private HashSet<Vertex> coLiaisons;
        private HashSet<Vertex> allVertices;
        private HashSet<Vertex> innerZone;
        private HashSet<Vertex> outerZone;

        public readonly HashSet<Vertex> InnerMembers;


        public DependencyZone(Network net, Vertex ego, int minSize, int maxSize, bool outerZone)
        {
            this.Network = net;
            this.ego = ego;

            this.allVertices = new HashSet<Vertex>();
            this.allVertices.Add(this.Ego);

            this.innerZone = new HashSet<Vertex>();
            this.innerZone.Add(this.Ego);

            this.egos = new HashSet<Vertex>();
            this.egos.Add(this.Ego);
            this.InnerMembers = new HashSet<Vertex>();

            this.outerZone = new HashSet<Vertex>();
            this.liaisons = new HashSet<Vertex>();
            this.coLiaisons = new HashSet<Vertex>();

            this.calculateZone(minSize, maxSize, outerZone);
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }
        public Vertex Ego
        {
            get
            {
                return this.ego;
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
        public double GetEmbeddedness()
        {
            return Network.GetEmbeddedness(this.AllVertices);
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

        public void AddEgo(Vertex newEgo)
        {
            this.Egos.Add(newEgo);
            this.InnerMembers.Remove(newEgo);

            if (this.isBestEgo(newEgo))
            {
                this.ego = newEgo;
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

        private double getInnerZoneWeightedDegree(Vertex v)
        {
            double weight = 0;
            foreach (Vertex adj in v.AdjacentVertices)
            {
                if (InnerZone.Contains(adj))
                {
                    weight += v.GetEdge(adj).Weight;
                }
            }

            return weight;
        }

        private void calculateZone(int minSize, int maxSize, bool outerZone)
        {
            HashSet<Vertex> neighborhood = new HashSet<Vertex>();
            HashSet<Vertex> verticesToAdd = new HashSet<Vertex>(this.AllVertices);
            this.updateNeighborhood(neighborhood, verticesToAdd);
            while (true)
            {
                HashSet<Vertex> lastAddedVertices = new HashSet<Vertex>(verticesToAdd);
                verticesToAdd.Clear();
                //this.rebuildNeighborhood(neighborhood);
                foreach (Vertex outVertex in neighborhood)
                {
                    //if (outVertex.GetDependencyOn(this.AllVertices, singleMode, 0.5) >= 0.5)
                    if (outVertex.IsDependentOn(lastAddedVertices))
                    {
                        verticesToAdd.Add(outVertex);
                    }
                }

                if (verticesToAdd.Count > 0)
                {
                    this.AllVertices.UnionWith(verticesToAdd);
                    this.InnerMembers.UnionWith(verticesToAdd);
                    if (this.AllVertices.Count > maxSize)
                    {
                        this.clearAll();
                        return;
                    }

                    this.InnerZone.UnionWith(verticesToAdd);
                    neighborhood.ExceptWith(verticesToAdd);
                    this.updateNeighborhood(neighborhood, verticesToAdd);
                }
                else
                {
                    break;
                }
            }

            if (outerZone)
            {
                //vrcholy, na kterych je zavisly alespon jeden vrchol z InnerZony (nezavisla OuterZona)
                this.updateNeighborhood(neighborhood, verticesToAdd);
                verticesToAdd.Clear();
                foreach (Vertex outVertex in neighborhood)
                {
                    foreach (Vertex inVertex in this.InnerZone)
                    {
                        if (inVertex.IsDependentOn(outVertex))
                        {
                            verticesToAdd.Add(outVertex);
                            break;
                        }
                    }
                }

                if (verticesToAdd.Count > 0)
                {
                    this.AllVertices.UnionWith(verticesToAdd);
                    if (this.AllVertices.Count > maxSize)
                    {
                        this.clearAll();
                        return;
                    }

                    this.OuterZone.UnionWith(verticesToAdd);
                }

                if (this.AllVertices.Count < minSize)
                {
                    this.clearAll();
                    return;
                }

                this.calculateLiaisons();
            }
        }

        private void clearAll()
        {
            //this.Seeds.Clear();

            this.AllVertices.Clear();

            this.InnerZone.Clear();
            this.OuterZone.Clear();

            this.Egos.Clear();
            this.InnerMembers.Clear();
            this.Liaisons.Clear();
            this.CoLiaisons.Clear();
        }

        private void updateNeighborhood(HashSet<Vertex> neighborhood, HashSet<Vertex> addedVertices)
        {
            foreach (Vertex v in addedVertices)
            {
                foreach (Vertex adj in v.AdjacentVertices)
                {
                    if (!this.AllVertices.Contains(adj))
                    {
                        neighborhood.Add(adj);
                    }
                }
            }
        }

        public void calculateLiaisons()
        {
            foreach (Vertex v in this.OuterZone)
            {
                bool isLiaison = true;
                foreach (Vertex outer in this.OuterZone)
                {
                    if (v.IsDependentOn(outer))
                    {
                        isLiaison = false;
                        this.CoLiaisons.Add(v);
                        break;
                    }
                }

                if (isLiaison)
                {
                    this.Liaisons.Add(v);
                }
            }
        }

        public class ZoneModularity
        {
            public readonly DependencyZone Zone;
            public readonly double BasicValue;
            public readonly double InnerValue;
            public readonly double ZoneValue;

            public ZoneModularity(DependencyZone zone)
            {
                this.Zone = zone;

                ////////////////////// zakladni modularita
                HashSet<Vertex> boundary = new HashSet<Vertex>();
                foreach (Vertex v in this.Zone.AllVertices)
                {
                    foreach (Vertex adj in v.AdjacentVertices)
                    {
                        if (!this.Zone.AllVertices.Contains(adj))
                        {
                            boundary.Add(v);
                            break;
                        }
                    }
                }

                double outEdgesWeight = 0;
                HashSet<Edge> inEdges = new HashSet<Edge>();
                //foreach (Vertex v in this.AllVertices)
                foreach (Vertex v in boundary)
                {
                    double outEw = 0;
                    HashSet<Edge> inE = new HashSet<Edge>();
                    foreach (Edge e in v.AdjacentEdges)
                    {
                        if (!this.Zone.AllVertices.Contains(e.GetAdjacent(v)))
                        {
                            outEw += e.Weight;
                        }
                        else
                        {
                            inE.Add(e);
                        }
                    }

                    outEdgesWeight += outEw;
                    foreach (Edge ee in inE)
                    {
                        inEdges.Add(ee);
                    }
                }

                double allEdgesWeight = outEdgesWeight;
                foreach (Edge e in inEdges)
                {
                    allEdgesWeight += e.Weight;
                }

                if (allEdgesWeight == 0)
                {
                    this.BasicValue = 0;
                }
                else
                {
                    this.BasicValue = 1 - outEdgesWeight / allEdgesWeight;
                }

                ///////////////////// modularita inner zony
                boundary = new HashSet<Vertex>();
                foreach (Vertex v in this.Zone.InnerZone)
                {
                    foreach (Vertex adj in v.AdjacentVertices)
                    {
                        if (!this.Zone.InnerZone.Contains(adj))
                        {
                            boundary.Add(v);
                            break;
                        }
                    }
                }

                outEdgesWeight = 0;
                inEdges = new HashSet<Edge>();
                foreach (Vertex v in boundary)
                {
                    double outEw = 0;
                    HashSet<Edge> inE = new HashSet<Edge>();
                    foreach (Edge e in v.AdjacentEdges)
                    {
                        if (!this.Zone.InnerZone.Contains(e.GetAdjacent(v)))
                        {
                            outEw += e.Weight;
                        }
                        else
                        {
                            inE.Add(e);
                        }
                    }

                    outEdgesWeight += outEw;
                    foreach (Edge ee in inE)
                    {
                        inEdges.Add(ee);
                    }
                }

                allEdgesWeight = outEdgesWeight;
                foreach (Edge e in inEdges)
                {
                    allEdgesWeight += e.Weight;
                }

                if (allEdgesWeight == 0)
                {
                    this.InnerValue = 0;
                }
                else
                {
                    this.InnerValue = 1 - outEdgesWeight / allEdgesWeight;
                }

                /////////////////////// modularita zony
                if (this.BasicValue == 0 && this.InnerValue == 0)
                {
                    this.ZoneValue = 0;
                }
                else
                {
                    double bm = this.BasicValue;
                    double im = this.InnerValue;
                    this.ZoneValue = Math.Sqrt((bm * bm + im * im) / 2);
                }
            }
        }
        public ZoneModularity Modularity
        {
            get
            {
                if (this.modularity == null)
                {
                    this.modularity = new ZoneModularity(this);
                }

                return this.modularity;
            }
        }

        public bool IsEqualTo(DependencyZone zone, DependencyZone.DuplicityFilter filter)
        {
            switch (filter)
            {
                case DuplicityFilter.MultiEgo:
                    return AreEqual(this.InnerZone, zone.InnerZone, filter);
                case DuplicityFilter.All:
                    return AreEqual(this.AllVertices, zone.AllVertices, filter);
                default:
                    return false;
            }
        }

        public static bool AreEqual(HashSet<Vertex> setA, HashSet<Vertex> setB, DependencyZone.DuplicityFilter filter)
            //ignoruje MultiEgo, povazuje ho za All
        {
            if (filter == DuplicityFilter.None || setA.Count != setB.Count)
            {
                return false;
            }
            else
            {
                foreach (Vertex v in setA)
                {
                    if (!setB.Contains(v))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public enum Relationship
        {
            None, Equal, Alternative, Overlap, Super, Sub
        }
        public Relationship GetRelationship(DependencyZone zone, out HashSet<Vertex> overlap)
        {
            overlap = new HashSet<Vertex>(this.AllVertices.Intersect(zone.AllVertices));

            if (this == zone)
            {
                return Relationship.Equal;
            }

            if (overlap.Count == 0)
            {
                return Relationship.None;
            }
            else if (this.AllVertices.Count > zone.AllVertices.Count)
            {
                if (overlap.Count == zone.AllVertices.Count)
                {
                    return Relationship.Super;
                }
                else
                {
                    return Relationship.Overlap;
                }
            }
            else if (this.AllVertices.Count < zone.AllVertices.Count)
            {
                if (overlap.Count == this.AllVertices.Count)
                {
                    return Relationship.Sub;
                }
                else
                {
                    return Relationship.Overlap;
                }
            }
            else
            {
                if (overlap.Count == zone.AllVertices.Count)
                {
                    return Relationship.Alternative; ;
                }
                else
                {
                    return Relationship.Overlap;
                }
            }
        }

        public HashSet<Vertex> GetOverlapWithoutEgos(DependencyZone zone)
        {
            HashSet<Vertex> overlap;
            this.GetRelationship(zone, out overlap);
            overlap.ExceptWith(zone.Egos);

            return overlap;
        }

        public List<string> GetNames()
        {
            List<string> names = new List<string>();
            names.Add(this.ego.Name);
            foreach (Vertex v in this.Egos)
            {
                if (v != this.ego)
                {
                    names.Add(v.Name);
                }
            }
            foreach (Vertex v in this.Liaisons)
            {
                names.Add(v.Name);
            }
            foreach (Vertex v in this.CoLiaisons)
            {
                names.Add(v.Name);
            }
            foreach (Vertex v in this.InnerMembers)
            {
                names.Add(v.Name);
            }

            return names;
        }

        public enum ZonesOverlapType
        {
            Cover, Overlap, Equal
        }
        public class ZonesOverlap
        {
            public readonly ZonesOverlapType Type;

            public readonly DependencyZone ZoneA;
            public readonly DependencyZone ZoneB;
            public readonly DependencyZone LargeZone;
            public readonly DependencyZone SmallZone;
            public readonly HashSet<Vertex> Overlap;
            public readonly double ZoneADensity;
            public readonly double ZoneBDensity;
            public readonly double OverlapDensity;
            public readonly double ZoneAExceptOverlapDensity;
            public readonly double ZoneBExceptOverlapDensity;

            public ZonesOverlap(DependencyZone zoneA, DependencyZone zoneB)
            {
                this.ZoneA = zoneA;
                this.ZoneB = zoneB;

                if (this.ZoneA.AllVertices.Count >= this.ZoneB.AllVertices.Count)
                {
                    this.LargeZone = this.ZoneA;
                    this.SmallZone = this.ZoneB;
                }
                else
                {
                    this.LargeZone = this.ZoneB;
                    this.SmallZone = this.ZoneA;
                }

                this.Overlap = new HashSet<Vertex>(ZoneA.AllVertices.Intersect(ZoneB.AllVertices));

                if (this.Overlap.Count == this.SmallZone.AllVertices.Count)
                {
                    if (this.Overlap.Count == this.LargeZone.AllVertices.Count)
                    {
                        this.Type = ZonesOverlapType.Equal;
                    }
                    else
                    {
                        this.Type = ZonesOverlapType.Cover;
                    }
                }
                else
                {
                    this.Type = ZonesOverlapType.Overlap;
                }

                this.ZoneADensity = Network.GetDensity(this.ZoneA.AllVertices);
                this.ZoneBDensity = Network.GetDensity(this.ZoneB.AllVertices);
                this.ZoneAExceptOverlapDensity = Network.GetDensity(this.ZoneA.AllVertices.Except(this.Overlap));
                this.ZoneBExceptOverlapDensity = Network.GetDensity(this.ZoneB.AllVertices.Except(this.Overlap));
                this.OverlapDensity = Network.GetDensity(this.Overlap);
            }

            public double GetDensityRatio(bool skipOverlap)
            {
                double densityA = this.ZoneADensity;
                double densityB = this.ZoneBDensity;
                if (skipOverlap)
                {
                    densityA = this.ZoneAExceptOverlapDensity;
                    densityB = this.ZoneBExceptOverlapDensity;
                }


                double density;
                HashSet<Vertex> verticesA = ZoneA.AllVertices;
                HashSet<Vertex> verticesB = ZoneB.AllVertices;

                if (verticesA.Count >= verticesB.Count && verticesA.Intersect(verticesB).Count() == verticesB.Count)
                {
                    density = densityA;
                }
                else if (verticesB.Count > verticesA.Count && verticesB.Intersect(verticesA).Count() == verticesA.Count)
                {
                    density = densityB;
                }
                else
                {
                    density = Math.Max(densityA, densityB);
                }

                if (density == 0)
                {
                    return -1;
                }
                else
                {
                    return this.OverlapDensity / density;
                }
            }

            public double GetOverlapRatio()
            {
                if (this.Type == ZonesOverlapType.Cover)
                {
                    return (double)this.SmallZone.AllVertices.Count / this.LargeZone.AllVertices.Count;
                }
                else
                {
                    return (double)this.Overlap.Count / this.SmallZone.AllVertices.Count;
                }
            }
        }

    }

}
