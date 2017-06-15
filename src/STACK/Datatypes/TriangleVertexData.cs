using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StarFinder;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using STACK.State;

namespace STACK
{
    [Serializable]
    public class PathVertex : DataVertex<TriangleVertexData>
    {
        public PathVertex(Vector2 point) : base(point) { }

        public PathVertex(Vector2 point, TriangleVertexData data) : base(point, data) { }

        public PathVertex(float x, float y) : base(x, y) { }

        public PathVertex(float x, float y, TriangleVertexData data) : base(x, y, data) { }
    }

    [Serializable]
    public class Path : Path<TriangleVertexData>, ISerializable
    {        
        public Path(DataVertex<TriangleVertexData>[] points, int[] indices) : base(points, indices) { }

        public Path(Mesh<TriangleVertexData> mesh, float maxScale, float minScale) : base(mesh, maxScale, minScale) { }

        public Path(DataVertex<TriangleVertexData>[] points, int[] indices, float maxScale, float minScale) : base(points, indices, maxScale, minScale) { }

        public Path(List<Triangle<TriangleVertexData>> triangles) : base(triangles) { }

        public Path(params Triangle<TriangleVertexData>[] triangles) : base(triangles) { }

        public Path() : base() { }

        public static Path LoadFromFile(string filename)
        {
            if (!System.IO.File.Exists(filename) && !filename.EndsWith(".stp"))                
            {
                filename = filename + ".stp";
            }

            using (FileStream Reader = new FileStream(filename, FileMode.Open))
            {
                using (DeflateStream ZipStream = new DeflateStream(Reader, CompressionMode.Decompress))
                {
                    using (SerializationReader SerializationReader = new SerializationReader(ZipStream))
                    {
                        return Path.LoadFrom(SerializationReader);
                    }                        
                }
            }
        }

        public void SaveToFile(string filename)
        {            
            using (FileStream Stream = new FileStream(filename, FileMode.OpenOrCreate))
            {
                using (DeflateStream ZipStream = new DeflateStream(Stream, CompressionMode.Compress))
                {
                    using (SerializationWriter writer = new SerializationWriter(ZipStream))
                    {
                        WriteDataTo(writer);
                    }
                }
            }
        }

        public void WriteDataTo(SerializationWriter writer)
        {
            writer.Write(MinScale);
            writer.Write(MaxScale);            
            writer.Write(Mesh.Triangles.ToList(), (a, b) => a.Write(b));            
        }

        public static Path LoadFrom(SerializationReader reader)        
        {            
            var MinScale = reader.ReadSingle();
            var MaxScale = reader.ReadSingle();
            var Triangles = reader.ReadList<Triangle<TriangleVertexData>>((a) => a.ReadTriangle()).ToList();                       
            
            return new Path(new Mesh<TriangleVertexData>(Triangles), MaxScale, MinScale);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var Writer = SerializationWriter.Create();
            
            WriteDataTo(Writer);

            Writer.InjectSerializationInfo(info);            
        }

        public Path(SerializationInfo info, StreamingContext ctxt)
        {
            var Reader = SerializationReader.GetReader(info);            

            MinScale = Reader.ReadSingle();
            MaxScale = Reader.ReadSingle();
            var Triangles = Reader.ReadList<Triangle<TriangleVertexData>>((a) => a.ReadTriangle()).ToList();
            
            Mesh = new Mesh<TriangleVertexData>(Triangles);            
        }
    }

    /// <summary>
    /// Stores common data for each triangle vertex like color or scale.
    /// </summary>
    [Serializable]
    public struct TriangleVertexData : IScalable<TriangleVertexData>
    {
        public Microsoft.Xna.Framework.Color Color;
        public Vector2 Scale;

        public TriangleVertexData(Color color, Vector2 scale)
        {
            Color = color;
            Scale = scale;
        }

        public TriangleVertexData Default()
        {
            return new TriangleVertexData()
            {
                Scale = Vector2.One,
                Color = Color.White
            };
        }

        public TriangleVertexData Multiply(float scale)
        {
            return new TriangleVertexData()
            {
                Scale = this.Scale * scale,
                Color = this.Color * scale
            };
        }

        public TriangleVertexData Add(TriangleVertexData t)
        {
            return new TriangleVertexData()
            {
                Scale = this.Scale + t.Scale,
                Color = new Color(this.Color.R + t.Color.R, this.Color.G + t.Color.G, this.Color.B + t.Color.B, this.Color.A + t.Color.A)
            };
        }

    }
}
