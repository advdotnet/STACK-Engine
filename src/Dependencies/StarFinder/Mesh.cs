using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace StarFinder
{
	/// <summary>
	/// A mesh represents a collection of triangles.
	/// </summary>    
	[Serializable]
	public class Mesh<T> where T : IScalable<T>
	{
		public float MinY { get; private set; }
		public float MaxY { get; private set; }
		public float MinX { get; private set; }
		public float MaxX { get; private set; }

		private Triangle<T>[] _triangles;

		[NonSerialized]
		private Vector2[] _vertices;

		public Mesh()
		{
		}

		public Mesh(DataVertex<T>[] points, int[] indices)
		{
			SetTriangles(points, indices);
		}

		public Mesh(List<Triangle<T>> triangles)
		{
			var points = new DataVertex<T>[triangles.Count * 3];
			var indices = new int[triangles.Count * 3];

			for (var i = 0; i < triangles.Count; i++)
			{
				points[(i * 3) + 0] = triangles[i].A;
				points[(i * 3) + 1] = triangles[i].B;
				points[(i * 3) + 2] = triangles[i].C;

				indices[i * 3] = i * 3;
				indices[(i * 3) + 1] = (i * 3) + 1;
				indices[(i * 3) + 2] = (i * 3) + 2;
			}

			SetTriangles(points, indices);
		}

		private void ExtendBounds(Vector2 point)
		{
			MaxX = Math.Max(MaxX, point.X);
			MinX = Math.Min(MinX, point.X);

			MaxY = Math.Max(MaxY, point.Y);
			MinY = Math.Min(MinY, point.Y);
		}

		/// <summary>
		/// Creates the underlying triangle structure using a set of vertices and indices.
		/// </summary>
		private void SetTriangles(DataVertex<T>[] points, int[] indices)
		{
			if (indices.Length % 3 != 0)
			{
				throw new ArgumentException("Triangle indices is not multiple of three.");
			}

			_triangles = new Triangle<T>[indices.Length / 3];

			var a = 0;

			MaxX = points[0].Point.X;
			MinX = MaxX;

			MaxY = points[0].Point.Y;
			MinY = MaxY;

			for (var i = 0; i <= indices.Length - 3; i += 3)
			{
				ExtendBounds(points[indices[i]].Point);
				ExtendBounds(points[indices[i + 1]].Point);
				ExtendBounds(points[indices[i + 2]].Point);

				_triangles[a] = new Triangle<T>(points[indices[i]], points[indices[i + 1]], points[indices[i + 2]], a++);
			}
		}

		private void CacheVertices()
		{
			_vertices = new Vector2[_triangles.Length * 3];

			for (var i = 0; i < _triangles.Length; i++)
			{
				_vertices[i * 3] = _triangles[i].A.Point;
				_vertices[(i * 3) + 1] = _triangles[i].B.Point;
				_vertices[(i * 3) + 2] = _triangles[i].C.Point;
			}
		}

		public Triangle<T>[] Triangles => _triangles;

		public Vector2[] Vertices
		{
			get
			{
				if (_vertices == null)
				{
					CacheVertices();
				}

				return _vertices;
			}
		}

		/// <summary>
		/// Returns the triangle which contains the given position.
		/// </summary>
		public Triangle<T> GetTriangleAt(Vector2 position)
		{
			for (var i = 0; i < _triangles.Length; i++)
			{
				if (_triangles[i].Encloses(position))
				{
					return _triangles[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Returns whether the mesh contains the given point.
		/// </summary>
		public bool Contains(Vector2 point)
		{
			if (point.X < MinX || point.Y < MinY || point.X > MaxX || point.Y > MaxY)
			{
				return false;
			}

			return GetTriangleAt(point) != null;
		}

		/// <summary>
		/// Returns the closest triangle to the given position.
		/// </summary>
		public Triangle<T> GetClosestTriangle(Vector2 position)
		{
			var resultDistance = -1f;
			Triangle<T> result = null;

			for (var i = 0; i < _triangles.Length; i++)
			{
				if (_triangles[i].Encloses(position))
				{
					return _triangles[i];
				}

				var closestPoint = _triangles[i].GetClosestPoint(position);

				var distance = (closestPoint - position).LengthSquared();

				if (resultDistance == -1 || distance < resultDistance)
				{
					result = _triangles[i];
					resultDistance = distance;
				}
			}

			return result;
		}

	}
}
