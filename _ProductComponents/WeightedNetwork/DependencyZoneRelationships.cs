using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public enum ZoneCategory
    {
        TopZone, BottomZone, InnerZone, OverlappingZone, IndependentZone, OutlyingZone
    }

    public class DependencyZoneRelationships
    {
        public readonly DependencyZone Zone;

        public readonly Dictionary<int, DependencyZone> SuperZones = new Dictionary<int, DependencyZone>();
        public readonly Dictionary<int, DependencyZone> SubZones = new Dictionary<int, DependencyZone>();
        public readonly Dictionary<int, DependencyZone> OverlappingZones = new Dictionary<int, DependencyZone>();
        public readonly Dictionary<int, DependencyZone> AlternativeZones = new Dictionary<int, DependencyZone>();

        public DependencyZoneRelationships(DependencyZone zone, DependencyZones allZones)
        {
            this.Zone = zone;
            foreach (DependencyZone aZone in allZones.AllZones)
            {
                HashSet<Vertex> overlap;
                if (aZone != this.Zone)
                {
                    switch (aZone.GetRelationship(this.Zone, out overlap))
                    {
                        case DependencyZone.Relationship.Super:
                            this.SuperZones.Add(aZone.Id, aZone);
                            break;
                        case DependencyZone.Relationship.Sub:
                            this.SubZones.Add(aZone.Id, aZone);
                            break;
                        case DependencyZone.Relationship.Overlap:
                            this.OverlappingZones.Add(aZone.Id, aZone);
                            break;
                        case DependencyZone.Relationship.Alternative:
                            this.AlternativeZones.Add(aZone.Id, aZone);
                            break;
                    }
                }
            }
        }

        public ZoneCategory GetCategory()
        {
            if (this.SuperZones.Count == 0 && this.SubZones.Count > 0)
            {
                return ZoneCategory.TopZone;
            }
            else if (this.SubZones.Count == 0 && this.SuperZones.Count > 0)
            {
                return ZoneCategory.BottomZone;
            }
            else if (this.SuperZones.Count > 0 && this.SubZones.Count > 0)
            {
                return ZoneCategory.InnerZone;
            }
            else if (this.OverlappingZones.Count > 0)
            {
                return ZoneCategory.OverlappingZone; //nenastava
            }
            else if (this.isIndependentZone())
            {
                return ZoneCategory.IndependentZone;
            }
            else
            {
                return ZoneCategory.OutlyingZone;
            }
        }

        private bool isIndependentZone()
        {
            foreach (Vertex v in this.Zone.AllVertices)
            {
                foreach (Vertex adj in v.AdjacentVertices)
                {
                    if (!this.Zone.AllVertices.Contains(adj))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Dictionary<int, DependencyZone> GetNearestSuperZones(Dictionary<DependencyZone, DependencyZoneRelationships> allRelationships)
        {
            HashSet<DependencyZone> toRemoveZones = new HashSet<DependencyZone>();
            foreach (DependencyZone superZone in this.SuperZones.Values)
            {
                foreach (DependencyZone zone in this.SuperZones.Values)
                {
                    if (superZone != zone)
                    {
                        if (allRelationships[zone].SuperZones.ContainsKey(superZone.Id))
                        {
                            toRemoveZones.Add(superZone);
                        }
                    }
                }
            }

            Dictionary<int, DependencyZone> nearestSuperZones = new Dictionary<int, DependencyZone>();
            foreach (KeyValuePair<int, DependencyZone> p in this.SuperZones)
            {
                if (!toRemoveZones.Contains(p.Value))
                {
                    nearestSuperZones.Add(p.Key, p.Value);
                }
            }

            return nearestSuperZones;
        }

    }

}
