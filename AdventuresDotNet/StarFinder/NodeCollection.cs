using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using StarFinder;

namespace StarFinder
{
    /// <summary>
    /// Stores a collection of nodes and their links which can be searched on.
    /// </summary>
    [Serializable]
    public class NodeCollection
    {
        HashSet<Vertex> Nodes = new HashSet<Vertex>();
        NodeLinks StaticLinks = new NodeLinks();
        NodeLinks DynamicLinks = new NodeLinks();

        public void Add(Vertex node)
        {            
            Nodes.Add(node);            
        }

        /// <summary>
        /// Calculates and stores the links of static nodes.
        /// </summary>        
        public void CalculateStaticLinks(Func<Vector2, Vector2, bool> predicate)
        {
            CalculateLinks(predicate, Nodes, StaticLinks);            
        }

        /// <summary>
        /// Calculates and stores the links of the dynamic nodes given by start and end.
        /// </summary>
        public void CalculateDynamicLinks(Vertex start, Vertex end, Func<Vector2, Vector2, bool> predicate)
        {
            List<Vertex> NewNodes = new List<Vertex>();

            if (!StaticLinks.Contains(start))
            {
                NewNodes.Add(start);
            }

            if (!StaticLinks.Contains(end) && start != end)
            {
                NewNodes.Add(end);
            }

            CalculateLinks(predicate, NewNodes, DynamicLinks);            
        }

        /// <summary>
        /// Returns all nodes linked to the given node.
        /// </summary>
        public IEnumerable<Vertex> GetLinks(Vertex point)
        {            
            var Dynamic = DynamicLinks.GetLinks(point);
            var Result = new HashSet<Vertex>(StaticLinks.GetLinks(point));

            for (int i = 0; i < Dynamic.Count; i++) 
            {
                Result.Add(Dynamic[i]);
            }

            return Result;
        }

        void CalculateLinks(Func<Vector2, Vector2, bool> predicate, IEnumerable<Vertex> nodes, NodeLinks links)
        {
            links.Clear();

            foreach (var Outer in Nodes)
            {
                foreach (var Inner in nodes.Where(n => n != Outer))
                {
                    if (predicate(Outer, Inner))
                    {
                        links.AddLink(Outer, Inner);
                    }
                }
            }
        }
    }
   
}
