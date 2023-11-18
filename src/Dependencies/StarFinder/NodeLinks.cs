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
		private readonly Dictionary<Vertex, List<Vertex>> _links = new Dictionary<Vertex, List<Vertex>>();

		public void AddLink(Vertex from, Vertex to)
		{
			if (!_links.ContainsKey(from))
			{
				_links.Add(from, new List<Vertex>());
			}

			if (!_links.ContainsKey(to))
			{
				_links.Add(to, new List<Vertex>());
			}

			if (!_links[to].Contains(from))
			{
				_links[to].Add(from);
			}

			if (!_links[from].Contains(to))
			{
				_links[from].Add(to);
			}
		}

		public bool Contains(Vertex vertex) => _links.Keys.Any(l => l == vertex);

		public List<Vertex> GetLinks(Vertex from)
		{
			var result = _links.FirstOrDefault(l => l.Key == from);

			if (result.Equals(default(KeyValuePair<Vertex, List<Vertex>>)))
			{
				return null;
			}

			return result.Value;
		}

		public void Clear()
		{
			_links.Clear();
		}
	}
}
