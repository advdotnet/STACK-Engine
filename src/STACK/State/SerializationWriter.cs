using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using StarFinder;
using Microsoft.Xna.Framework;

namespace STACK.State
{
    public class SerializationWriter : BinaryWriter
    {
        public SerializationWriter(Stream s) : base(s) { }

        public static SerializationWriter Create()
        {
            var MemoryStream = new MemoryStream(1024);
            return new SerializationWriter(MemoryStream);
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
                foreach (T item in collection)
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
            byte[] Bytes = ((MemoryStream)BaseStream).ToArray();
            info.AddValue("X", Bytes, typeof(byte[]));
        }
    }

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
