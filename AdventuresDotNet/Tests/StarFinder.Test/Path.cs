using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace StarFinder.Test
{
    
    public class PathVertex : DataVertex<TriangleVertexData>
    {
        public PathVertex(Vector2 point) : base(point) { }

        public PathVertex(Vector2 point, TriangleVertexData data) : base(point, data) { }

        public PathVertex(float x, float y) : base(x, y) { }

        public PathVertex(float x, float y, TriangleVertexData data) : base(x, y, data) { }
    }

    public class Path : Path<TriangleVertexData>
    {
        public Path(DataVertex<TriangleVertexData>[] points, int[] indices) : base(points, indices) { }

        public Path(Mesh<TriangleVertexData> mesh, float maxScale, float minScale) : base(mesh, maxScale, minScale) { }

        public Path(DataVertex<TriangleVertexData>[] points, int[] indices, float maxScale, float minScale) : base(points, indices, maxScale, minScale) { }

        public Path(List<Triangle<TriangleVertexData>> triangles) : base(triangles) { }

        public Path(params Triangle<TriangleVertexData>[] triangles) : base(triangles) { }

        public Path() : base() { }        
    }

    [TestClass]
    public class PathTest
    {
        [TestMethod]
        public void ZeroAreaTest()
        {            
            var T1 = new Triangle<TestVertexData>(Vector2.Zero, Vector2.Zero, Vector2.UnitX);
            var T2 = new Triangle<TestVertexData>(Vector2.Zero, Vector2.Zero, Vector2.UnitY);
            var Mesh = new Path<TestVertexData>(T1, T2);

            var Result = new List<Vector2>();
            Mesh.FindPath(Vector2.UnitX, Vector2.UnitY, ref Result);

            Assert.AreEqual(3, Result.Count);
            Assert.AreEqual(Vector2.UnitX, Result[0]);
            Assert.AreEqual(Vector2.Zero, Result[1]);
            Assert.AreEqual(Vector2.UnitY, Result[2]);
        }

        [TestMethod]
        public void LeutnantsRoomIssue()
        {
            var A = new Vector2(1050, 482);
            var B = new Vector2(593, 555);
            var C = new Vector2(343, 476);            

            var Start = new Vector2(497.5439f, 477.3116f);
            var End = new Vector2(917, 238);

            var T1 = new Triangle<TestVertexData>(A, B, C);            
            var Mesh = new Path<TestVertexData>(T1);

            var Result = new List<Vector2>();
            Mesh.FindPath(Start, End, ref Result);

            Assert.IsTrue(Mesh.Contains(Start));            
            Assert.AreEqual(2, Result.Count);
        }

        /// <summary>
        /// If there are two edges on a line separated by a space in between, there must
        /// not be a way from one to another.
        /// </summary>
        [TestMethod]
        public void NoPathBetweenTwoEdgesOnSameLine()
        {
            var A = new Vector2(0, 0);
            var B = new Vector2(100, 0);
            var ABB = new Vector2(50, 100);

            var C = new Vector2(250, 0);
            var D = new Vector2(500, 0);
            var CDB = new Vector2(375, 100);

            var T1 = new Triangle<TestVertexData>(A, B, ABB);
            var T2 = new Triangle<TestVertexData>(C, D, CDB);

            var Mesh = new Path<TestVertexData>(T1, T2);
            var Result = new List<Vector2>();
            Mesh.FindPath(A, D, ref Result);            
            Assert.AreEqual(0, Result.Count);
        }

        /// <summary>
        /// Asserts that the shorter path to the goal is returned.
        /// </summary>
        [TestMethod]
        public void ReturnsShortestPath()
        {
            var V1 = new Vector2(0, 0);
            var V2 = new Vector2(0.33f, 1);
            var V3 = new Vector2(1, 1);
            var V4 = new Vector2(1, 0);
            var V5 = new Vector2(1, 2);
            var V6 = new Vector2(2, 1);

            var A = new Triangle<TestVertexData>(V1, V4, V3);
            var B = new Triangle<TestVertexData>(V1, V2, V3);
            var C = new Triangle<TestVertexData>(V2, V3, V5);
            var D = new Triangle<TestVertexData>(V3, V5, V6);

            var Mesh = new Path<TestVertexData>(A, B, C, D);

            var Result = new List<Vector2>();
            Mesh.FindPath(new Vector2(1.99f, 1.05f), new Vector2(0.02f, 0.33f), ref Result);
            Assert.AreEqual(3, Result.Count);
        }

        /// <summary>
        /// Any point returned by the GetClosestPoint method can be reached.
        /// </summary>
        [TestMethod]
        public void GetClosestPointDoesNotCrossItsEdge()
        {
            var LB = new Vector2(0, 0);
            var R = new Vector2(1, 0);
            var T = new Vector2(0, 1);

            var Start = new Vector2(0.1f, 0.14f);

            var A = new Triangle<TestVertexData>(LB, R, T);
            var Mesh = new Path<TestVertexData>(A);

            for (float a = 0; a <= 1; a += 0.0005f)
            {
                var Test1 = new Vector2(a, 1);
                var Test2 = new Vector2(1, a);

                var Closest1 = A.GetClosestPoint(Test1);
                var Closest2 = A.GetClosestPoint(Test2);

                var Res1 = new List<Vector2>();
                var Res2 = new List<Vector2>();
                var Res3 = new List<Vector2>();
                var Res4 = new List<Vector2>();

                Mesh.FindPath(Start, Closest1, ref Res3);
                Assert.AreEqual(2, Res3.Count);
                Mesh.FindPath(Start, Closest2, ref Res4);
                Assert.AreEqual(2, Res4.Count);

                Mesh.FindPath(Start, Closest1, ref Res1);
                Assert.AreEqual(2, Res1.Count);
                Mesh.FindPath(Start, Closest2, ref Res2);
                Assert.AreEqual(2, Res2.Count);
            }
        }

        /// <summary>
        /// Tests two simple adjacent pointy triangles
        /// </summary>
        [TestMethod]
        public void PathA()
        {
            var Left = new Vector2(-100, 0);
            var Right = new Vector2(100, 0);
            var Top = new Vector2(0, 200);
            var Bottom = new Vector2(0, 100);

            var A = new Triangle<TestVertexData>(Bottom, Top, Left);
            var B = new Triangle<TestVertexData>(Right, Top, Bottom);

            var Mesh = new Path<TestVertexData>(A, B);

            var Result = new List<Vector2>();
            Mesh.FindPath(Left, Right, ref Result);
            Assert.AreEqual(3, Result.Count);
            Assert.AreEqual(Left, Result[0]);
            Assert.AreEqual(Bottom, Result[1]);
            Assert.AreEqual(Right, Result[2]);

            Mesh.FindPath(Right, Left, ref Result);
            Assert.AreEqual(3, Result.Count);
            Assert.AreEqual(Left, Result[2]);
            Assert.AreEqual(Bottom, Result[1]);
            Assert.AreEqual(Right, Result[0]);
        }

        /// <summary>
        /// Tests two pointy triangles with a bridge inbetween
        /// </summary>
        [TestMethod]
        public void PathB()
        {
            var Left = new Vector2(-200, 0);
            var Right = new Vector2(200, 0);
            var Top = new Vector2(0, 200);
            var Bottom = new Vector2(0, 100);

            var A = new Triangle<TestVertexData>(Left, Top - new Vector2(100, 0), Bottom - new Vector2(100, 0));
            var B = new Triangle<TestVertexData>(Top - new Vector2(100, 0), Bottom - new Vector2(100, 0), Top + new Vector2(100, 0));
            var C = new Triangle<TestVertexData>(Top + new Vector2(100, 0), Bottom - new Vector2(100, 0), Bottom + new Vector2(100, 0));
            var D = new Triangle<TestVertexData>(Top + new Vector2(100, 0), Bottom + new Vector2(100, 0), Right);

            var Mesh = new Path<TestVertexData>(A, B, C, D);

            var Result = new List<Vector2>();
            Mesh.FindPath(Left, Right, ref Result);
            Assert.AreEqual(4, Result.Count);
            Assert.AreEqual(Left, Result[0]);
            Assert.AreEqual(Bottom - new Vector2(100, 0), Result[1]);
            Assert.AreEqual(Bottom + new Vector2(100, 0), Result[2]);
            Assert.AreEqual(Right, Result[3]);

            Mesh.FindPath(Right, Left, ref Result);
            Assert.AreEqual(4, Result.Count);
            Assert.AreEqual(Left, Result[3]);
            Assert.AreEqual(Bottom - new Vector2(100, 0), Result[2]);
            Assert.AreEqual(Bottom + new Vector2(100, 0), Result[1]);
            Assert.AreEqual(Right, Result[0]);

            Mesh.FindPath(new Vector2(101, 100), new Vector2(-101, 100), ref Result);
            Assert.AreEqual(2, Result.Count);
            Assert.AreEqual(new Vector2(-101, 100), Result[1]);
            Assert.AreEqual(new Vector2(101, 100), Result[0]);

            Mesh.FindPath(new Vector2(-101, 100), new Vector2(101, 100), ref Result);
            Assert.AreEqual(2, Result.Count);
            Assert.AreEqual(new Vector2(101, 100), Result[1]);
            Assert.AreEqual(new Vector2(-101, 100), Result[0]);

            Mesh.FindPath(new Vector2(-100, 100), new Vector2(102, 98), ref Result);
            Assert.AreEqual(3, Result.Count);
            Assert.AreEqual(new Vector2(-100, 100), Result[0]);
            Assert.AreEqual(new Vector2(100, 100), Result[1]);
            Assert.AreEqual(new Vector2(102, 98), Result[2]);
        }

        [TestMethod]
        public void PathBStartOnCommonEdge()
        {
            var Left = new Vector2(-200, 0);
            var Right = new Vector2(200, 0);
            var Top = new Vector2(0, 200);
            var Bottom = new Vector2(0, 100);

            var A = new Triangle<TestVertexData>(Left, Top - new Vector2(100, 0), Bottom - new Vector2(100, 0));
            var B = new Triangle<TestVertexData>(Top - new Vector2(100, 0), Bottom - new Vector2(100, 0), Top + new Vector2(100, 0));
            var C = new Triangle<TestVertexData>(Top + new Vector2(100, 0), Bottom - new Vector2(100, 0), Bottom + new Vector2(100, 0));
            var D = new Triangle<TestVertexData>(Top + new Vector2(100, 0), Bottom + new Vector2(100, 0), Right);

            var Mesh = new Path<TestVertexData>(A, B, C, D);

            var Result = new List<Vector2>();
            Mesh.FindPath(new Vector2(-100, 150), Right, ref Result);

            Assert.AreEqual(3, Result.Count);
            Assert.AreEqual(new Vector2(-100, 150), Result[0]);
            Assert.AreEqual(Bottom + new Vector2(100, 0), Result[1]);
            Assert.AreEqual(Right, Result[2]);
        }

        [TestMethod]
        public void PathC()
        {
            var Left = new Vector2(-200, 0);
            var Right = new Vector2(200, 0);
            var Top = new Vector2(0, 200);
            var Bottom = new Vector2(0, 100);

            var A = new Triangle<TestVertexData>(Left, Top - new Vector2(100, 0), Bottom - new Vector2(100, 0));
            var B = new Triangle<TestVertexData>(Top - new Vector2(100, 0), Bottom - new Vector2(100, 0), Top + new Vector2(100, 0));
            var C = new Triangle<TestVertexData>(Top + new Vector2(100, 0), Bottom - new Vector2(100, 0), Bottom + new Vector2(100, 0));
            var D = new Triangle<TestVertexData>(Top + new Vector2(100, 0), Bottom + new Vector2(100, 0), Right);

            var Mesh = new Path<TestVertexData>(A, B, C, D);

            var Result = new List<Vector2>();
            Mesh.FindPath(new Vector2(-129, 97), Right, ref Result);

            Assert.AreEqual(4, Result.Count);
            Assert.AreEqual(new Vector2(-129, 97), Result[0]);
            Assert.AreEqual(Bottom - new Vector2(100, 0), Result[1]);
            Assert.AreEqual(Bottom + new Vector2(100, 0), Result[2]);
            Assert.AreEqual(Right, Result[3]);
        }

        /// <summary>
        /// Tests a more complicated mesh
        /// </summary>
        [TestMethod]
        public void PathD()
        {
            var Points = new DataVertex<TestVertexData>[14];
            Points[0] = new DataVertex<TestVertexData>(new Vector2(0, 0));
            Points[1] = new DataVertex<TestVertexData>(new Vector2(100, 0));
            Points[2] = new DataVertex<TestVertexData>(new Vector2(0, 100));
            Points[3] = new DataVertex<TestVertexData>(new Vector2(100, 110));
            Points[4] = new DataVertex<TestVertexData>(new Vector2(50, 150));
            Points[5] = new DataVertex<TestVertexData>(new Vector2(250, 250));
            Points[6] = new DataVertex<TestVertexData>(new Vector2(500, 500));
            Points[7] = new DataVertex<TestVertexData>(new Vector2(250, 0));
            Points[8] = new DataVertex<TestVertexData>(new Vector2(500, 0));
            Points[9] = new DataVertex<TestVertexData>(new Vector2(600, 100));
            Points[10] = new DataVertex<TestVertexData>(new Vector2(700, 0));
            Points[11] = new DataVertex<TestVertexData>(new Vector2(900, 400));
            Points[12] = new DataVertex<TestVertexData>(new Vector2(602, 400));
            Points[13] = new DataVertex<TestVertexData>(new Vector2(300, 800));


            int[] Indices = new int[36];
            Indices[0] = 0; Indices[1] = 1; Indices[2] = 2;
            Indices[3] = 1; Indices[4] = 2; Indices[5] = 3;
            Indices[6] = 2; Indices[7] = 3; Indices[8] = 4;
            Indices[9] = 3; Indices[10] = 4; Indices[11] = 5;
            Indices[12] = 4; Indices[13] = 5; Indices[14] = 6;
            Indices[15] = 5; Indices[16] = 6; Indices[17] = 7;
            Indices[18] = 6; Indices[19] = 7; Indices[20] = 8;
            Indices[21] = 6; Indices[22] = 8; Indices[23] = 9;
            Indices[24] = 8; Indices[25] = 9; Indices[26] = 10;
            Indices[27] = 9; Indices[28] = 10; Indices[29] = 11;
            Indices[30] = 9; Indices[31] = 12; Indices[32] = 11;
            Indices[33] = 11; Indices[34] = 12; Indices[35] = 13;

            var Collection = new Mesh<TestVertexData>(Points, Indices);
            var Triangles = Collection.Triangles.ToList();

            var Mesh = new Path<TestVertexData>(Points, Indices);

            var Result = new List<Vector2>(); 
            Mesh.FindPath(Triangles.First().A, Triangles.Last().C, ref Result);

            Assert.AreEqual(6, Result.Count);
            Assert.AreEqual(Triangles.First().A, Result[0]);
            Assert.AreEqual(Points[3].Point, Result[1]);
            Assert.AreEqual(Points[5].Point, Result[2]);
            Assert.AreEqual(Points[9].Point, Result[3]);
            Assert.AreEqual(Points[12].Point, Result[4]);
            Assert.AreEqual(Triangles.Last().C, Result[5]);

            var SubList = Triangles.GetRange(4, 6);

            Mesh.FindPath(SubList.First().A, SubList.Last().C, ref Result);

            Assert.AreEqual(SubList.First().A, Result[0]);
            Assert.AreEqual(Points[5].Point, Result[1]);
            Assert.AreEqual(Points[9].Point, Result[2]);
            Assert.AreEqual(SubList.Last().C, Result[3]);
        }


        /// <summary>
        /// Tests a more complicated mesh enclosing a hole
        /// </summary>
        [TestMethod]
        public void PathE()
        {
            var Points = new DataVertex<TestVertexData>[9 + 5 + 1];
            Points[0] = new DataVertex<TestVertexData>(new Vector2(0, 0));
            Points[1] = new DataVertex<TestVertexData>(new Vector2(100, 0));
            Points[2] = new DataVertex<TestVertexData>(new Vector2(0, 100));
            Points[3] = new DataVertex<TestVertexData>(new Vector2(100, 110));
            Points[4] = new DataVertex<TestVertexData>(new Vector2(50, 150));
            Points[5] = new DataVertex<TestVertexData>(new Vector2(250, 250));
            Points[6] = new DataVertex<TestVertexData>(new Vector2(500, 500));
            Points[7] = new DataVertex<TestVertexData>(new Vector2(250, 0));
            Points[8] = new DataVertex<TestVertexData>(new Vector2(500, 0));
            Points[9] = new DataVertex<TestVertexData>(new Vector2(600, 100));
            Points[10] = new DataVertex<TestVertexData>(new Vector2(700, 0));
            Points[11] = new DataVertex<TestVertexData>(new Vector2(900, 400));
            Points[12] = new DataVertex<TestVertexData>(new Vector2(602, 400));
            Points[13] = new DataVertex<TestVertexData>(new Vector2(700, 700));
            Points[14] = new DataVertex<TestVertexData>(new Vector2(500, 700));


            int[] Indices = new int[45];
            Indices[0] = 0; Indices[1] = 1; Indices[2] = 2;
            Indices[3] = 1; Indices[4] = 2; Indices[5] = 3;
            Indices[6] = 2; Indices[7] = 3; Indices[8] = 4;
            Indices[9] = 3; Indices[10] = 4; Indices[11] = 5;
            Indices[12] = 4; Indices[13] = 5; Indices[14] = 6;
            Indices[15] = 5; Indices[16] = 6; Indices[17] = 7;
            Indices[18] = 6; Indices[19] = 7; Indices[20] = 8;
            Indices[21] = 6; Indices[22] = 8; Indices[23] = 9;
            Indices[24] = 8; Indices[25] = 9; Indices[26] = 10;
            Indices[27] = 9; Indices[28] = 10; Indices[29] = 11;
            Indices[30] = 9; Indices[31] = 12; Indices[32] = 11;
            Indices[33] = 11; Indices[34] = 12; Indices[35] = 13;
            Indices[36] = 6; Indices[37] = 14; Indices[38] = 4;
            Indices[39] = 14; Indices[40] = 6; Indices[41] = 13;
            Indices[42] = 12; Indices[43] = 6; Indices[44] = 13;

            var Collection = new Mesh<TestVertexData>(Points, Indices);
            var Triangles = Collection.Triangles.ToList();

            var Mesh = new Path<TestVertexData>(Points, Indices);

            var SubList = Triangles.GetRange(3, 5);
            var Result = new List<Vector2>();
            Mesh.FindPath(new Vector2(68, 159), new Vector2(509, 434), ref Result);
            Assert.AreEqual(2, Result.Count);

            Mesh.FindPath(new Vector2(92, 171), new Vector2(509, 434), ref Result);
            Assert.AreEqual(2, Result.Count);

            Mesh.FindPath(new Vector2(130, 190), new Vector2(509, 434), ref Result);
            Assert.AreEqual(2, Result.Count);

            Mesh.FindPath(new Vector2(66, 158), new Vector2(509, 434), ref Result);
            Assert.AreEqual(2, Result.Count);
        }

        /// <summary>
        /// Performance test.
        /// </summary>
        [TestMethod]
        [TestCategory("performance")]
        public void Performance()
        {
            var Points = new DataVertex<TestVertexData>[9 + 5 + 1];
            Points[0] = new DataVertex<TestVertexData>(new Vector2(0, 0));
            Points[1] = new DataVertex<TestVertexData>(new Vector2(100, 0));
            Points[2] = new DataVertex<TestVertexData>(new Vector2(0, 100));
            Points[3] = new DataVertex<TestVertexData>(new Vector2(100, 110));
            Points[4] = new DataVertex<TestVertexData>(new Vector2(50, 150));
            Points[5] = new DataVertex<TestVertexData>(new Vector2(250, 250));
            Points[6] = new DataVertex<TestVertexData>(new Vector2(500, 500));
            Points[7] = new DataVertex<TestVertexData>(new Vector2(250, 0));
            Points[8] = new DataVertex<TestVertexData>(new Vector2(500, 0));
            Points[9] = new DataVertex<TestVertexData>(new Vector2(600, 100));
            Points[10] = new DataVertex<TestVertexData>(new Vector2(700, 0));
            Points[11] = new DataVertex<TestVertexData>(new Vector2(900, 400));
            Points[12] = new DataVertex<TestVertexData>(new Vector2(602, 400));
            Points[13] = new DataVertex<TestVertexData>(new Vector2(700, 700));
            Points[14] = new DataVertex<TestVertexData>(new Vector2(500, 700));


            int[] Indices = new int[45];
            Indices[0] = 0; Indices[1] = 1; Indices[2] = 2;
            Indices[3] = 1; Indices[4] = 2; Indices[5] = 3;
            Indices[6] = 2; Indices[7] = 3; Indices[8] = 4;
            Indices[9] = 3; Indices[10] = 4; Indices[11] = 5;
            Indices[12] = 4; Indices[13] = 5; Indices[14] = 6;
            Indices[15] = 5; Indices[16] = 6; Indices[17] = 7;
            Indices[18] = 6; Indices[19] = 7; Indices[20] = 8;
            Indices[21] = 6; Indices[22] = 8; Indices[23] = 9;
            Indices[24] = 8; Indices[25] = 9; Indices[26] = 10;
            Indices[27] = 9; Indices[28] = 10; Indices[29] = 11;
            Indices[30] = 9; Indices[31] = 12; Indices[32] = 11;
            Indices[33] = 11; Indices[34] = 12; Indices[35] = 13;
            Indices[36] = 6; Indices[37] = 14; Indices[38] = 4;
            Indices[39] = 14; Indices[40] = 6; Indices[41] = 13;
            Indices[42] = 12; Indices[43] = 6; Indices[44] = 13;

            var Path = new Path<TestVertexData>();
            
            System.Diagnostics.Stopwatch Watch = new System.Diagnostics.Stopwatch();
            Watch.Start();

            for (int i = 0; i < 100; i++)
            {
                Path = new Path<TestVertexData>(Points, Indices);
            }

            Watch.Stop();
            System.Diagnostics.Debug.WriteLine("Constructor: "+ Watch.Elapsed);
            Watch.Restart();

            for (int i = 0; i < 100; i++)
            {
                foreach (var PointA in Points)
                {
                    foreach (var PointB in Points)
                    {
                        if (PointB != PointA)
                        {
                            var Result = new List<Vector2>();
                            Path.FindPath(PointA, PointB, ref Result);
                            Assert.IsFalse(Result.Count == 0);
                        }
                    }
                }
            }

            Watch.Stop();
            System.Diagnostics.Debug.WriteLine("Searching: " + Watch.Elapsed);            
        }

        [TestMethod]
        public void PathVertexDataInterpolation()
        {
            var Points = new DataVertex<TestVertexData>[3];
            Points[0] = new DataVertex<TestVertexData>(new Vector2(0, 0), new TestVertexData(0));
            Points[1] = new DataVertex<TestVertexData>(new Vector2(0, 100), new TestVertexData(1));
            Points[2] = new DataVertex<TestVertexData>(new Vector2(100, 0), new TestVertexData(2));

            int[] Indices = new int[3];
            Indices[0] = 0; Indices[1] = 1; Indices[2] = 2;

            Path<TestVertexData> Mesh = new Path<TestVertexData>(Points, Indices);

            Assert.AreEqual(Mesh.GetVertexData(new Vector2(100f / 3f, 100 / 3f)).Value, 1f, .0005f);
        }

        /// <summary>
        /// Tests a triangle made up of three smaller triangles.
        /// </summary>
        [TestMethod]
        public void ContiguousTriangles()
        {
            var Left = new Vector2(-100, 0);
            var Right = new Vector2(100, 0);
            var Top = new Vector2(0, 200);
            var Common = new Vector2(0, 100);

            var A = new Triangle<TestVertexData>(Common, Top, Left);
            var B = new Triangle<TestVertexData>(Right, Top, Common);
            var C = new Triangle<TestVertexData>(Left, Common, Right);

            var Path = new Path<TestVertexData>(A, B, C);

            var Result = new List<Vector2>();
            Path.FindPath(Left, Right, ref Result);

            Assert.AreEqual(2, Result.Count);
            Assert.AreEqual(Left, Result[0]);
            Assert.AreEqual(Right, Result[1]);

            Path.FindPath(Right, Left, ref Result);

            Assert.AreEqual(2, Result.Count);
            Assert.AreEqual(Left, Result[1]);
            Assert.AreEqual(Right, Result[0]);
        }
     
    }
}
