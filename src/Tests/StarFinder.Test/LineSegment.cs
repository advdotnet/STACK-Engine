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
			var edge = new LineSegment(Vector2.Zero, Vector2.UnitY * 3);
			var crosses = edge.Crosses(new Vector2(-1, 1), new Vector2(1, 1), out var intersection);

			Assert.IsTrue(crosses);
			Assert.AreEqual(Vector2.UnitY, intersection);
		}

		[TestMethod]
		public void OnePointSegmentNearestPoint()
		{
			var edge = new LineSegment(Vector2.Zero, Vector2.Zero);
			Assert.AreEqual(Vector2.Zero, edge.GetClosestPoint(Vector2.UnitY));
		}

		[TestMethod]
		public void Equality()
		{
			var edge1 = new LineSegment(Vector2.Zero, Vector2.UnitX);
			var edge2 = new LineSegment(Vector2.UnitX, Vector2.Zero);
			var edge3 = new LineSegment(Vector2.UnitY, Vector2.Zero);

			Assert.IsTrue(edge1 == edge2);
			Assert.IsTrue(edge1.Equals(edge2));

			Assert.IsFalse(edge1 == edge3);
			Assert.IsFalse(edge1.Equals(edge3));
		}

		[TestMethod]
		public void Crosses()
		{
			var edge = new LineSegment(-Vector2.UnitX, Vector2.UnitX);

			Assert.IsTrue(edge.Crosses(Vector2.UnitY, -Vector2.UnitY));
			Assert.IsFalse(edge.Crosses(Vector2.UnitX, Vector2.UnitX + Vector2.UnitY));
			Assert.IsFalse(edge.Crosses(new Vector2(-2, 1), new Vector2(-2, -1)));
		}

		[TestMethod]
		public void ClosestPoint()
		{
			var edge = new LineSegment(Vector2.Zero, Vector2.UnitX);

			Assert.AreEqual(Vector2.Zero, edge.GetClosestPoint(new Vector2(-0.1f, -0.1f)));
			Assert.AreEqual(Vector2.UnitX, edge.GetClosestPoint(new Vector2(1.1f, 0)));
			Assert.AreEqual(Vector2.UnitX / 2, edge.GetClosestPoint(new Vector2(0.5f, 10)));
		}

		[TestMethod]
		public void CrossesAccuracy()
		{
			var edge = new LineSegment(Vector2.UnitY, Vector2.UnitX);

			Assert.IsFalse(edge.Crosses(new Vector2(0.1f, 0.14f), new Vector2(0.998f, 0.002f)));
		}

		[TestMethod]
		public void CrossesAccuracyAlongEdge()
		{
			for (var i = 1; i < 3; i++)
			{
				var edge = new LineSegment(Vector2.Zero, new Vector2(10000 * i * 3000, 7008 * i * 3000));
				var dx = edge.Vertex1 - edge.Vertex2;

				for (var a = 0f; a <= 1; a += 0.05f)
				{
					var edgePoint = edge.Vertex2 + (dx * a);
					Assert.IsTrue(edge.Contains(edgePoint));
					Assert.IsTrue(edge.Contains(edge.Vertex1));
					Assert.IsTrue(edge.Contains(edge.Vertex2));
				}
			}
		}
	}
}
