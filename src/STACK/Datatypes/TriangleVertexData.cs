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
			if (!File.Exists(filename) && !filename.EndsWith(".stp"))
			{
				filename += ".stp";
			}

			using (var reader = new FileStream(filename, FileMode.Open))
			{
				using (var zipStream = new DeflateStream(reader, CompressionMode.Decompress))
				{
					using (var serializationReader = new SerializationReader(zipStream))
					{
						return LoadFrom(serializationReader);
					}
				}
			}
		}

		public void SaveToFile(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.OpenOrCreate))
			{
				using (var zipStream = new DeflateStream(stream, CompressionMode.Compress))
				{
					using (var writer = new SerializationWriter(zipStream))
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
			var minScale = reader.ReadSingle();
			var maxScale = reader.ReadSingle();
			var triangles = reader.ReadList((a) => a.ReadTriangle()).ToList();

			return new Path(new Mesh<TriangleVertexData>(triangles), maxScale, minScale);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			var writer = SerializationWriter.Create();

			WriteDataTo(writer);

			writer.InjectSerializationInfo(info);
		}

		public Path(SerializationInfo info, StreamingContext ctxt)
		{
			var reader = SerializationReader.GetReader(info);

			MinScale = reader.ReadSingle();
			MaxScale = reader.ReadSingle();
			var triangles = reader.ReadList((a) => a.ReadTriangle()).ToList();

			Mesh = new Mesh<TriangleVertexData>(triangles);
		}
	}
}
