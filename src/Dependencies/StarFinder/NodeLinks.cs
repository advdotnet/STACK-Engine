using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFinder
{
    /// <summary>
    /// Keeps track of links between nodes.
    /// </summary>
    [Serializable]
    public class NodeLinks
    {
        Dictionary<Vertex, List<Vertex>> Links = new Dictionary<Vertex, List<Vertex>>();

        public void AddLink(Vertex from, Vertex to)
        {
            if (!Links.ContainsKey(from))
            {
                Links.Add(from, new List<Vertex>());
            }

            if (!Links.ContainsKey(to))
            {
                Links.Add(to, new List<Vertex>());
            }

            if (!Links[to].Contains(from))
            {
                Links[to].Add(from);
            }

            if (!Links[from].Contains(to))
            {
                Links[from].Add(to);
            }
        }

        public bool Contains(Vertex vertex)
        {
            return Links.Keys.Any(l => l == vertex);
        }

        public List<Vertex> GetLinks(Vertex from)
        {
            var Result = Links.FirstOrDefault(l => l.Key == from);

            if (Result.Equals(default(KeyValuePair<Vertex, List<Vertex>>)))
            {
                return null;
            }

            return Result.Value;
        }

        public void Clear()
        {
            Links.Clear();
        }
    }
}
