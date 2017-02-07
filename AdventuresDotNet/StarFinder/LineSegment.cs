using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace StarFinder
{
    [DebuggerDisplay("{Vertex1}, {Vertex2}")]
    [Serializable]
    public class LineSegment
    {
        public Vector2 Vertex1 { get; private set; }
        public Vector2 Vertex2 { get; private set; }
        float Length;

        public LineSegment(Vector2 vertex1, Vector2 vertex2) 
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Length = (Vertex1 - Vertex2).Length();
        }

        public static bool operator ==(LineSegment t1, LineSegment t2)
        {
            if (object.ReferenceEquals(t1, null))
            {
                return object.ReferenceEquals(t2, null);
            }

            if (object.ReferenceEquals(t2, null)) return false;

            return (((t1.Vertex1 == t2.Vertex1) && (t1.Vertex2 == t2.Vertex2)) || 
                ((t1.Vertex1 == t2.Vertex2) && (t1.Vertex2 == t2.Vertex1)));
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
            return ((a == Vertex1 && b == Vertex2) || (a == Vertex2 && b == Vertex1));
        }

        public override bool Equals(object other)
        {
            if (other is LineSegment)
            {
                return Equals((LineSegment)other);
            }

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return Vertex1.GetHashCode() ^ Vertex2.GetHashCode();
        }

        public bool HasVertex(Vector2 vertex)
        {
            return (Vertex1 == vertex || Vertex2 == vertex);
        }

        public bool Crosses(Vector2 a, Vector2 b)
        {
            Vector2 Intersection;
            return Crosses(a, b, out Intersection);
        }

        /// <summary>
        /// Returns if the line a-b crosses the line segment.
        /// </summary>
        public bool Crosses(Vector2 a, Vector2 b, out Vector2 intersection)
        {
            intersection = Vector2.Zero;
                 
            Vector2 c = Vertex1;
            Vector2 d = Vertex2;

            float denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));

            if (denominator == 0)
            {
                return false;
            }

            float numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            float numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            if (numerator1 == 0 || numerator2 == 0)
            {
                return false;
            }

            float r = EpsilonUnitInterval(numerator1 / denominator);
            float s = EpsilonUnitInterval(numerator2 / denominator);

            intersection = new Vector2(a.X + (b.X - a.X) * r, a.Y + (b.Y - a.Y) * s);

            return (r > 0 && r < 1) && (s > 0 && s < 1);
        }
        
        const float EPSILON = 0.00001f;            

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
            var PartsLength = (Vertex1 - c).Length() + (c - Vertex2).Length();
            
            return NearlyEqual(Length, PartsLength);
        }

        /// <summary>
        /// Returns whether the line segment ab contains c.
        /// </summary>        
        public static bool Contains(Vector2 a, Vector2 b, Vector2 c)
        {
            var PartsLength = (a - c).Length() + (c - b).Length();
            var SegmentLength = (a - b).Length();
            
            return NearlyEqual(SegmentLength, PartsLength);            
        }

        public static bool NearlyEqual(float a, float b)
        {
            var Diff = Math.Abs(b - a);

            if (b == a)
            {
                return true;
            }
            else if (b == 0 || a == 0 || Diff < float.Epsilon)
            {
                return Diff < EPSILON;
            }
            else
            {
                return Diff / (b + a) < EPSILON;
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

            Vector2 ap = near - Vertex1;
            Vector2 ab = Vertex2 - Vertex1;

            float Distance = (ab.X * ap.X + ab.Y * ap.Y) / ab.LengthSquared();            

            if (Distance < 0)
            {
                return Vertex1;
            }
            else if (Distance > 1)
            {
                return Vertex2;
            }

            return Vertex1 + ab * Distance;
        }
    }
}
