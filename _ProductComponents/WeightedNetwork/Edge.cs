using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
    public class Edge
    {
        public readonly Vertex VertexA;
        public readonly Vertex VertexB;

        public readonly double Weight;

        public Edge(Vertex vertexA, Vertex vertexB, double weight)
        {
	        this.VertexA = vertexA;
	        this.VertexB = vertexB;
            this.Weight = weight;
        }

        public Edge(Vertex vertexA, Vertex vertexB)
        {
            this.VertexA = vertexA;
            this.VertexB = vertexB;
            this.Weight = 1;
        }

        public Vertex GetAdjacent(Vertex vertex)
        {
            if (this.VertexA == vertex)
            {
                return this.VertexB;
            }
            else if (this.VertexB == vertex)
            {
                return this.VertexA;
            }
            return null;
        }

        public bool Contains(Vertex vertex)
        {
            return (this.VertexA == vertex) || (this.VertexB == vertex);
        }
    }
}
