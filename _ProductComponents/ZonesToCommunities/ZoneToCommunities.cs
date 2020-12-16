using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightedNetwork;
using LouvainCommunityPL;

namespace ZonesToCommunities
{
    public class ZoneToCommunities
    {
        public readonly List<IDependencyGroup> AllGroups;
        public readonly Network Network;

        private Communities communities = null;

        public ZoneToCommunities(List<IDependencyGroup> allGroups, Network network)
        {
            this.AllGroups = allGroups;
            this.Network = network;
        }

        public Communities Communities
        {
            get
            {
                if (this.communities == null)
                {
                    this.Detect();
                }

                return this.communities;
            }
        }

        public void Detect(string groundTruthName)
        {
            this.communities = new Communities(groundTruthName, this.Network);
        }

        public void Detect()
        {
            Graph g = new Graph();
            foreach (Edge e in this.Network.Edges)
            {
                g.AddEdge(e.VertexA.Id, e.VertexB.Id, e.Weight);
            }

            this.communities = new Communities(g, this.Network);
            this.communities.Detect();
        }

        public class ZoneFitResult
        {
            public readonly IDependencyGroup Group;
            public double Value = -2;
            public int CommunityId = -1;
            public int CommunityCount = 0;
            public int OverlapCount = 0;

            public ZoneFitResult(IDependencyGroup group)
            {
                if (group == null)
                {
                    this.Group = null;
                }
                else
                {
                    this.Group = group;
                }
            }
        }
        public ZoneFitResult GetBestZoneFit(IDependencyGroup zone) //MCC
        {
            ZoneFitResult FIT = new ZoneFitResult(zone);
            foreach (KeyValuePair<int, List<int>> pair in this.Communities.AllCommunities)
            {
                int TP = 0;
                foreach (int id in pair.Value)
                {
                    if (zone.AllVertices.Contains(this.Network.Vertices[id]))
                    {
                        TP += 1;
                    }
                }
                ////Puvodni clanek
                //double FN = zone.AllVertices.Count - TP;
                //double FP = pair.Value.Count - TP;
                double FN = zone.AllVertices.Count - TP;
                double FP = pair.Value.Count - TP;
                double TN = this.Network.Vertices.Count - FP - FN - TP;

                double num = TP * TN - FP * FN;
                double denom = (TP + FP) * (TP + FN) * (TN + FP) * (TN + FN);

                double fit = num / Math.Sqrt(denom);
                if (fit > FIT.Value)
                {
                    FIT.Value = fit;
                    FIT.CommunityId = pair.Key;
                    FIT.CommunityCount = pair.Value.Count;
                    FIT.OverlapCount = TP;
                }

                if (FIT.Value == 1)
                {
                    return FIT;
                }
            }

            return FIT;
        }
        //public FitResult GetBestFit(DependencyZone zone) //F1 Score
        //{
        //    FitResult FIT = new FitResult(zone);
        //    foreach (KeyValuePair<int, List<int>> pair in this.Communities.AllCommunities)
        //    {
        //        int TP = 0;
        //        foreach (int id in pair.Value)
        //        {
        //            if (zone.AllVertices.Contains(this.Network.Vertices[id]))
        //            {
        //                TP += 1;
        //            }
        //        }

        //        double FN = zone.AllVertices.Count - TP;
        //        double FP = pair.Value.Count - TP;

        //        double P = TP / (TP + FP);
        //        double R = TP / (TP + FN);
        //        double F1score = P + R;
        //        if (F1score > 0)
        //        {
        //            F1score = 2 * P * R / F1score;
        //        }

        //        if (F1score > FIT.Value)
        //        {
        //            FIT.Value = F1score;
        //            FIT.CommunityId = pair.Key;
        //            FIT.CommunityCount = pair.Value.Count;
        //            FIT.OverlapCount = TP;
        //        }

        //        if (FIT.Value == 1)
        //        {
        //            return FIT;
        //        }
        //    }

        //    return FIT;
        //}

        private Object zoneFitLock = new object();
        public Dictionary<IDependencyGroup, ZoneFitResult> GetZoneFitResults()
        {
            ZoneFitResult[] results = new ZoneFitResult[this.AllGroups.Count];
            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            //Parallel.ForEach(this.AllGroups, options, group =>
            foreach (IDependencyGroup group in this.AllGroups)
            {
                ZoneFitResult mcc = this.GetBestZoneFit(group);
                lock (zoneFitLock)
                {
                    results[group.Id - 1] = mcc;
                }
            }//);

            Dictionary<IDependencyGroup, ZoneFitResult>  fitResults = new Dictionary<IDependencyGroup, ZoneFitResult>();
            for (int i = 0; i < results.Length; i++)
            {
                fitResults.Add(this.AllGroups[i], results[i]);
            }

            return fitResults;
        }

