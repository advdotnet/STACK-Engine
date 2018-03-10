using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using StarFinder;
using System;
using System.Collections.Generic;
using System.IO;

namespace STACK.Test
{


    [TestClass]
    public class PathState
    {
        [TestMethod]
        public void SerializePath()
        {
            var V1 = new Vector2(0, 0);
            var V2 = new Vector2(0.33f, 1);
            var V3 = new Vector2(1, 1);
            var V4 = new Vector2(1, 0);
            var V5 = new Vector2(1, 2);
            var V6 = new Vector2(2, 1);

            var A = new Triangle<TriangleVertexData>(V1, V4, V3);
            var B = new Triangle<TriangleVertexData>(V1, V2, V3);
            var C = new Triangle<TriangleVertexData>(V2, V3, V5);
            var D = new Triangle<TriangleVertexData>(V3, V5, V6);

            var Path = new Path(A, B, C, D);

            byte[] Check = STACK.State.Serialization.SaveState<Path>(Path);
            File.WriteAllBytes("pathstate.txt", Check);

            Console.Write(Check.Length + " bytes.");

            var NewPath = STACK.State.Serialization.LoadState<Path>(Check);
            Assert.AreEqual(2, NewPath.Mesh.MaxX);
            Assert.AreEqual(0, NewPath.Mesh.MinX);
            Assert.IsTrue(Check.Length < 4089);
            Assert.AreEqual(4, NewPath.Mesh.Triangles.Length);
            var WayPoints = new List<Vector2>();
            NewPath.FindPath(V1, V6, ref WayPoints);
        }
    }
}
