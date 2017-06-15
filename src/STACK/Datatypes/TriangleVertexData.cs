using Microsoft.Xna.Framework;
using STACK.State;
using StarFinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;

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

            using (var Reader = new FileStream(filename, FileMode.Open))
            {
                using (var ZipStream = new DeflateStream(Reader, CompressionMode.Decompress))
                {
                    using (var SerializationReader = new SerializationReader(ZipStream))
                    {
                        return Path.LoadFrom(SerializationReader);
                    }
                }
            }
        }

        public void SaveToFile(string filename)
        {
            using (var Stream = new FileStream(filename, FileMode.OpenOrCreate))
            {
                using (var ZipStream = new DeflateStream(Stream, CompressionMode.Compress))
                {
                    using (var writer = new SerializationWriter(ZipStream))
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
}
