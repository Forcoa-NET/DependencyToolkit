using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightedNetwork;

namespace DependencyZones
{
    class Statistics
    {
        public readonly int NodesCount = 0;//
        public readonly int EdgesCount = 0;//
        public readonly int ZonesCount = 0;//

        public int MaxZoneSize = 0;//
        public int SumZoneSize = 0;//
        public int MaxInnerZoneSize = 0;//
        public int SumInnerZoneSize = 0;//
        public int MaxOuterZoneSize = 0;//
        public int SumOuterZoneSize = 0;//
        public int TrivialZonesCount = 0;//
        public int DyadZonesCount = 0;//
        public int TriadZonesCount = 0;//
        public int MultiEgoZonesCount = 0;//
        public int MaxMultiEgoZoneEgos = 0;//
        public int SumMultiEgoZoneEgos = 0;//
        public int AltZonesCount = 0;//
        public int MaxSubZonesCount = 0;//
        public int SumSubZonesCount = 0;//
        public int MaxOverlappingZonesCount = 0;//
        public int SumOverlappingZonesCount = 0;//
        public double SumEmbeddedness = 0;//
        public int NonTrivialEmbeddednessCount = 0;//
        public int TopZonesCount = 0;//
        public int InnerZonesCount = 0;//
        public int BottomZonesCount = 0;//
        public int IndependentZonesCount = 0;//

        public int MaxDegree = 0;//
        public int ProminentsCount = 0;//
        public int SubProminentsCount = 0;//
        public int MaxMembershipCount = 0;//
        public int SumMembreshipCount = 0;//
        public int MaxLiaisonshipCount = 0;//
        public int SumLiaisonshipCount = 0;//
        public int MaxCoLiaisonshipCount = 0;//
        public int SumCoLiaisonshipCount = 0;//
        public int MaxOwDep = 0;//
        public int SumOwDep = 0;//
        public int MaxOwIndep = 0;//
        public int SumOwIndep = 0;//
        public int MaxTwDep = 0;//
        public int SumTwDep = 0;//
        public int MaxTwIndep = 0;//
        public int SumTwIndep = 0;//

        public int OverlapsCount = 0;//
        public int MaxOverlapSize = 0;//
        public int SumOverlapSize = 0;//
        public int ZoneInOverlapsCount = 0;//
        public int MaxZoneInOverlapsSize = 0;//
        public int SumZoneInOverlapsSize = 0;//

        public int CommunitiesCount = 0;//
        public int MaxCommunitySize = 0;//
        public int SumCommunitySize = 0;//
        public double SumCommunityEmbeddedness = 0;
        public double Modularity = 0;
        public int MaxCommZoneSize = 0;//
        public int SumCommZoneSize = 0;//
        public double SumCommZoneEmbeddedness = 0;
        public double MaxFit = 0;//
        public double SumFit = 0;//
        public int SumCommZoneOverlapSize = 0;//

        public Statistics(WeightedNetwork.DependencyZones zones)
        {
            this.NodesCount = zones.Network.Vertices.Count;
            this.EdgesCount = zones.Network.Edges.Count;
            this.ZonesCount = zones.AllZones.Count;
        }
    }
}
