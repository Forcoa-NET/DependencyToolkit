using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightedNetwork
{
    public class Triad
    {
        public static double DEFAULT_EDGE_VALUE = 1;

        private enum Dependency
        {
            OwDep, TwDep, OwIndep, TwIndep
        }

        private enum Prominency
        {
            Strong, Weak, None
        }

        public readonly Vertex VertexX;
        public readonly Vertex VertexA;
        public readonly Vertex VertexB;

        private Edge TemporalClosureEdge;

        private string prominencyConfiguration = string.Empty;
        private string dependencyConfiguration = string.Empty;
        private bool closure;

        public readonly static string[] AllDependencyConfigurations =
            { "DD", "Dd", "DI", "Di", "Id", "II", "Ii", "dd", "di", "ii" };
        public readonly static string[] AllProminencyConfigurations =
            { "SSS", "SSW", "SSN", "SWW", "SWN", "SNN", "WSS", "WSW", "WSN", "WWW", "WWN", "WNN", "NSS", "NSW", "NSN", "NWW", "NWN", "NNN" };

        public Triad(Vertex vX, Vertex vA, Vertex vB)
        {
            this.VertexX = vX;
            this.VertexA = vA;
            this.VertexB = vB;

            this.processDependencyConfiguration();
            this.processProminencyConfiguration();

            this.closure = this.VertexA.AdjacentVertices.Contains(this.VertexB) || this.VertexB.AdjacentVertices.Contains(this.VertexA);
        }

        public string DependencyConfiguration
        {
            get
            {
                return this.dependencyConfiguration;
            }
        }

        public string ProminencyConfiguration
        {
            get
            {
                return this.prominencyConfiguration;
            }
        }

        public bool Closure
        {
            get
            {
                return this.closure;
            }
        }

        private void processDependencyConfiguration()
        {
            bool dAX = this.VertexA.IsDependentOn(this.VertexX);
            bool dXA = this.VertexX.IsDependentOn(this.VertexA);
            bool dBX = this.VertexB.IsDependentOn(this.VertexX);
            bool dXB = this.VertexX.IsDependentOn(this.VertexB);

            Dependency depA;
            if (dAX)
            {
                if (dXA)
                {
                    depA = Dependency.TwDep;
                }
                else
                {
                    depA = Dependency.OwDep;
                }
            }
            else
            {
                if (dXA)
                {
                    depA = Dependency.OwIndep;
                }
                else
                {
                    depA = Dependency.TwIndep;
                }
            }

            Dependency depB;
            if (dBX)
            {
                if (dXB)
                {
                    depB = Dependency.TwDep;
                }
                else
                {
                    depB = Dependency.OwDep;
                }
            }
            else
            {
                if (dXB)
                {
                    depB = Dependency.OwIndep;
                }
                else
                {
                    depB = Dependency.TwIndep;
                }
            }

            if (depA == Dependency.TwDep && depB == Dependency.TwDep)
            {
                this.dependencyConfiguration = "DD";
            }
            else if (depA == Dependency.TwDep && depB == Dependency.OwDep)
            {
                this.dependencyConfiguration = "Dd";
            }
            else if (depB == Dependency.TwDep && depA == Dependency.OwDep)
            {
                this.dependencyConfiguration = "Dd";
            }
            else if (depA == Dependency.TwDep && depB == Dependency.TwIndep)
            {
                this.dependencyConfiguration = "DI";
            }
            else if (depB == Dependency.TwDep && depA == Dependency.TwIndep)
            {
                this.dependencyConfiguration = "DI";
            }
            else if (depA == Dependency.TwDep && depB == Dependency.OwIndep)
            {
                this.dependencyConfiguration = "Di";
            }
            else if (depB == Dependency.TwDep && depA == Dependency.OwIndep)
            {
                this.dependencyConfiguration = "Di";
            }
            else if (depA == Dependency.TwIndep && depB == Dependency.OwDep)
            {
                this.dependencyConfiguration = "Id";
            }
            else if (depB == Dependency.TwIndep && depA == Dependency.OwDep)
            {
                this.dependencyConfiguration = "Id";
            }
            else if (depA == Dependency.TwIndep && depB == Dependency.TwIndep)
            {
                this.dependencyConfiguration = "II";
            }
            else if (depA == Dependency.TwIndep && depB == Dependency.OwIndep)
            {
                this.dependencyConfiguration = "Ii";
            }
            else if (depB == Dependency.TwIndep && depA == Dependency.OwIndep)
            {
                this.dependencyConfiguration = "Ii";
            }
            else if (depA == Dependency.OwDep && depB == Dependency.OwDep)
            {
                this.dependencyConfiguration = "dd";
            }
            else if (depA == Dependency.OwDep && depB == Dependency.OwIndep)
            {
                this.dependencyConfiguration = "di";
            }
            else if (depB == Dependency.OwDep && depA == Dependency.OwIndep)
            {
                this.dependencyConfiguration = "di";
            }
            else if (depA == Dependency.OwIndep && depB == Dependency.OwIndep)
            {
                this.dependencyConfiguration = "ii";
            }
        }

        private void processProminencyConfiguration()
        {
            Vertex.ProminencyType pX = this.VertexX.GetProminencyType();
            Vertex.ProminencyType pA = this.VertexA.GetProminencyType();
            Vertex.ProminencyType pB = this.VertexB.GetProminencyType();

            if (pX == Vertex.ProminencyType.StronglyProminent)
            {
                if (pA == Vertex.ProminencyType.StronglyProminent && pB == Vertex.ProminencyType.StronglyProminent)
                {
                    this.prominencyConfiguration = "SSS";
                }
                else if (pA == Vertex.ProminencyType.StronglyProminent && pB == Vertex.ProminencyType.WeaklyProminent)
                {
                    this.prominencyConfiguration = "SSW";
                }
                else if (pA == Vertex.ProminencyType.WeaklyProminent && pB == Vertex.ProminencyType.StronglyProminent)
                {
                    this.prominencyConfiguration = "SSW";
                }
                else if (pA == Vertex.ProminencyType.StronglyProminent && pB == Vertex.ProminencyType.NonProminent)
                {
                    this.prominencyConfiguration = "SSN";
                }
                else if (pA == Vertex.ProminencyType.NonProminent && pB == Vertex.ProminencyType.StronglyProminent)
                {
                    this.prominencyConfiguration = "SSN";
                }
                else if (pA == Vertex.ProminencyType.WeaklyProminent && pB == Vertex.ProminencyType.WeaklyProminent)
                {
                    this.prominencyConfiguration = "SWW";
                }
                else if (pA == Vertex.ProminencyType.WeaklyProminent && pB == Vertex.ProminencyType.NonProminent)
                {
                    this.prominencyConfiguration = "SWN";
                }
                else if (pA == Vertex.ProminencyType.NonProminent && pB == Vertex.ProminencyType.WeaklyProminent)
                {
                    this.prominencyConfiguration = "SWN";
                }
                else if (pA == Vertex.ProminencyType.NonProminent && pB == Vertex.ProminencyType.NonProminent)
                {
                    this.prominencyConfiguration = "SNN";
                }
            }
            else if (pX == Vertex.ProminencyType.WeaklyProminent)
            {
                if (pA == Vertex.ProminencyType.StronglyProminent && pB == Vertex.ProminencyType.StronglyProminent)
                {
                    this.prominencyConfiguration = "WSS";
                }
                else if (pA == Vertex.ProminencyType.StronglyProminent && pB == Vertex.ProminencyType.WeaklyProminent)
                {
                    this.prominencyConfiguration = "WSW";
                }
                else if (pA == Vertex.ProminencyType.WeaklyProminent && pB == Vertex.ProminencyType.StronglyProminent)
                {
                    this.prominencyConfiguration = "WSW";
                }
                else if (pA == Vertex.ProminencyType.StronglyProminent && pB == Vertex.ProminencyType.NonProminent)
                {
                    this.prominencyConfiguration = "WSN";
                }
                else if (pA == Vertex.ProminencyType.NonProminent && pB == Vertex.ProminencyType.StronglyProminent)
                {
                    this.prominencyConfiguration = "WSN";
                }
                else if (pA == Vertex.ProminencyType.WeaklyProminent && pB == Vertex.ProminencyType.WeaklyProminent)
                {
                    this.prominencyConfiguration = "WWW";
                }
                else if (pA == Vertex.ProminencyType.WeaklyProminent && pB == Vertex.ProminencyType.NonProminent)
                {
                    this.prominencyConfiguration = "WWN";
                }
                else if (pA == Vertex.ProminencyType.NonProminent && pB == Vertex.ProminencyType.WeaklyProminent)
                {
                    this.prominencyConfiguration = "WWN";
                }
                else if (pA == Vertex.ProminencyType.NonProminent && pB == Vertex.ProminencyType.NonProminent)
                {
                    this.prominencyConfiguration = "WNN";
                }
            }
            else if (pX == Vertex.ProminencyType.NonProminent)
            {
                if (pA == Vertex.ProminencyType.StronglyProminent && pB == Vertex.ProminencyType.StronglyProminent)
                {
                    this.prominencyConfiguration = "NSS";
                }
                else if (pA == Vertex.ProminencyType.StronglyProminent && pB == Vertex.ProminencyType.WeaklyProminent)
                {
                    this.prominencyConfiguration = "NSW";
                }
                else if (pA == Vertex.ProminencyType.WeaklyProminent && pB == Vertex.ProminencyType.StronglyProminent)
                {
                    this.prominencyConfiguration = "NSW";
                }
                else if (pA == Vertex.ProminencyType.StronglyProminent && pB == Vertex.ProminencyType.NonProminent)
                {
                    this.prominencyConfiguration = "NSN";
                }
                else if (pA == Vertex.ProminencyType.NonProminent && pB == Vertex.ProminencyType.StronglyProminent)
                {
                    this.prominencyConfiguration = "NSN";
                }
                else if (pA == Vertex.ProminencyType.WeaklyProminent && pB == Vertex.ProminencyType.WeaklyProminent)
                {
                    this.prominencyConfiguration = "NWW";
                }
                else if (pA == Vertex.ProminencyType.WeaklyProminent && pB == Vertex.ProminencyType.NonProminent)
                {
                    this.prominencyConfiguration = "NWN";
                }
                else if (pA == Vertex.ProminencyType.NonProminent && pB == Vertex.ProminencyType.WeaklyProminent)
                {
                    this.prominencyConfiguration = "NWN";
                }
                else if (pA == Vertex.ProminencyType.NonProminent && pB == Vertex.ProminencyType.NonProminent)
                {
                    this.prominencyConfiguration = "NNN";
                }
            }
        }

        public string [] ComputeTriadProminencyChange()
        {
            var oldValue = this.prominencyConfiguration;
            // X si core    
            // add missing endge between A and B
            TemporalClosureEdge = this.VertexA.Network.CreateEdge(this.VertexA,this.VertexB);
            // this.VertexA.AddAdjacent(this.VertexB, new Edge(this.VertexA, this.VertexB));
            // this.VertexB.AddAdjacent(this.VertexA, new Edge(this.VertexA, this.VertexB));

            // recompute dependency
            this.VertexA.ResetProminency();
            this.VertexB.ResetProminency();
            this.VertexX.ResetProminency();

            this.processDependencyConfiguration();
            this.processProminencyConfiguration();

            // check if role changed
            var newValue = this.prominencyConfiguration;

            if (oldValue == newValue)
                return null;

            // write role change
            return new[] { oldValue, newValue};
        }

        public string[] ComputeTriadProminencyChangeOnDelete()
        {
            var oldValue = this.prominencyConfiguration;
            // X si core    
            // add missing endge between A and B
            var edge = VertexA.GetEdge(VertexB);
            VertexA.Network.RemoveEdge(edge);
            VertexA.RemoveAdjancent(this.VertexB);
            VertexB.RemoveAdjancent(this.VertexA);

            // this.VertexA.AddAdjacent(this.VertexB, new Edge(this.VertexA, this.VertexB));
            // this.VertexB.AddAdjacent(this.VertexA, new Edge(this.VertexA, this.VertexB));

            // recompute dependency
            this.VertexA.ResetProminency();
            this.VertexB.ResetProminency();
            this.VertexX.ResetProminency();

            this.processDependencyConfiguration();
            this.processProminencyConfiguration();

            // check if role changed
            var newValue = this.prominencyConfiguration;

            if (oldValue == newValue)
                return null;

            // write role change
            return new[] { oldValue, newValue };
        }

        public void CleanAfterDelete()
        {
            this.VertexA.Network.CreateEdge(this.VertexA, this.VertexB);
            this.VertexA.ResetProminency();
            this.VertexB.ResetProminency();
            this.VertexX.ResetProminency();
            this.processDependencyConfiguration();
            this.processProminencyConfiguration();
        }

        public void Clean()
        {
            this.VertexA.RemoveAdjancent(this.VertexB);
            this.VertexB.RemoveAdjancent(this.VertexA);
            this.VertexA.Network.RemoveEdge(this.TemporalClosureEdge);
            this.VertexA.ResetProminency();
            this.VertexB.ResetProminency();
            this.VertexX.ResetProminency();
            this.processDependencyConfiguration();
            this.processProminencyConfiguration();
            TemporalClosureEdge = null;
        }
    }
}
