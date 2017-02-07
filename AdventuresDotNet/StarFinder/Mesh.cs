using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
        Triangle<T>[] _Triangles; 

        [NonSerialized]
        Vector2[] _Vertices;

        public Mesh()
        {
        }

        public Mesh(DataVertex<T>[] points, int[] indices)
        {            
            SetTriangles(points, indices);
        }        

        public Mesh(List<Triangle<T>> triangles)
        {
            DataVertex<T>[] Points = new DataVertex<T>[triangles.Count * 3];
            int[] Indices = new int[triangles.Count * 3];

            for (int i = 0; i < triangles.Count; i++)
            {
                Points[i * 3 + 0] = triangles[i].A;
                Points[i * 3 + 1] = triangles[i].B;
                Points[i * 3 + 2] = triangles[i].C;

                Indices[i * 3] = i * 3;
                Indices[i * 3 + 1] = i * 3 + 1;
                Indices[i * 3 + 2] = i * 3 + 2;
            }  
          
            SetTriangles(Points, Indices);
        }

        void ExtendBounds(Vector2 point)
        {
            MaxX = Math.Max(MaxX, point.X);
            MinX = Math.Min(MinX, point.X);

            MaxY = Math.Max(MaxY, point.Y);
            MinY = Math.Min(MinY, point.Y);            
        }        

        /// <summary>
        /// Creates the underlying triangle structure using a set of vertices and indices.
        /// </summary>
        void SetTriangles(DataVertex<T>[] points, int[] indices)
        {
            if (indices.Length % 3 != 0)
            {
                throw new ArgumentException("Triangle indices is not multiple of three.");
            }

            _Triangles = new Triangle<T>[indices.Length / 3];            

            var a = 0;

            MaxX = points[0].Point.X;
            MinX = MaxX;            

            MaxY = points[0].Point.Y;
            MinY = MaxY;            

            for (int i = 0; i <= indices.Length - 3; i += 3)
            {
                ExtendBounds(points[indices[i]].Point);
                ExtendBounds(points[indices[i + 1]].Point);
                ExtendBounds(points[indices[i + 2]].Point);

                _Triangles[a] = new Triangle<T>(points[indices[i]], points[indices[i + 1]], points[indices[i + 2]], a++);                           
            }            
        }

        private void CacheVertices()
        {
            _Vertices = new Vector2[_Triangles.Length * 3];

            for (var i = 0; i < _Triangles.Length; i++)
            {
                _Vertices[i * 3] = _Triangles[i].A.Point;
                _Vertices[i * 3 + 1] = _Triangles[i].B.Point;
                _Vertices[i * 3 + 2] = _Triangles[i].C.Point;
            }
        }

        public Triangle<T>[] Triangles
        {
            get
            {
                return _Triangles;
            }
        }               

        public Vector2[] Vertices
        {
            get 
            {
                if (_Vertices == null)
                {
                    CacheVertices();
                }

                return _Vertices;
            }
        }

        /// <summary>
        /// Returns the triangle which contains the given position.
        /// </summary>
        public Triangle<T> GetTriangleAt(Vector2 position)
        {
            for (int i = 0; i < _Triangles.Length; i++)
            {
                if (_Triangles[i].Encloses(position))
                {
                    return _Triangles[i];
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
            float ResultDistance = -1;
            Triangle<T> Result = null;

            for (int i = 0; i < _Triangles.Length; i++)
            {
                if (_Triangles[i].Encloses(position))
                {
                    return _Triangles[i];
                }

                Vector2 ClosestPoint = _Triangles[i].GetClosestPoint(position);

                float Distance = (ClosestPoint - position).LengthSquared();

                if (ResultDistance == -1 || Distance < ResultDistance)
                {                        
                    Result = _Triangles[i];
                    ResultDistance = Distance;
                }
            }

            return Result;
        }               

    }
}
