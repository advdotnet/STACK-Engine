using Microsoft.Xna.Framework;
using StarFinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace STACK.State
{
	public class SerializationReader : BinaryReader
	{
		public SerializationReader(Stream s) : base(s) { }

		public static SerializationReader GetReader(SerializationInfo info)
		{
			var bytes = (byte[])info.GetValue("X", typeof(byte[]));
			var ms = new MemoryStream(bytes);
			return new SerializationReader(ms);
		}

		public Color ReadColor()
		{
			var a = ReadByte();
			var r = ReadByte();
			var g = ReadByte();
			var b = ReadByte();

			return new Color(r, g, b, a);
		}

		public Vector2 ReadVector2()
		{
			var x = ReadSingle();
			var y = ReadSingle();

			return new Vector2(x, y);
		}

		public Triangle<TriangleVertexData> ReadTriangle()
		{
			var id = ReadInt32();
			var a = ReadDataVertex();
			var b = ReadDataVertex();
			var c = ReadDataVertex();

			return new Triangle<TriangleVertexData>(a, b, c, (int)id);
		}

		public IList<T> ReadList<T>(Func<SerializationReader, T> itemAction)
		{
			var count = ReadInt32();

			if (count < 0)
			{
				return null;
			}

			var result = new List<T>();

			for (var i = 0; i < count; i++)
			{
				result.Add(itemAction(this));
			}

			return result;
		}

		public object ReadObject()
		{
			return new BinaryFormatter().Deserialize(BaseStream);
		}

		public DataVertex<TriangleVertexData> ReadDataVertex()
		{
			var color = ReadColor();
			var scale = ReadVector2();
			var position = ReadVector2();
			var data = new TriangleVertexData(color, scale);

			return new DataVertex<TriangleVertexData>(position, data);
		}
	}
}
