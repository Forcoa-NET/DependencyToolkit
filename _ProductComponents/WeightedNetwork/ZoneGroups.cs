using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightedNetwork
{
    public class ZoneGroups
    {
        public enum GroupingStrategy
        {
            Inner, InnerAndDependentOuter, InnerAndIndependentOuter, InnerAndOuter, DependentOuter, IndependentOuter, Outer
        }

        public readonly DependencyZone.DuplicityFilter DUPLICITY_FILTER;

        private DependencyZones zones;

        public readonly List<ZoneGroup> AllGroups;
        public readonly GroupingStrategy Strategy;
        public readonly bool NonZonesOnly;
        public readonly double DependencyThreshold;

        private ZoneGroup[] tmpGroups = null;

        public ZoneGroups(DependencyZones zones, GroupingStrategy strategy, bool nonZonesOnly, double dependencyThreshold)
        {
            this.zones = zones;
            this.DUPLICITY_FILTER = this.zones.DUPLICITY_FILTER;
            if (this.DUPLICITY_FILTER == DependencyZone.DuplicityFilter.MultiEgo)
            {
                this.DUPLICITY_FILTER = DependencyZone.DuplicityFilter.All;
            }

            this.AllGroups = new List<ZoneGroup>();
            this.Strategy = strategy;
            this.NonZonesOnly = nonZonesOnly;
            this.DependencyThreshold = dependencyThreshold;

            this.calculateGroups();
            this.CompleteAndFilterDuplicities();
        }

        public ZoneGroups(bool index,  DependencyZones zones, GroupingStrategy strategy, bool nonZonesOnly, double dependencyThreshold)
        {
            this.zones = zones;
            this.DUPLICITY_FILTER = this.zones.DUPLICITY_FILTER;
            if (this.DUPLICITY_FILTER == DependencyZone.DuplicityFilter.MultiEgo)
            {
                this.DUPLICITY_FILTER = DependencyZone.DuplicityFilter.All;
            }

            this.AllGroups = new List<ZoneGroup>();
            this.Strategy = strategy;
            this.NonZonesOnly = nonZonesOnly;
            this.DependencyThreshold = dependencyThreshold;

            this.calculateGroups();
            if (index)
            {
                this.CompleteAndFilterDuplicities();
            }
        }

        private void calculateGroups()
        {
            tmpGroups = new ZoneGroup[this.zones.AllZones.Count];
            Parallel.For(0, this.zones.AllZones.Count, i =>
            //for (int i = 0; i < this.zones.AllZones.Count; i++)
            {
                DependencyZone zone = this.zones.AllZones[i];
                HashSet<Vertex> vertices = new HashSet<Vertex>();
                switch (this.Strategy)
                {
                    case GroupingStrategy.Inner:
                        vertices.UnionWith(zone.InnerZone);
                        break;
                    case GroupingStrategy.InnerAndDependentOuter:
                        vertices.UnionWith(zone.InnerZone);
                        vertices.UnionWith(zone.CoLiaisons);
                        break;
                    case GroupingStrategy.InnerAndIndependentOuter:
                        vertices.UnionWith(zone.InnerZone);
                        vertices.UnionWith(zone.Liaisons);
                        break;
                    case GroupingStrategy.InnerAndOuter:
                        vertices.UnionWith(zone.InnerZone);
                        vertices.UnionWith(zone.OuterZone);
                        break;
                    case GroupingStrategy.DependentOuter:
                        vertices.UnionWith(zone.CoLiaisons);
                        break;
                    case GroupingStrategy.IndependentOuter:
                        vertices.UnionWith(zone.Liaisons);
                        break;
                    case GroupingStrategy.Outer:
                        vertices.UnionWith(zone.OuterZone);
                        break;
                    default:
                        break;
                }

                this.tmpGroups[i] = new ZoneGroup(zone);
                foreach (Vertex v in vertices)
                {
                    DependencyZone z = this.zones.GetZone(v);
                    if (z != null)
                    {
                        this.tmpGroups[i].AddZone(z);
                    }
                }
            });
        }

        public void CompleteAndFilterDuplicities()
        {
            Dictionary<int, List<ZoneGroup>> groupIndex = new Dictionary<int, List<ZoneGroup>>();
            int id = this.zones.AllZones.Count;
            foreach (ZoneGroup newGroup in this.tmpGroups)
            {
                if (!(this.NonZonesOnly && newGroup.IsZone) && newGroup.Dependency.DependencyScore >= this.DependencyThreshold)
                {
                    int n = newGroup.AllVertices.Count;
                    if (this.DUPLICITY_FILTER == DependencyZone.DuplicityFilter.None) //neni potreba indexovat
                    {
                        id += 1;
                        newGroup.Id = id;
                        this.AllGroups.Add(newGroup);
                    }
                    else if (n == 1) //neni potreba indexovat
                    {
                        id += 1;
                        newGroup.Id = id;
                        this.AllGroups.Add(newGroup);
                    }
                    else
                    {
                        List<ZoneGroup> nGroups;
                        if (!groupIndex.TryGetValue(n, out nGroups))
                        {
                            nGroups = new List<ZoneGroup>();
                            groupIndex.Add(n, nGroups);
                        }

                        bool existingGroup = false;
                        foreach (ZoneGroup nGroup in nGroups)
                        {
                            if (DependencyZone.AreEqual(newGroup.AllVertices, nGroup.AllVertices, this.DUPLICITY_FILTER))
                            {
                                existingGroup = true;
                                break;
                            }
                        }
                        if (!existingGroup)
                        {
                            nGroups.Add(newGroup);

                            id += 1;
                            newGroup.Id = id;
                            this.AllGroups.Add(newGroup);
                        }
                    }
                }
            }
        }

    }

}
