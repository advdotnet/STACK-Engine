using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StarFinder.Test
{

	[TestClass]
	public class TriangleVertexTests
	{
		[TestMethod]
		public void Equality()
		{
			var one = new Vertex(Vector2.UnitX);
			var two = new Vertex(Vector2.UnitX);

			Assert.IsTrue(one == two);
			Assert.IsTrue(one.Equals(two));
		}

		private readonly Vertex _a = new Vertex(-200, 0);
		private readonly Vertex _bb = new Vector2(-100, 100);
		private readonly Vertex _bt = new Vector2(-100, 200);
		private readonly Vertex _bc = new Vector2(0, 150);
		private readonly Vertex _cb = new Vector2(100, 100);
		private readonly Vertex _ct = new Vector2(100, 200);
		private readonly Vertex _d = new Vector2(200, 0);

		private IEnumerable<Vertex> GetNeighbors(Vertex a)
		{
			if (a == _a)
			{
				return new Vertex[] { _bb, _bt };
			}

			if (a == _bb)
			{
				return new Vertex[] { _a, _bt, _cb, _ct };
			}

			if (a == _bt)
			{
				return new Vertex[] { _a, _bb, _cb, _ct };
			}

			if (a == _ct)
			{
				return new Vertex[] { _d, _bb, _bt, _cb };
			}

			if (a == _cb)
			{
				return new Vertex[] { _d, _bb, _bt, _ct };
			}

			if (a == _d)
			{
				return new Vertex[] { _cb, _ct };
			}

			return new Vertex[] { };
		}

		[TestMethod]
		public void FindShortestPath1()
		{
			var finder = new AStar<Vertex>(GetNeighbors);
			var result = new List<Vertex>();
			finder.Search(_a, _d, ref result, Vertex.Heuristic);
			Assert.IsNotNull(result);

			Assert.AreEqual(4, result.Count);
			Assert.AreEqual(_a, result[0]);
			Assert.AreEqual(_bb, result[1]);
			Assert.AreEqual(_cb, result[2]);
			Assert.AreEqual(_d, result[3]);
		}

		private IEnumerable<Vertex> GetNeighbors2(Vertex a)
		{
			if (a == _a)
			{
				return new Vertex[] { _bb, _bt };
			}

			if (a == _bb)
			{
				return new Vertex[] { _a, _bt, _bc };
			}

			if (a == _bt)
			{
				return new Vertex[] { _a, _bb, _ct };
			}

			if (a == _ct)
			{
				return new Vertex[] { _bc, _bt };
			}

			if (a == _bc)
			{
				return new Vertex[] { _bb, _ct };
			}

			return new Vertex[] { };
		}

		[TestMethod]
		public void FindShortestPath2()
		{
			var finder = new AStar<Vertex>(GetNeighbors2);
			var result = new List<Vertex>();
			finder.Search(_a, _ct, ref result, null);
			Assert.IsNotNull(result);
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual(_a, result[0]);
			Assert.AreEqual(_bb, result[1]);
			Assert.AreEqual(_bc, result[2]);
			Assert.AreEqual(_ct, result[3]);
		}

		[TestMethod]
		public void FindShortestPath3()
		{
			var points = new DataVertex<TriangleVertexData>[14];
			points[0] = new DataVertex<TriangleVertexData>(new Vector2(0, 0));
			points[1] = new DataVertex<TriangleVertexData>(new Vector2(100, 0));
			points[2] = new DataVertex<TriangleVertexData>(new Vector2(0, 100));
			points[3] = new DataVertex<TriangleVertexData>(new Vector2(100, 110));
			points[4] = new DataVertex<TriangleVertexData>(new Vector2(50, 150));
			points[5] = new DataVertex<TriangleVertexData>(new Vector2(250, 250));
			points[6] = new DataVertex<TriangleVertexData>(new Vector2(500, 500));
			points[7] = new DataVertex<TriangleVertexData>(new Vector2(250, 0));

			var indices = new int[36];
			indices[0] = 0; indices[1] = 1; indices[2] = 2;
			indices[3] = 1; indices[4] = 2; indices[5] = 3;
			indices[6] = 2; indices[7] = 3; indices[8] = 4;
			indices[9] = 3; indices[10] = 4; indices[11] = 5;
			indices[12] = 4; indices[13] = 5; indices[14] = 6;
			indices[15] = 5; indices[16] = 6; indices[17] = 7;
			var path = new Path<TriangleVertexData>(points, indices, 1.1f, 0.475f);

			var result = new List<Vector2>();
			path.FindPath(new Vector2(10, 10), new Vector2(250, 0), ref result);
			Assert.AreNotEqual(points[2], result[1]);
		}
	}
}
