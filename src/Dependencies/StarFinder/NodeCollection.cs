using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFinder
{
    /// <summary>
    /// Stores a collection of nodes and their links which can be searched on.
    /// </summary>
    [Serializable]
    public class NodeCollection
    {
        HashSet<Vertex> Nodes = new HashSet<Vertex>();
        List<Vertex> DynamicNodes = new List<Vertex>();
        NodeLinks StaticLinks = new NodeLinks();
        NodeLinks DynamicLinks = new NodeLinks();
        HashSet<Vertex> GetLinksResult = new HashSet<Vertex>();

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
            if (!StaticLinks.Contains(start))
            {
                DynamicNodes.Add(start);
            }

            if (!StaticLinks.Contains(end) && start != end)
            {
                DynamicNodes.Add(end);
            }

            CalculateLinks(predicate, DynamicNodes, DynamicLinks);
        }

        /// <summary>
        /// Returns all nodes linked to the given node.
        /// </summary>
        public IEnumerable<Vertex> GetLinks(Vertex point)
        {
            GetLinksResult.Clear();

            var Dynamic = DynamicLinks.GetLinks(point);
            var Static = StaticLinks.GetLinks(point);

            if (null != Dynamic)
            {
                for (int i = 0; i < Dynamic.Count; i++)
                {
                    GetLinksResult.Add(Dynamic[i]);
                }
            }

            if (null != Static)
            {
                for (int i = 0; i < Static.Count; i++)
                {
                    GetLinksResult.Add(Static[i]);
                }
            }

            return GetLinksResult;
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
