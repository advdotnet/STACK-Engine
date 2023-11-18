using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StarFinder
{
	/// <summary>
	/// Contains a triangle collection that can be searched on.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class Path<T> where T : IScalable<T>
	{
		public Mesh<T> Mesh;
		public float MaxScale, MinScale;
		[NonSerialized]
		private NodeCollection _nodes;

		[NonSerialized]
		private AStar<Vertex> _pathFinder;

		private AStar<Vertex> PathFinder
			// Lazy load the AStar instance in case of loss due to state restore
			=> _pathFinder ?? (_pathFinder = new AStar<Vertex>(_nodes.GetLinks));

		public Path()
		{
			MaxScale = 1.0f;
			MinScale = 1.0f;
		}

		public Path(DataVertex<T>[] points, int[] indices)
			: this()
		{
			Mesh = new Mesh<T>(points, indices);
			Initialize();
		}

		public Path(Mesh<T> mesh, float maxScale, float minScale)
		{
			Mesh = mesh;
			Initialize();
			MaxScale = maxScale;
			MinScale = minScale;
		}

		public Path(DataVertex<T>[] points, int[] indices, float maxScale, float minScale)
			: this()
		{
			Mesh = new Mesh<T>(points, indices);
			Initialize();
			MaxScale = maxScale;
			MinScale = minScale;
		}

		public Path(List<Triangle<T>> triangles)
			: this()
		{
			Mesh = new Mesh<T>(triangles);
			Initialize();
		}

		public Path(params Triangle<T>[] triangles) : this(triangles.ToList()) { }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			Initialize();
		}

		/// <summary>
		/// A list of edges which obstruct the line of sight.
		/// </summary>
		[NonSerialized]
		private List<LineSegment> _obstructingEdges;

		private bool LineOfSightCheck(Vector2 a, Vector2 b)
		{
			var center = (a + b) / 2;

			if (!Mesh.Contains(center))
			{
				return false;
			}

			for (var i = 0; i < _obstructingEdges.Count; i++)
			{
				if (_obstructingEdges[i].Equals(a, b))
				{
					return true;
				}

				if (_obstructingEdges[i].Crosses(a, b))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns if the line a-b is not obstructed by the mesh.
		/// </summary>
		private bool InLineOfSight(Vector2 a, Vector2 b)
		{
			if (!LineOfSightCheck(a, b))
			{
				return false;
			}

			// Iterate over all vertices of the mesh lying on the line segment given by a and b.
			foreach (var vertex in Mesh.Vertices)
			{
				if (vertex != a && vertex != b && LineSegment.Contains(a, b, vertex))
				{
					if (!LineOfSightCheck(vertex, a) || !LineOfSightCheck(vertex, b))
					{
						return false;
					}
				}
			}

			return true;
		}

		protected void Initialize()
		{
			_obstructingEdges = new List<LineSegment>();
			var sharedEdges = new List<LineSegment>();

			foreach (var outerTriangle in Mesh.Triangles)
			{
				foreach (var outerEdge in outerTriangle.Edges)
				{
					if (!_obstructingEdges.Any(e => e == outerEdge))
					{
						_obstructingEdges.Add(outerEdge);
					}
				}

				foreach (var innerTriangle in Mesh.Triangles)
				{
					if (innerTriangle != outerTriangle)
					{
						var sharedEdge = outerTriangle.GetSharedEdge(innerTriangle);

						if (sharedEdge != null && !sharedEdges.Any(e => e == sharedEdge))
						{
							sharedEdges.Add(sharedEdge);
						}
					}
				}
			}

			_obstructingEdges.RemoveAll(e => sharedEdges.Any(s => s == e));

			_nodes = new NodeCollection();

			foreach (var point in Mesh.Vertices)
			{
				var triangleCount = Mesh.Triangles.Where(t => t.HasVertex(point)).Count();
				var edgeCount = sharedEdges.Where(e => e.HasVertex(point)).Count();

				if (triangleCount != edgeCount)
				{
					_nodes.Add(point);
				}
			}

			_nodes.CalculateStaticLinks(InLineOfSight);
		}

		/// <summary>
		/// Gets the path scale depending on the given y value.
		/// </summary>
		public float GetScale(float y)
		{
			if (Mesh == null)
			{
				return 1.0f;
			}

			if (y >= Mesh.MaxY)
			{
				return MaxScale;
			}

			if (y <= Mesh.MinY)
			{
				return MinScale;
			}

			var m = (MaxScale - MinScale) / (Mesh.MaxY - Mesh.MinY);

			return ((y - Mesh.MinY) * m) + MinScale;
		}

		/// <summary>
		/// Gets the interpolated vertex data at the given position.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public T GetVertexData(Vector2 position)
		{
			if (!Mesh.Contains(position))
			{
				return default;
			}

			var triangle = Mesh.GetClosestTriangle(position);

			if (triangle == null)
			{
				return default;
			}

			return triangle.GetVertexData(position);
		}

		public bool LineObstructed(Vector2 from, Vector2 to, out Vector2 position)
		{
			position = Vector2.Zero;

			if (Mesh == null || from == to)
			{
				return false;
			}

			var points = new List<Vector2>();

			for (var i = 0; i < _obstructingEdges.Count; i++)
			{
				if (_obstructingEdges[i].Equals(from, to))
				{
					return false;
				}

				if (_obstructingEdges[i].Crosses(from, to, out var intersection))
				{
					points.Add(intersection);
				}
			}

			if (points.Count == 0)
			{
				return false;
			}

			var smallestIndex = -1;
			var smallestDistance = float.MaxValue;

			for (var i = 0; i < points.Count; i++)
			{
				var distance = (points[i] - from).LengthSquared();
				if (smallestDistance > distance)
				{
					smallestDistance = distance;
					smallestIndex = i;
				}
			}

			position = points[smallestIndex];

			return true;
		}

		/// <summary>
		/// Returns a path between given points. If the points are
		/// not contained in the mesh, the closest points inside
		/// the mesh are used.
		/// </summary>
		public void FindPath(Vector2 from, Vector2 to, ref List<Vector2> result)
		{
			result.Clear();

			if (Mesh == null)
			{
				result.Add(from);
				result.Add(to);
				return;
			}

			var triangleStart = Mesh.GetClosestTriangle(from);
			var triangleLast = Mesh.GetClosestTriangle(to);

			var first = triangleStart.GetClosestPoint(from);
			var last = triangleLast.GetClosestPoint(to);

			if (first == last)
			{
				result.Add(first);

				return;
			}

			// line of sight check
			if (triangleStart == triangleLast || InLineOfSight(first, last))
			{
				result.Add(first);
				result.Add(last);

				return;
			}

			_nodes.CalculateDynamicLinks(first, last, InLineOfSight);

			PathFinder.Search(first, last, ref _searchResult, Vertex.Heuristic);

			if (_searchResult.Count == 0)
			{
				return;
			}

			for (var i = 0; i < _searchResult.Count; i++)
			{
				result.Add(_searchResult[i].Point);
			}
		}

		[NonSerialized]
		private List<Vertex> _searchResult = new List<Vertex>();

		public bool Contains(Vector2 position)
		{
			if (Mesh == null)
			{
				return false;
			}

			return Mesh.Contains(position);
		}

		public Triangle<T> GetClosestTriangle(Vector2 position)
		{
			if (Mesh == null)
			{
				return null;
			}

			return Mesh.GetClosestTriangle(position);
		}

		public Vector2 GetClosestPoint(Vector2 position)
		{
			if (Mesh == null)
			{
				return Vector2.Zero;
			}

			return Mesh.GetClosestTriangle(position).GetClosestPoint(position);
		}

		/// <summary>
		/// Draws the path for debug purposes.
		/// </summary>        
		public void Draw(Action<Vector2, Vector2, Vector2, Color> drawTriangle, Action<Vector2, Vector2, Color> drawLine, Vector2 mouse)
		{
			if (Mesh == null || Mesh.Triangles == null)
			{
				return;
			}

			for (var i = 0; i < Mesh.Triangles.Length; i++)
			{
				var color = (Mesh.Triangles[i] == GetClosestTriangle(mouse)) ? new Color(255, 100, 100, 50) : new Color(255, 255, 255, 50);

				drawTriangle(new Vector2(Mesh.Triangles[i].A.Point.X, Mesh.Triangles[i].A.Point.Y),
					new Vector2(Mesh.Triangles[i].B.Point.X, Mesh.Triangles[i].B.Point.Y),
					new Vector2(Mesh.Triangles[i].C.Point.X, Mesh.Triangles[i].C.Point.Y), color);

				drawLine(new Vector2(Mesh.Triangles[i].A.Point.X, Mesh.Triangles[i].A.Point.Y),
					new Vector2(Mesh.Triangles[i].B.Point.X, Mesh.Triangles[i].B.Point.Y), new Color(0, 0, 0, 100));

				drawLine(new Vector2(Mesh.Triangles[i].B.Point.X, Mesh.Triangles[i].B.Point.Y),
					new Vector2(Mesh.Triangles[i].C.Point.X, Mesh.Triangles[i].C.Point.Y), new Color(0, 0, 0, 100));

				drawLine(new Vector2(Mesh.Triangles[i].A.Point.X, Mesh.Triangles[i].A.Point.Y),
					new Vector2(Mesh.Triangles[i].C.Point.X, Mesh.Triangles[i].C.Point.Y), new Color(0, 0, 0, 100));

			}
		}
	}
}
