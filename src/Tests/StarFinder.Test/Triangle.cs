using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace StarFinder.Test
{

    [TestClass]
    public class TriangleTests
    {
        [TestMethod]
        public void TriangleContainsEdges()
        {
            var Triangle = new Triangle<TestVertexData>(new Vector2(220, 480), new Vector2(670, 480), new Vector2(256, 380));
            foreach (var Edge in Triangle.Edges)
            {
                var Delta = Edge.Vertex1 - Edge.Vertex2;

                for (float a = 0; a <= 1; a += 0.0005f)
                {
                    Vector2 EdgePoint = Edge.Vertex2 + Delta * a;
                    Assert.IsTrue(Triangle.Encloses(EdgePoint), EdgePoint.ToString() + " " + a);
                    Assert.IsFalse(Edge.Crosses(EdgePoint, Edge.Vertex2));
                    Assert.IsFalse(Edge.Crosses(EdgePoint, Edge.Vertex1));
                }
            }
        }

        [TestMethod]
        public void TriangleEdgeContainsCenter()
        {
            var Triangle = new Triangle<TestVertexData>(Vector2.Zero, Vector2.UnitX, Vector2.UnitX);

            Assert.IsTrue(Triangle.Encloses(Vector2.UnitX / 2f));
        }

        [TestMethod]
        public void TriangleArea()
        {
            Assert.AreEqual(70 * 200 * 0.5f, Triangle<TestVertexData>.Area(new Vector2(0, 0), new Vector2(200, 0), new Vector2(100, 70)));
        }

        [TestMethod]
        public void PointInTriangle()
        {
            var Tri = new Triangle<TestVertexData>(new Vector2(100, 100), new Vector2(50, 150), new Vector2(250, 250), 0);

            Assert.IsTrue(Tri.Encloses(new Vector2(100, 100)));
            Assert.IsFalse(Tri.Encloses(new Vector2(99, 99)));
            Assert.IsTrue(Tri.Encloses(new Vector2(50, 150)));
            Assert.IsFalse(Tri.Encloses(new Vector2(49, 151)));
            Assert.IsTrue(Tri.Encloses(new Vector2(250, 250)));
            Assert.IsFalse(Tri.Encloses(new Vector2(251, 251)));
            Assert.IsTrue(Tri.Encloses(new Vector2(135, 164)));
        }

        [TestMethod]
        public void TriangleVertexDataInterpolation()
        {
            var Triangle = new Triangle<TestVertexData>(
                new DataVertex<TestVertexData>(new Vector2(0, 0), new TestVertexData(0)),
                new DataVertex<TestVertexData>(new Vector2(0, 100), new TestVertexData(1)),
                new DataVertex<TestVertexData>(new Vector2(100, 0), new TestVertexData(2))
            );

            Assert.AreEqual(0, ((TestVertexData)Triangle.GetVertexData(new Vector2(0, 0))).Value);
            Assert.AreEqual(1f, ((TestVertexData)Triangle.GetVertexData(new Vector2(0, 100))).Value);
            Assert.AreEqual(2f, ((TestVertexData)Triangle.GetVertexData(new Vector2(100, 0))).Value);

            Assert.AreEqual(0.5f, ((TestVertexData)Triangle.GetVertexData(new Vector2(0, 50))).Value);
            Assert.AreEqual(1f, ((TestVertexData)Triangle.GetVertexData(new Vector2(50, 0))).Value);

            Assert.AreEqual(((TestVertexData)Triangle.GetVertexData(new Vector2(100f / 3f, 100 / 3f))).Value, 1f, .0005f);
        }
     
    }
}
