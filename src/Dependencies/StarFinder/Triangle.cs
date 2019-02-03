using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace StarFinder
{
    /// <summary>
    /// Represents a triangle given by three vertices which may contain additional data.
    /// Used for the triangle based pathfinding.
    /// </summary>    
    [Serializable]
    [DebuggerDisplay("{A.Point}, {B.Point}, {C.Point}")]
    public class Triangle<T> where T : IScalable<T>
    {
        public int ID { get; private set; }

        public DataVertex<T> A { get; private set; }
        public DataVertex<T> B { get; private set; }
        public DataVertex<T> C { get; private set; }

        public LineSegment AB { get; private set; }
        public LineSegment BC { get; private set; }
        public LineSegment CA { get; private set; }

        LineSegment[] _Edges;

        public LineSegment[] Edges
        {
            get
            {
                return _Edges;
            }
        }

        public Triangle()
        {

        }

        public Triangle(DataVertex<T> a, DataVertex<T> b, DataVertex<T> c, int id = -1)
        {
            A = a;
            B = b;
            C = c;

            ID = id;

            Cache();
        }

        public Triangle(Vector2 a, Vector2 b, Vector2 c, int id = -1)
        {
            A = new DataVertex<T>(a);
            B = new DataVertex<T>(b);
            C = new DataVertex<T>(c);

            ID = id;

            Cache();
        }

        float s1, s2, s3, s4, Denominator;
        float TriangleArea;
        Vector2 BoundsLT, BoundsRB;

        public bool HasEdge(LineSegment edge)
        {
            return AB == edge || BC == edge || CA == edge;
        }

        /// <summary>
        /// Returns the edge the triangle shares with another or null.
        /// </summary>
        public LineSegment GetSharedEdge(Triangle<T> other)
        {
            if (HasEdge(other.AB)) return other.AB;
            if (HasEdge(other.BC)) return other.BC;
            if (HasEdge(other.CA)) return other.CA;

            return null;
        }

        /// <summary>
        /// Caches some calculations to speed up point inside triangle test.
        /// </summary>        
        void Cache()
        {
            AB = new LineSegment(A, B);
            BC = new LineSegment(B, C);
            CA = new LineSegment(C, A);

            s1 = (B.Point.Y - C.Point.Y);
            s2 = (A.Point.X - C.Point.X);
            s3 = (C.Point.X - B.Point.X);
            s4 = (C.Point.Y - A.Point.Y);

            Denominator = (s1 * s2 + s3 * (A.Point.Y - C.Point.Y));
            TriangleArea = 0.5f * Math.Abs(s2 * (B.Point.Y - A.Point.Y) - (A.Point.X - B.Point.X) * s4);
            _Edges = new LineSegment[] { AB, BC, CA };

            BoundsLT.X = Math.Min(Math.Min(A.Point.X, B.Point.X), C.Point.X);
            BoundsLT.Y = Math.Min(Math.Min(A.Point.Y, B.Point.Y), C.Point.Y);

            BoundsRB.X = Math.Max(Math.Max(A.Point.X, B.Point.X), C.Point.X);
            BoundsRB.Y = Math.Max(Math.Max(A.Point.Y, B.Point.Y), C.Point.Y);
        }

        /// <summary>
        /// Returns the area of the triangle given by the vertices a,b and c.
        /// </summary>
        public static float Area(Vector2 a, Vector2 b, Vector2 c)
        {
            return 0.5f * Math.Abs((a.X - c.X) * (b.Y - a.Y) - (a.X - b.X) * (c.Y - a.Y));
        }

        public T GetVertexData(Vector2 p)
        {
            if (!Encloses(p))
            {
                throw new InvalidOperationException("Point outside of triangle.");
            }

            var a = Area(B.Point, p, C.Point);
            var b = Area(C.Point, p, A.Point);
            var c = Area(A.Point, p, B.Point);

            return A.Data.Multiply(a / TriangleArea)
                .Add(B.Data.Multiply(b / TriangleArea)
                .Add(C.Data.Multiply(c / TriangleArea)));
        }

        public bool HasVertex(Vector2 point)
        {
            return (A.Point == point || B.Point == point || C.Point == point);
        }

        /// <summary>
        /// Checks if a point lies within this triangle.
        /// </summary>
        public bool Encloses(Vector2 point)
        {
            if (point.X < BoundsLT.X || point.Y < BoundsLT.Y || point.X > BoundsRB.X || point.Y > BoundsRB.Y)
            {
                return false;
            }

            // Point lies on a vertex
            if (HasVertex(point))
            {
                return true;
            }

            // Triangle having zero area -> two points equal
            if (Denominator == 0)
            {
                return AB.Contains(point) || BC.Contains(point);
            }

            var a = LineSegment.EpsilonUnitInterval((s1 * (point.X - C.Point.X) + s3 * (point.Y - C.Point.Y)) / Denominator);
            var b = LineSegment.EpsilonUnitInterval((s4 * (point.X - C.Point.X) + s2 * (point.Y - C.Point.Y)) / Denominator);
            var c = LineSegment.EpsilonUnitInterval(1 - a - b);

            return (0 <= a && a <= 1 && 0 <= b && b <= 1 && 0 <= c && c <= 1);
        }

        /// <summary>
        /// Returns either the given point if it is contained in the triangle or returns the 
        /// closest point within.
        /// </summary>
        public Vector2 GetClosestPoint(Vector2 point)
        {
            var ResultDistance = -1f;
            var ResultPoint = Vector2.Zero;

            if (Encloses(point))
            {
                return point;
            }

            for (int i = 0; i < 3; i++)
            {
                var ClosestPoint = Edges[i].GetClosestPoint(point);
                var Distance = (ClosestPoint - point).LengthSquared();

                if (ResultDistance == -1 || Distance < ResultDistance)
                {
                    ResultPoint = ClosestPoint;
                    ResultDistance = Distance;
                }
            }

            return ResultPoint;
        }
    }
}
