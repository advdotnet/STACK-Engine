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
		private readonly HashSet<Vertex> _nodes = new HashSet<Vertex>();
		private readonly List<Vertex> _dynamicNodes = new List<Vertex>();
		private readonly NodeLinks _staticLinks = new NodeLinks();
		private readonly NodeLinks _dynamicLinks = new NodeLinks();
		private readonly HashSet<Vertex> _getLinksResult = new HashSet<Vertex>();

		public void Add(Vertex node)
		{
			_nodes.Add(node);
		}

		/// <summary>
		/// Calculates and stores the links of static nodes.
		/// </summary>        
		public void CalculateStaticLinks(Func<Vector2, Vector2, bool> predicate)
		{
			CalculateLinks(predicate, _nodes, _staticLinks);
		}

		/// <summary>
		/// Calculates and stores the links of the dynamic nodes given by start and end.
		/// </summary>
		public void CalculateDynamicLinks(Vertex start, Vertex end, Func<Vector2, Vector2, bool> predicate)
		{
			_dynamicNodes.Clear();

			if (!_staticLinks.Contains(start))
			{
				_dynamicNodes.Add(start);
			}

			if (!_staticLinks.Contains(end) && start != end)
			{
				_dynamicNodes.Add(end);
			}

			CalculateLinks(predicate, _dynamicNodes, _dynamicLinks);
		}

		/// <summary>
		/// Returns all nodes linked to the given node.
		/// </summary>
		public IEnumerable<Vertex> GetLinks(Vertex point)
		{
			_getLinksResult.Clear();

			var dynamic = _dynamicLinks.GetLinks(point);
			var @static = _staticLinks.GetLinks(point);

			if (null != dynamic)
			{
				for (var i = 0; i < dynamic.Count; i++)
				{
					_getLinksResult.Add(dynamic[i]);
				}
			}

			if (null != @static)
			{
				for (var i = 0; i < @static.Count; i++)
				{
					_getLinksResult.Add(@static[i]);
				}
			}

			return _getLinksResult;
		}

		private void CalculateLinks(Func<Vector2, Vector2, bool> predicate, IEnumerable<Vertex> nodes, NodeLinks links)
		{
			links.Clear();

			foreach (var outer in _nodes)
			{
				foreach (var inner in nodes.Where(n => n != outer))
				{
					if (predicate(outer, inner))
					{
						links.AddLink(outer, inner);
					}
				}
			}
		}
	}

}
