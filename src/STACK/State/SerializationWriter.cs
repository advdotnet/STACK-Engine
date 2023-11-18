using Microsoft.Xna.Framework;
using StarFinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace STACK.State
{
	public class SerializationWriter : BinaryWriter
	{
		public SerializationWriter(Stream s) : base(s) { }

		public static SerializationWriter Create()
		{
			var memoryStream = new MemoryStream(1024);
			return new SerializationWriter(memoryStream);
		}

		public void Write(Vector2 vector2)
		{
			Write(vector2.X);
			Write(vector2.Y);
		}

		public void Write(Matrix matrix)
		{
			Write(matrix.M11);
			Write(matrix.M12);
			Write(matrix.M13);
			Write(matrix.M14);

			Write(matrix.M21);
			Write(matrix.M22);
			Write(matrix.M23);
			Write(matrix.M24);

			Write(matrix.M31);
			Write(matrix.M32);
			Write(matrix.M33);
			Write(matrix.M34);

			Write(matrix.M41);
			Write(matrix.M42);
			Write(matrix.M43);
			Write(matrix.M44);
		}

		public void Write(Color color)
		{
			Write(color.A);
			Write(color.R);
			Write(color.G);
			Write(color.B);
		}

		public void Write<T>(ICollection<T> collection, Action<SerializationWriter, T> itemAction)
		{
			if (collection == null)
			{
				Write(-1);
			}
			else
			{
				Write(collection.Count);
				foreach (var item in collection)
				{
					itemAction(this, item);
				}
			}
		}

		public void Write(Triangle<TriangleVertexData> triangle)
		{
			Write(triangle.ID);
			Write(triangle.A);
			Write(triangle.B);
			Write(triangle.C);
		}

		public void Write(DataVertex<TriangleVertexData> vertex)
		{
			Write(vertex.Data.Color);
			Write(vertex.Data.Scale);
			Write(vertex.Point);
		}

		public void InjectSerializationInfo(SerializationInfo info)
		{
			var bytes = ((MemoryStream)BaseStream).ToArray();
			info.AddValue("X", bytes, typeof(byte[]));
		}
	}
}
