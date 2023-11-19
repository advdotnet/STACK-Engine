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
			var triangle = new Triangle<TestVertexData>(new Vector2(220, 480), new Vector2(670, 480), new Vector2(256, 380));
			foreach (var edge in triangle.Edges)
			{
				var delta = edge.Vertex1 - edge.Vertex2;

				for (float a = 0; a <= 1; a += 0.0005f)
				{
					var edgePoint = edge.Vertex2 + (delta * a);
					Assert.IsTrue(triangle.Encloses(edgePoint), edgePoint.ToString() + " " + a);
					Assert.IsFalse(edge.Crosses(edgePoint, edge.Vertex2));
					Assert.IsFalse(edge.Crosses(edgePoint, edge.Vertex1));
				}
			}
		}

		[TestMethod]
		public void TriangleEdgeContainsCenter()
		{
			var triangle = new Triangle<TestVertexData>(Vector2.Zero, Vector2.UnitX, Vector2.UnitX);

			Assert.IsTrue(triangle.Encloses(Vector2.UnitX / 2f));
		}

		[TestMethod]
		public void TriangleArea()
		{
			Assert.AreEqual(70 * 200 * 0.5f, Triangle<TestVertexData>.Area(new Vector2(0, 0), new Vector2(200, 0), new Vector2(100, 70)));
		}

		[TestMethod]
		public void PointInTriangle()
		{
			var tri = new Triangle<TestVertexData>(new Vector2(100, 100), new Vector2(50, 150), new Vector2(250, 250), 0);

			Assert.IsTrue(tri.Encloses(new Vector2(100, 100)));
			Assert.IsFalse(tri.Encloses(new Vector2(99, 99)));
			Assert.IsTrue(tri.Encloses(new Vector2(50, 150)));
			Assert.IsFalse(tri.Encloses(new Vector2(49, 151)));
			Assert.IsTrue(tri.Encloses(new Vector2(250, 250)));
			Assert.IsFalse(tri.Encloses(new Vector2(251, 251)));
			Assert.IsTrue(tri.Encloses(new Vector2(135, 164)));
		}

		[TestMethod]
		public void TriangleVertexDataInterpolation()
		{
			var triangle = new Triangle<TestVertexData>(
				new DataVertex<TestVertexData>(new Vector2(0, 0), new TestVertexData(0)),
				new DataVertex<TestVertexData>(new Vector2(0, 100), new TestVertexData(1)),
				new DataVertex<TestVertexData>(new Vector2(100, 0), new TestVertexData(2))
			);

			Assert.AreEqual(0, triangle.GetVertexData(new Vector2(0, 0)).Value);
			Assert.AreEqual(1f, triangle.GetVertexData(new Vector2(0, 100)).Value);
			Assert.AreEqual(2f, triangle.GetVertexData(new Vector2(100, 0)).Value);

			Assert.AreEqual(0.5f, triangle.GetVertexData(new Vector2(0, 50)).Value);
			Assert.AreEqual(1f, triangle.GetVertexData(new Vector2(50, 0)).Value);

			Assert.AreEqual(triangle.GetVertexData(new Vector2(100f / 3f, 100 / 3f)).Value, 1f, .0005f);
		}

	}
}
