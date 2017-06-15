using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace StarFinder.Test
{
    [TestClass]
    public class TriangleEdgeTests
    {        
        [TestMethod]
        public void IntersectionPoint()
        {
            Vector2 Intersection;
            var Edge = new LineSegment(Vector2.Zero, Vector2.UnitY * 3);            
            var Crosses = Edge.Crosses(new Vector2(-1, 1), new Vector2(1, 1), out Intersection);

            Assert.IsTrue(Crosses);
            Assert.AreEqual(Vector2.UnitY, Intersection);            
        }

        [TestMethod]
        public void OnePointSegmentNearestPoint()
        {
            var Edge = new LineSegment(Vector2.Zero, Vector2.Zero);
            Assert.AreEqual(Vector2.Zero, Edge.GetClosestPoint(Vector2.UnitY));
        }

        [TestMethod]
        public void Equality()
        {
            var Edge1 = new LineSegment(Vector2.Zero, Vector2.UnitX);
            var Edge2 = new LineSegment(Vector2.UnitX, Vector2.Zero);
            var Edge3 = new LineSegment(Vector2.UnitY, Vector2.Zero);

            Assert.IsTrue(Edge1 == Edge2);
            Assert.IsTrue(Edge1.Equals(Edge2));

            Assert.IsFalse(Edge1 == Edge3);
            Assert.IsFalse(Edge1.Equals(Edge3));
        }   
   
        [TestMethod]
        public void Crosses()
        {
            var Edge = new LineSegment(-Vector2.UnitX, Vector2.UnitX);

            Assert.IsTrue(Edge.Crosses(Vector2.UnitY, -Vector2.UnitY));
            Assert.IsFalse(Edge.Crosses(Vector2.UnitX, Vector2.UnitX + Vector2.UnitY));
            Assert.IsFalse(Edge.Crosses(new Vector2(-2, 1), new Vector2(-2, -1)));
        }

        [TestMethod]
        public void ClosestPoint()
        {
            var Edge = new LineSegment(Vector2.Zero, Vector2.UnitX);

            Assert.AreEqual(Vector2.Zero, Edge.GetClosestPoint(new Vector2(-0.1f, -0.1f)));
            Assert.AreEqual(Vector2.UnitX, Edge.GetClosestPoint(new Vector2(1.1f, 0)));
            Assert.AreEqual(Vector2.UnitX / 2, Edge.GetClosestPoint(new Vector2(0.5f, 10)));
        }

        [TestMethod]
        public void CrossesAccuracy()
        {
            var Edge = new LineSegment(Vector2.UnitY, Vector2.UnitX);

            Assert.IsFalse(Edge.Crosses(new Vector2(0.1f, 0.14f), new Vector2(0.998f, 0.002f)));
        }     

        [TestMethod]
        public void CrossesAccuracyAlongEdge()
        {            
            for (int i = 1; i < 3; i++)
            {
                var Edge = new LineSegment(Vector2.Zero, new Vector2(10000 * i*3000, 7008 * i*3000));
                var Dx = (Vector2)(Edge.Vertex1 - Edge.Vertex2);

                for (float a = 0f; a <= 1; a += 0.05f)
                {
                    Vector2 EdgePoint = Edge.Vertex2 + Dx * a;
                    Assert.IsTrue(Edge.Contains(EdgePoint));
                    Assert.IsTrue(Edge.Contains(Edge.Vertex1));
                    Assert.IsTrue(Edge.Contains(Edge.Vertex2));
                }
            }
        }           
    }
}
