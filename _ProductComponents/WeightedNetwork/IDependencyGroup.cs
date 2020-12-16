using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightedNetwork
{
    public interface IDependencyGroup
    {
        int Id { get; }
        //DependencyZone Zone { get; }
        Vertex Ego { get; }
        HashSet<Vertex> Egos { get; }
        HashSet<Vertex> Liaisons { get; }
        HashSet<Vertex> CoLiaisons { get; }
        HashSet<Vertex> AllVertices { get; }
        HashSet<Vertex> InnerZone { get; }
        HashSet<Vertex> OuterZone { get; }

        double GetEmbeddedness();
        Network.GroupDependency Dependency { get; }
        double GetQuality();
        double GetEgoExpansionRatio();
    }
}
