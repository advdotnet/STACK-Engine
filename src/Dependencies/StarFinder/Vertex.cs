using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace StarFinder
{
	[Serializable]
	[DebuggerDisplay("{Point}")]
	public class Vertex : IMapPosition
	{
		public Vector2 Point;

		public float Cost(IMapPosition position)
		{
			if (position == null)
			{
				return 0;
			}

			return Heuristic(this, (Vertex)position);
		}

		private Vertex()
		{
		}

		public Vertex(Vector2 point) : this()
		{
			Point = point;
		}

		public Vertex(float x, float y) : this()
		{
			Point = new Vector2(x, y);
		}

		public bool Equals(IMapPosition b)
		{
			if (!(b is Vertex))
			{
				return false;
			}

			return Equals((Vertex)b);
		}

		public bool Equals(Vertex other)
		{
			return (Point == other.Point) && (Point == other.Point) && (Point == other.Point);
		}

		public override bool Equals(object other)
		{
			return other is Vertex vertex ? Equals(vertex) : base.Equals(other);
		}

		public static float Heuristic(Vertex current, Vertex goal)
		{
			return (current.Point - goal.Point).Length();
		}

		public static bool operator ==(Vertex t1, Vertex t2)
		{
			if (t1 is null)
			{
				return t2 is null;
			}

			if (t2 is null)
			{
				return false;
			}

			return t1.Point == t2.Point;
		}

		public static bool operator !=(Vertex t1, Vertex t2)
		{
			return !(t1 == t2);
		}

		public static implicit operator Vector2(Vertex vertex)
		{
			return vertex.Point;
		}

		public static implicit operator Vertex(Vector2 vector)
		{
			return new Vertex(vector);
		}

		public override int GetHashCode()
		{
			return Point.GetHashCode();
		}
	}

	/// <summary>
	/// Provides a position and additional data in the generic class.
	/// </summary>    
	[Serializable]
	[DebuggerDisplay("{Point}")]
	public class DataVertex<T> : Vertex where T : IScalable<T>
	{
		public T Data;

		public DataVertex(Vector2 point) : base(point)
		{
			Data = default(T).Default();
		}

		public DataVertex(Vector2 point, T data) : base(point)
		{
			Data = data;
		}

		public DataVertex(float x, float y) : base(x, y)
		{
			Data = default(T).Default();
		}

		public DataVertex(float x, float y, T data)
			: base(x, y)
		{
			Data = data;
		}

		public static implicit operator Vector2(DataVertex<T> vertex)
		{
			return vertex.Point;
		}
	}

}
