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
        NodeCollection Nodes;

        [NonSerialized]
        AStar<Vertex> _PathFinder;

        AStar<Vertex> PathFinder
        {
            // Lazy load the AStar instance in case of loss due to state restore
            get
            {
                return _PathFinder ?? (_PathFinder = new AStar<Vertex>(Nodes.GetLinks));
            }
        }

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
        private List<LineSegment> ObstructingEdges;

        private bool LineOfSightCheck(Vector2 a, Vector2 b)
        {
            var Center = (a + b) / 2;

            if (!Mesh.Contains(Center))
            {
                return false;
            }

            for (int i = 0; i < ObstructingEdges.Count; i++)
            {
                if (ObstructingEdges[i].Equals(a, b))
                {
                    return true;
                }

                if (ObstructingEdges[i].Crosses(a, b))
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
            foreach (var Vertex in Mesh.Vertices)
            {
                if (Vertex != a && Vertex != b && LineSegment.Contains(a, b, Vertex))
                {
                    if (!LineOfSightCheck(Vertex, a) || !LineOfSightCheck(Vertex, b))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected void Initialize()
        {
            ObstructingEdges = new List<LineSegment>();
            var SharedEdges = new List<LineSegment>();

            foreach (var OuterTriangle in Mesh.Triangles)
            {
                foreach (var OuterEdge in OuterTriangle.Edges)
                {
                    if (!ObstructingEdges.Any(e => e == OuterEdge))
                    {
                        ObstructingEdges.Add(OuterEdge);
                    }
                }

                foreach (var InnerTriangle in Mesh.Triangles)
                {
                    if (InnerTriangle != OuterTriangle)
                    {
                        var SharedEdge = OuterTriangle.GetSharedEdge(InnerTriangle);

                        if (SharedEdge != null && !SharedEdges.Any(e => e == SharedEdge))
                        {
                            SharedEdges.Add(SharedEdge);
                        }
                    }
                }
            }

            ObstructingEdges.RemoveAll(e => SharedEdges.Any(s => s == e));

            Nodes = new NodeCollection();

            foreach (var Point in Mesh.Vertices)
            {
                int TriangleCount = Mesh.Triangles.Where(t => t.HasVertex(Point)).Count();
                int EdgeCount = SharedEdges.Where(e => e.HasVertex(Point)).Count();

                if (TriangleCount != EdgeCount)
                {
                    Nodes.Add(Point);
                }
            }

            Nodes.CalculateStaticLinks(InLineOfSight);
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

            float m = (MaxScale - MinScale) / (Mesh.MaxY - Mesh.MinY);
            return (y - Mesh.MinY) * m + MinScale;
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
                return default(T);
            }

            var Triangle = Mesh.GetClosestTriangle(position);

            if (Triangle == null)
            {
                return default(T);
            }

            return Triangle.GetVertexData(position);
        }

        public bool LineObstructed(Vector2 from, Vector2 to, out Vector2 position)
        {
            position = Vector2.Zero;

            if (Mesh == null || from == to)
            {
                return false;
            }

            var Points = new List<Vector2>();
            Vector2 Intersection;

            for (int i = 0; i < ObstructingEdges.Count; i++)
            {
                if (ObstructingEdges[i].Equals(from, to))
                {
                    return false;
                }

                if (ObstructingEdges[i].Crosses(from, to, out Intersection))
                {
                    Points.Add(Intersection);
                }
            }

            if (Points.Count == 0)
            {
                return false;
            }

            int SmallestIndex = -1;
            float SmallestDistance = float.MaxValue;

            for (int i = 0; i < Points.Count; i++)
            {
                float Distance = (Points[i] - from).LengthSquared();
                if (SmallestDistance > Distance)
                {
                    SmallestDistance = Distance;
                    SmallestIndex = i;
                }
            }

            position = Points[SmallestIndex];

            return true;
        }

        /// <summary>
        /// Returns a path between given points. If the points are
        /// not contained in the mesh, the closest points inside
        /// the collections are used.
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

            var TriangleStart = Mesh.GetClosestTriangle(from);
            var TriangleLast = Mesh.GetClosestTriangle(to);

            var First = TriangleStart.GetClosestPoint(from);
            var Last = TriangleLast.GetClosestPoint(to);

            if (First == Last)
            {
                result.Add(First);
                return;
            }

            // line of sight check
            if (TriangleStart == TriangleLast || InLineOfSight(First, Last))
            {
                result.Add(First);
                result.Add(Last);
                return;
            }

            Nodes.CalculateDynamicLinks(First, Last, InLineOfSight);

            PathFinder.Search(First, Last, ref SearchResult, Vertex.Heuristic);

            if (SearchResult.Count == 0)
            {
                return;
            }

            for (int i = 0; i < SearchResult.Count; i++)
            {
                result.Add(SearchResult[i].Point);
            }
        }

        [NonSerialized]
        private List<Vertex> SearchResult = new List<Vertex>();

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

            for (int i = 0; i < Mesh.Triangles.Length; i++)
            {
                var Color = (Mesh.Triangles[i] == GetClosestTriangle(mouse)) ? new Color(255, 100, 100, 50) : new Color(255, 255, 255, 50);

                drawTriangle(new Vector2(Mesh.Triangles[i].A.Point.X, Mesh.Triangles[i].A.Point.Y),
                    new Vector2(Mesh.Triangles[i].B.Point.X, Mesh.Triangles[i].B.Point.Y),
                    new Vector2(Mesh.Triangles[i].C.Point.X, Mesh.Triangles[i].C.Point.Y), Color);

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
