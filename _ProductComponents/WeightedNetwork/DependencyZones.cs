using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightedNetwork
{
    public class DependencyZones
    {
        public enum ProminencyFilter
        {
            None, AllProminents, StrongProminents, WeakProminents
        }

        public class VertexMemberships
        {
            public readonly HashSet<DependencyZone> Membership = new HashSet<DependencyZone>();
            public readonly HashSet<DependencyZone> InnerMembership = new HashSet<DependencyZone>();
            public readonly HashSet<DependencyZone> Liaisonship = new HashSet<DependencyZone>();
            public readonly HashSet<DependencyZone> CoLiaisonship = new HashSet<DependencyZone>();
        }

        public readonly DependencyZone.DuplicityFilter DUPLICITY_FILTER = DependencyZone.DuplicityFilter.MultiEgo;
        public readonly ProminencyFilter PROMINENCY_FILTER = ProminencyFilter.None;
        public readonly Network Network;
        public readonly double DependencyThreshold;
        private Dictionary<Vertex, DependencyZone> egoToZoneDictionary;

        public readonly List<DependencyZone> AllZones;
        public readonly Dictionary<Vertex, VertexMemberships> Memberships;
        public readonly Dictionary<DependencyZone, DependencyZoneRelationships> Relationships;

        private DependencyZone[] tmpZones = null;
        private Object relLock = new object();

        public DependencyZones(Network net, int minSize, int maxSize, bool outerZone,
            DependencyZone.DuplicityFilter duplicityFilter, ProminencyFilter prominencyFilter, double dependencyThreshold)
        {
            this.Network = net;
            this.egoToZoneDictionary = new Dictionary<Vertex, DependencyZone>();

            this.AllZones = new List<DependencyZone>();
            this.Memberships = new Dictionary<Vertex, VertexMemberships>();
            this.Relationships = new Dictionary<DependencyZone, DependencyZoneRelationships>();

            this.DUPLICITY_FILTER = duplicityFilter;
            this.PROMINENCY_FILTER = prominencyFilter;
            this.DependencyThreshold = dependencyThreshold;

            this.calculateZones(minSize, maxSize, outerZone);
            this.CompleteAndFilterDuplicities();
        }

        public DependencyZones(bool index, Network net, int minSize, int maxSize, bool outerZone,
            DependencyZone.DuplicityFilter duplicityFilter, ProminencyFilter prominencyFilter, double dependencyThreshold)
        {
            this.Network = net;
            this.egoToZoneDictionary = new Dictionary<Vertex, DependencyZone>();

            this.AllZones = new List<DependencyZone>();
            this.Memberships = new Dictionary<Vertex, VertexMemberships>();
            this.Relationships = new Dictionary<DependencyZone, DependencyZoneRelationships>();

            this.DUPLICITY_FILTER = duplicityFilter;
            this.PROMINENCY_FILTER = prominencyFilter;
            this.DependencyThreshold = dependencyThreshold;

            this.calculateZones(minSize, maxSize, outerZone);
            if (index)
            {
                this.CompleteAndFilterDuplicities();
            }
        }

        private void calculateZones(int minSize, int maxSize, bool outerZone)
        {
            int[] ids = new int[this.Network.Vertices.Values.Count];
            int index = 0;
            foreach (Vertex v in this.Network.Vertices.Values)
            {
                ids[index] = v.Id;
                index += 1;
            }
            Array.Sort(ids);

            Dictionary<int, int> indexes = new Dictionary<int, int>();
            for (index = 0; index < ids.Length; index++)
            {
                indexes.Add(ids[index], index);
            }

            this.tmpZones = new DependencyZone[indexes.Count];
            //foreach (int id in ids)
            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(ids, options, id =>
            {
                DependencyZone newZone = null;

                Vertex v = this.Network.Vertices[id];
                bool toDetect = true;
                if (this.PROMINENCY_FILTER == ProminencyFilter.AllProminents && v.GetProminency().GetValue() == 0)
                {
                    toDetect = false;
                }
                else if (this.PROMINENCY_FILTER == ProminencyFilter.StrongProminents && v.GetProminency().GetValue() < 1)
                {
                    toDetect = false;
                }
                else if (this.PROMINENCY_FILTER == ProminencyFilter.WeakProminents && (v.GetProminency().GetValue() == 1 || v.GetProminency().GetValue() == 0))
                {
                    toDetect = false;
                }
                if (toDetect)
                {
                    newZone = new DependencyZone(this.Network, v, minSize, maxSize, outerZone);
                    if (newZone.AllVertices.Count == 0)
                    {
                        newZone = null;
                    }
                }
                this.tmpZones[indexes[id]] = newZone;
           });
        }

        public void CompleteAndFilterDuplicities()
        {
            Dictionary<int, HashSet<DependencyZone>> zoneIndex = new Dictionary<int, HashSet<DependencyZone>>();

            int id = 0;
            foreach (DependencyZone newZone in this.tmpZones)
            {
                if (newZone != null && newZone.Dependency.DependencyScore >= this.DependencyThreshold)
                {
                    int N = newZone.AllVertices.Count;
                    HashSet<DependencyZone> indexedZonesForN;
                    Vertex firstInnerVertex = null;
                    Vertex secondInnerVertex = null;
                    if (newZone.InnerZone.Count == 2) //kvuli testu na indexovani
                    {
                        firstInnerVertex = newZone.InnerZone.ToList()[0];
                        secondInnerVertex = newZone.InnerZone.ToList()[1];
                    }

                    if (this.DUPLICITY_FILTER == DependencyZone.DuplicityFilter.None) //neni potreba indexovat
                    {
                        id += 1;
                        this.addNewZone(id, newZone);
                    }
                    else if (newZone.InnerZone.Count == 1) //neni potreba indexovat
                    {
                        id += 1;
                        this.addNewZone(id, newZone);
                    }
                    else if (newZone.InnerZone.Count == 2 && //neni potreba indexovat
                        !(firstInnerVertex.IsDependentOn(secondInnerVertex) && secondInnerVertex.IsDependentOn(firstInnerVertex))) 
                    {
                        id += 1;
                        this.addNewZone(id, newZone);
                    }
                    else if (!zoneIndex.TryGetValue(N, out indexedZonesForN))
                    {
                        indexedZonesForN = new HashSet<DependencyZone>();
                        zoneIndex.Add(N, indexedZonesForN);

                        indexedZonesForN.Add(newZone);

                        id += 1;
                        this.addNewZone(id, newZone);
                    }
                    else
                    {
                        DependencyZone existingZone = null;

                        foreach (DependencyZone nZone in indexedZonesForN)
                        {
                            if (newZone.IsEqualTo(nZone, this.DUPLICITY_FILTER))
                            {
                                existingZone = nZone;
                                break;
                            }
                        }

                        if (existingZone == null)
                        {
                            indexedZonesForN.Add(newZone);

                            id += 1;
                            this.addNewZone(id, newZone);
                        }
                        else
                        {
                            existingZone.AddEgo(newZone.Ego);
                            this.egoToZoneDictionary.Add(newZone.Ego, existingZone);
                        }
                    }
                }
            }

            this.tmpZones = null;
        }

        private void addNewZone(int id, DependencyZone zone)
        {
            zone.Id = id;
            this.AllZones.Add(zone);
            this.egoToZoneDictionary.Add(zone.Ego, zone);
        }

        public void CalculateMemberships()
        {
            foreach (Vertex v in this.Network.Vertices.Values)
            {
                this.Memberships.Add(v, new VertexMemberships());
            }

            foreach (DependencyZone zone in this.AllZones)
            {
                foreach (Vertex v in zone.AllVertices)
                {
                    this.Memberships[v].Membership.Add(zone);
                }
                foreach (Vertex v in zone.InnerMembers)
                {
                    this.Memberships[v].InnerMembership.Add(zone);
                }
                foreach (Vertex v in zone.Liaisons)
                {
                    this.Memberships[v].Liaisonship.Add(zone);
                }
                foreach (Vertex v in zone.CoLiaisons)
                {
                    this.Memberships[v].CoLiaisonship.Add(zone);
                }
            }
        }

        public void CalculateRelationships()
        {
            if (this.Relationships.Count > 0)
            {
                return;
            }

            DependencyZoneRelationships[] relationshipsArray = new DependencyZoneRelationships[this.AllZones.Count];
            Parallel.For(0, this.AllZones.Count, i =>
            {
                DependencyZone zone = this.AllZones[i];
                DependencyZoneRelationships relationship = new DependencyZoneRelationships(zone, this);
                lock (relLock)
                {
                    relationshipsArray[i] = relationship;
                }
            });

            foreach (DependencyZoneRelationships relationship in relationshipsArray)
            {
                this.Relationships.Add(relationship.Zone, relationship);
            }
        }

        public DependencyZone GetZone(Vertex ego)
        {
            if (!this.egoToZoneDictionary.ContainsKey(ego))
            {
                return null;
            }

            DependencyZone zone = this.egoToZoneDictionary[ego];
            return zone;
        }

        public DependencyZone GetZone(int id)
        {
            if (id < 1 || id > this.AllZones.Count)
            {
                return null;
            }
            else
            {
                return this.AllZones[id - 1];
            }
        }

    }

}
