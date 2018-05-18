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
            byte[] Bytes = (byte[])info.GetValue("X", typeof(byte[]));
            MemoryStream ms = new MemoryStream(Bytes);
            return new SerializationReader(ms);
        }

        public Color ReadColor()
        {
            var A = ReadByte();
            var R = ReadByte();
            var G = ReadByte();
            var B = ReadByte();

            return new Color(R, G, B, A);
        }

        public Vector2 ReadVector2()
        {
            var X = ReadSingle();
            var Y = ReadSingle();

            return new Vector2(X, Y);
        }

        public Triangle<TriangleVertexData> ReadTriangle()
        {
            var ID = ReadInt32();
            var A = ReadDataVertex();
            var B = ReadDataVertex();
            var C = ReadDataVertex();

            return new Triangle<TriangleVertexData>(A, B, C, (int)ID);
        }

        public IList<T> ReadList<T>(Func<SerializationReader, T> itemAction)
        {
            var Count = ReadInt32();

            if (Count < 0)
            {
                return null;
            }

            var Result = new List<T>();

            for (int i = 0; i < Count; i++)
            {
                Result.Add(itemAction(this));
            }

            return Result;
        }

        public object ReadObject()
        {
            return new BinaryFormatter().Deserialize(BaseStream);
        }

        public DataVertex<TriangleVertexData> ReadDataVertex()
        {
            var Color = ReadColor();
            var Scale = ReadVector2();
            var Position = ReadVector2();
            var Data = new TriangleVertexData(Color, Scale);

            return new DataVertex<TriangleVertexData>(Position, Data);
        }
    }
}
