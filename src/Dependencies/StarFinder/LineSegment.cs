using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace StarFinder
{
	[DebuggerDisplay("{Vertex1}, {Vertex2}")]
	[Serializable]
	public class LineSegment
	{
		public Vector2 Vertex1 { get; private set; }
		public Vector2 Vertex2 { get; private set; }

		private readonly float _length;

		public LineSegment(Vector2 vertex1, Vector2 vertex2)
		{
			Vertex1 = vertex1;
			Vertex2 = vertex2;
			_length = (Vertex1 - Vertex2).Length();
		}

		public static bool operator ==(LineSegment t1, LineSegment t2)
		{
			if (t1 is null)
			{
				return t2 is null;
			}

			if (t2 is null)
			{
				return false;
			}

			return ((t1.Vertex1 == t2.Vertex1) && (t1.Vertex2 == t2.Vertex2)) ||
				   ((t1.Vertex1 == t2.Vertex2) && (t1.Vertex2 == t2.Vertex1));
		}

		public static bool operator !=(LineSegment t1, LineSegment t2)
		{
			return !(t1 == t2);
		}

		public bool Equals(LineSegment other)
		{
			return other == this;
		}

		public bool Equals(Vector2 a, Vector2 b)
		{
			return (a == Vertex1 && b == Vertex2) || (a == Vertex2 && b == Vertex1);
		}

		public override bool Equals(object other)
		{
			if (other is LineSegment segment)
			{
				return Equals(segment);
			}

			return base.Equals(other);
		}

		public override int GetHashCode()
		{
			return Vertex1.GetHashCode() ^ Vertex2.GetHashCode();
		}

		public bool HasVertex(Vector2 vertex)
		{
			return Vertex1 == vertex || Vertex2 == vertex;
		}

		public bool Crosses(Vector2 a, Vector2 b) => Crosses(a, b, out _);

		/// <summary>
		/// Returns if the line a-b crosses the line segment.
		/// </summary>
		public bool Crosses(Vector2 a, Vector2 b, out Vector2 intersection)
		{
			intersection = Vector2.Zero;

			var c = Vertex1;
			var d = Vertex2;

			var denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));

			if (denominator == 0)
			{
				return false;
			}

			var numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
			var numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

			if (numerator1 == 0 || numerator2 == 0)
			{
				return false;
			}

			var r = EpsilonUnitInterval(numerator1 / denominator);
			var s = EpsilonUnitInterval(numerator2 / denominator);

			intersection = new Vector2(a.X + ((b.X - a.X) * r), a.Y + ((b.Y - a.Y) * s));

			return r > 0 && r < 1 && s > 0 && s < 1;
		}

		private const float _epsilon = 0.00001f;

		public static float EpsilonUnitInterval(float a)
		{
			if (NearlyEqual(a, 1f))
			{
				return 1f;
			}

			if (NearlyEqual(a, 0f))
			{
				return 0f;
			}

			return a;
		}

		public bool Contains(Vector2 c)
		{
			var partsLength = (Vertex1 - c).Length() + (c - Vertex2).Length();

			return NearlyEqual(_length, partsLength);
		}

		/// <summary>
		/// Returns whether the line segment ab contains c.
		/// </summary>        
		public static bool Contains(Vector2 a, Vector2 b, Vector2 c)
		{
			var partsLength = (a - c).Length() + (c - b).Length();
			var segmentLength = (a - b).Length();

			return NearlyEqual(segmentLength, partsLength);
		}

		public static bool NearlyEqual(float a, float b)
		{
			var diff = Math.Abs(b - a);

			if (b == a)
			{
				return true;
			}
			else if (b == 0 || a == 0 || diff < float.Epsilon)
			{
				return diff < _epsilon;
			}
			else
			{
				return diff / (b + a) < _epsilon;
			}
		}

		/// <summary>
		/// Returns the closest point on this line segment to the given point.
		/// </summary>
		public Vector2 GetClosestPoint(Vector2 near)
		{
			if (Vertex1 == Vertex2)
			{
				return Vertex1;
			}

			var ap = near - Vertex1;
			var ab = Vertex2 - Vertex1;

			var distance = ((ab.X * ap.X) + (ab.Y * ap.Y)) / ab.LengthSquared();

			if (distance < 0)
			{
				return Vertex1;
			}
			else if (distance > 1)
			{
				return Vertex2;
			}

			return Vertex1 + (ab * distance);
		}
	}
}