        ////Rozpracovano pro komunity
        //public class CommunityFitResult
        //{
        //    public readonly KeyValuePair<int, List<int>> Community;
        //    public double Value = -2;
        //    public int ZoneId = -1;
        //    public int ZoneCount = 0;
        //    public int OverlapCount = 0;

        //    public CommunityFitResult(KeyValuePair<int, List<int>> community)
        //    {
        //        this.Community = community;
        //    }
        //}
        //public CommunityFitResult GetBestCommunityFit(KeyValuePair<int, List<int>> community) //MCC
        //{
        //    CommunityFitResult FIT = new CommunityFitResult(community);
        //    foreach (KeyValuePair<int, List<int>> pair in this.Communities.AllCommunities)
        //    {
        //        int TP = 0;
        //        foreach (int id in pair.Value)
        //        {
        //            if (community.Value.Contains(this.Network.Vertices[id]))
        //            {
        //                TP += 1;
        //            }
        //        }
        //        ////Puvodni clanek
        //        //double FN = zone.AllVertices.Count - TP;
        //        //double FP = pair.Value.Count - TP;
        //        double FP = zone.AllVertices.Count - TP;
        //        double FN = pair.Value.Count - TP;
        //        double TN = this.Network.Vertices.Count - FP - FN - TP;

        //        double num = TP * TN - FP * FN;
        //        double denom = (TP + FP) * (TP + FN) * (TN + FP) * (TN + FN);

        //        double fit = num / Math.Sqrt(denom);
        //        if (fit > FIT.Value)
        //        {
        //            FIT.Value = fit;
        //            FIT.CommunityId = pair.Key;
        //            FIT.CommunityCount = pair.Value.Count;
        //            FIT.OverlapCount = TP;
        //        }

        //        if (FIT.Value == 1)
        //        {
        //            return FIT;
        //        }
        //    }

        //    return FIT;
        //}
        ////public FitResult GetBestFit(DependencyZone zone) //F1 Score
        ////{
        ////    FitResult FIT = new FitResult(zone);
        ////    foreach (KeyValuePair<int, List<int>> pair in this.Communities.AllCommunities)
        ////    {
        ////        int TP = 0;
        ////        foreach (int id in pair.Value)
        ////        {
        ////            if (zone.AllVertices.Contains(this.Network.Vertices[id]))
        ////            {
        ////                TP += 1;
        ////            }
        ////        }

        ////        double FN = zone.AllVertices.Count - TP;
        ////        double FP = pair.Value.Count - TP;

        ////        double P = TP / (TP + FP);
        ////        double R = TP / (TP + FN);
        ////        double F1score = P + R;
        ////        if (F1score > 0)
        ////        {
        ////            F1score = 2 * P * R / F1score;
        ////        }

        ////        if (F1score > FIT.Value)
        ////        {
        ////            FIT.Value = F1score;
        ////            FIT.CommunityId = pair.Key;
        ////            FIT.CommunityCount = pair.Value.Count;
        ////            FIT.OverlapCount = TP;
        ////        }

        ////        if (FIT.Value == 1)
        ////        {
        ////            return FIT;
        ////        }
        ////    }

        ////    return FIT;
        ////}

        //private Object communityFitLock = new object();
        //public Dictionary<DependencyZone, ZoneToCommunities.ZoneFitResult> GetCommunityFitResults()
        //{
        //    ZoneToCommunities.ZoneFitResult[] results = new ZoneToCommunities.ZoneFitResult[this.Zones.AllZones.Count];
        //    //foreach (DependencyZone zone in this.Zones.AllZones)
        //    Parallel.ForEach(this.Zones.AllZones, zone =>
        //    {
        //        ZoneToCommunities.ZoneFitResult mcc = this.GetBestZoneFit(zone);
        //        lock (communityFitLock)
        //        {
        //            results[zone.Id - 1] = mcc;
        //        }
        //    });

        //    Dictionary<DependencyZone, ZoneToCommunities.ZoneFitResult> fitResults = new Dictionary<DependencyZone, ZoneToCommunities.ZoneFitResult>();
        //    for (int i = 0; i < results.Length; i++)
        //    {
        //        fitResults.Add(this.Zones.AllZones[i], results[i]);
        //    }

        //    return fitResults;
        //}

    }

}
