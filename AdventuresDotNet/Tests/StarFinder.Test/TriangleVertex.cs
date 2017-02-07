using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace StarFinder.Test
{

    [TestClass]
    public class TriangleVertexTests
    {        
        [TestMethod]
        public void Equality()
        {
            Vertex One = new Vertex(Vector2.UnitX);
            Vertex Two = new Vertex(Vector2.UnitX);

            Assert.IsTrue(One == Two);
            Assert.IsTrue(One.Equals(Two));
        }

        Vertex A = new Vertex(-200, 0);
        Vertex BB = new Vector2(-100, 100);
        Vertex BT = new Vector2(-100, 200);
        Vertex BC = new Vector2(0, 150);
        Vertex CB = new Vector2(100, 100);
        Vertex CT = new Vector2(100, 200);
        Vertex D = new Vector2(200, 0);

        IEnumerable<Vertex> GetNeighbors(Vertex a) 
        {
            if (a == A) return new Vertex[] { BB, BT };
            if (a == BB) return new Vertex[] { A, BT, CB, CT };
            if (a == BT) return new Vertex[] { A, BB, CB, CT };
            if (a == CT) return new Vertex[] { D, BB, BT, CB };
            if (a == CB) return new Vertex[] { D, BB, BT, CT };
            if (a == D) return new Vertex[] { CB, CT };
            return new Vertex[] { };
        }

        [TestMethod]
        public void FindShortestPath1()
        {
            AStar<Vertex> Finder = new AStar<Vertex>(GetNeighbors);
            var Result = new List<Vertex>();
            Finder.Search(A, D, ref Result, Vertex.Heuristic);
            Assert.IsNotNull(Result);
            
            Assert.AreEqual(4,  Result.Count);
            Assert.AreEqual(A,  Result[0]);
            Assert.AreEqual(BB, Result[1]);
            Assert.AreEqual(CB, Result[2]);
            Assert.AreEqual(D,  Result[3]);            
        }

        IEnumerable<Vertex> GetNeighbors2(Vertex a)
        {
            if (a == A) return new Vertex[] { BB, BT };
            if (a == BB) return new Vertex[] { A, BT, BC };
            if (a == BT) return new Vertex[] { A, BB, CT };
            if (a == CT) return new Vertex[] { BC, BT };
            if (a == BC) return new Vertex[] { BB, CT };
            return new Vertex[] { };
        }

        [TestMethod]
        public void FindShortestPath2()
        {
            AStar<Vertex> Finder = new AStar<Vertex>(GetNeighbors2);
            var Result = new List<Vertex>();
            Finder.Search(A, CT, ref Result, null);
            Assert.IsNotNull(Result);            
            Assert.AreEqual(4,  Result.Count);
            Assert.AreEqual(A,  Result[0]);
            Assert.AreEqual(BB, Result[1]);
            Assert.AreEqual(BC, Result[2]);
            Assert.AreEqual(CT, Result[3]);
        }

        [TestMethod]
        public void FindShortestPath3()
        {
            DataVertex<TriangleVertexData>[] Points = new DataVertex<TriangleVertexData>[14];
            Points[0] = new DataVertex<TriangleVertexData>(new Vector2(0, 0));
            Points[1] = new DataVertex<TriangleVertexData>(new Vector2(100, 0));
            Points[2] = new DataVertex<TriangleVertexData>(new Vector2(0, 100));
            Points[3] = new DataVertex<TriangleVertexData>(new Vector2(100, 110));
            Points[4] = new DataVertex<TriangleVertexData>(new Vector2(50, 150));
            Points[5] = new DataVertex<TriangleVertexData>(new Vector2(250, 250));
            Points[6] = new DataVertex<TriangleVertexData>(new Vector2(500, 500));
            Points[7] = new DataVertex<TriangleVertexData>(new Vector2(250, 0));

            int[] Indices = new int[36];
            Indices[0] = 0; Indices[1] = 1; Indices[2] = 2;
            Indices[3] = 1; Indices[4] = 2; Indices[5] = 3;
            Indices[6] = 2; Indices[7] = 3; Indices[8] = 4;
            Indices[9] = 3; Indices[10] = 4; Indices[11] = 5;
            Indices[12] = 4; Indices[13] = 5; Indices[14] = 6;
            Indices[15] = 5; Indices[16] = 6; Indices[17] = 7;
            var Path = new Path<TriangleVertexData>(Points, Indices, 1.1f, 0.475f);

            var Result = new List<Vector2>();
            Path.FindPath(new Vector2(10, 10), new Vector2(250, 0), ref Result);
            Assert.AreNotEqual(Points[2], Result[1]);
        }

     
    }
}
