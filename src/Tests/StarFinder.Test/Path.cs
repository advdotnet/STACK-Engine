using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

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
			var t1 = new Triangle<TestVertexData>(Vector2.Zero, Vector2.Zero, Vector2.UnitX);
			var t2 = new Triangle<TestVertexData>(Vector2.Zero, Vector2.Zero, Vector2.UnitY);
			var mesh = new Path<TestVertexData>(t1, t2);

			var result = new List<Vector2>();
			mesh.FindPath(Vector2.UnitX, Vector2.UnitY, ref result);

			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(Vector2.UnitX, result[0]);
			Assert.AreEqual(Vector2.Zero, result[1]);
			Assert.AreEqual(Vector2.UnitY, result[2]);
		}

		[TestMethod]
		public void LeutnantsRoomIssue()
		{
			var a = new Vector2(1050, 482);
			var b = new Vector2(593, 555);
			var c = new Vector2(343, 476);

			var start = new Vector2(497.5439f, 477.3116f);
			var end = new Vector2(917, 238);

			var t1 = new Triangle<TestVertexData>(a, b, c);
			var mesh = new Path<TestVertexData>(t1);

			var result = new List<Vector2>();
			mesh.FindPath(start, end, ref result);

			Assert.IsTrue(mesh.Contains(start));
			Assert.AreEqual(2, result.Count);
		}

		/// <summary>
		/// If there are two edges on a line separated by a space in between, there must
		/// not be a way from one to another.
		/// </summary>
		[TestMethod]
		public void NoPathBetweenTwoEdgesOnSameLine()
		{
			var a = new Vector2(0, 0);
			var b = new Vector2(100, 0);
			var abb = new Vector2(50, 100);

			var c = new Vector2(250, 0);
			var d = new Vector2(500, 0);
			var cdb = new Vector2(375, 100);

			var t1 = new Triangle<TestVertexData>(a, b, abb);
			var t2 = new Triangle<TestVertexData>(c, d, cdb);

			var mesh = new Path<TestVertexData>(t1, t2);
			var result = new List<Vector2>();
			mesh.FindPath(a, d, ref result);
			Assert.AreEqual(0, result.Count);
		}

		/// <summary>
		/// Asserts that the shorter path to the goal is returned.
		/// </summary>
		[TestMethod]
		public void ReturnsShortestPath()
		{
			var v1 = new Vector2(0, 0);
			var v2 = new Vector2(0.33f, 1);
			var v3 = new Vector2(1, 1);
			var v4 = new Vector2(1, 0);
			var v5 = new Vector2(1, 2);
			var v6 = new Vector2(2, 1);

			var a = new Triangle<TestVertexData>(v1, v4, v3);
			var b = new Triangle<TestVertexData>(v1, v2, v3);
			var c = new Triangle<TestVertexData>(v2, v3, v5);
			var d = new Triangle<TestVertexData>(v3, v5, v6);

			var mesh = new Path<TestVertexData>(a, b, c, d);

			var result = new List<Vector2>();
			mesh.FindPath(new Vector2(1.99f, 1.05f), new Vector2(0.02f, 0.33f), ref result);
			Assert.AreEqual(3, result.Count);
		}

		/// <summary>
		/// Any point returned by the GetClosestPoint method can be reached.
		/// </summary>
		[TestMethod]
		public void GetClosestPointDoesNotCrossItsEdge()
		{
			var lb = new Vector2(0, 0);
			var r = new Vector2(1, 0);
			var t = new Vector2(0, 1);

			var start = new Vector2(0.1f, 0.14f);

			var a = new Triangle<TestVertexData>(lb, r, t);
			var mesh = new Path<TestVertexData>(a);

			for (var f = 0f; f <= 1; f += 0.0005f)
			{
				var test1 = new Vector2(f, 1);
				var test2 = new Vector2(1, f);

				var closest1 = a.GetClosestPoint(test1);
				var closest2 = a.GetClosestPoint(test2);

				var res1 = new List<Vector2>();
				var res2 = new List<Vector2>();
				var res3 = new List<Vector2>();
				var res4 = new List<Vector2>();

				mesh.FindPath(start, closest1, ref res3);
				Assert.AreEqual(2, res3.Count);
				mesh.FindPath(start, closest2, ref res4);
				Assert.AreEqual(2, res4.Count);

				mesh.FindPath(start, closest1, ref res1);
				Assert.AreEqual(2, res1.Count);
				mesh.FindPath(start, closest2, ref res2);
				Assert.AreEqual(2, res2.Count);
			}
		}

		/// <summary>
		/// Tests two simple adjacent pointy triangles
		/// </summary>
		[TestMethod]
		public void PathA()
		{
			var left = new Vector2(-100, 0);
			var right = new Vector2(100, 0);
			var top = new Vector2(0, 200);
			var bottom = new Vector2(0, 100);

			var a = new Triangle<TestVertexData>(bottom, top, left);
			var b = new Triangle<TestVertexData>(right, top, bottom);

			var mesh = new Path<TestVertexData>(a, b);

			var result = new List<Vector2>();
			mesh.FindPath(left, right, ref result);
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(left, result[0]);
			Assert.AreEqual(bottom, result[1]);
			Assert.AreEqual(right, result[2]);

			mesh.FindPath(right, left, ref result);
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(left, result[2]);
			Assert.AreEqual(bottom, result[1]);
			Assert.AreEqual(right, result[0]);
		}

		/// <summary>
		/// Tests two pointy triangles with a bridge inbetween
		/// </summary>
		[TestMethod]
		public void PathB()
		{
			var left = new Vector2(-200, 0);
			var right = new Vector2(200, 0);
			var top = new Vector2(0, 200);
			var bottom = new Vector2(0, 100);

			var a = new Triangle<TestVertexData>(left, top - new Vector2(100, 0), bottom - new Vector2(100, 0));
			var b = new Triangle<TestVertexData>(top - new Vector2(100, 0), bottom - new Vector2(100, 0), top + new Vector2(100, 0));
			var c = new Triangle<TestVertexData>(top + new Vector2(100, 0), bottom - new Vector2(100, 0), bottom + new Vector2(100, 0));
			var d = new Triangle<TestVertexData>(top + new Vector2(100, 0), bottom + new Vector2(100, 0), right);

			var mesh = new Path<TestVertexData>(a, b, c, d);

			var result = new List<Vector2>();
			mesh.FindPath(left, right, ref result);
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual(left, result[0]);
			Assert.AreEqual(bottom - new Vector2(100, 0), result[1]);
			Assert.AreEqual(bottom + new Vector2(100, 0), result[2]);
			Assert.AreEqual(right, result[3]);

			mesh.FindPath(right, left, ref result);
			Assert.AreEqual(4, result.Count);
			Assert.AreEqual(left, result[3]);
			Assert.AreEqual(bottom - new Vector2(100, 0), result[2]);
			Assert.AreEqual(bottom + new Vector2(100, 0), result[1]);
			Assert.AreEqual(right, result[0]);

			mesh.FindPath(new Vector2(101, 100), new Vector2(-101, 100), ref result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(new Vector2(-101, 100), result[1]);
			Assert.AreEqual(new Vector2(101, 100), result[0]);

			mesh.FindPath(new Vector2(-101, 100), new Vector2(101, 100), ref result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(new Vector2(101, 100), result[1]);
			Assert.AreEqual(new Vector2(-101, 100), result[0]);

			mesh.FindPath(new Vector2(-100, 100), new Vector2(102, 98), ref result);
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(new Vector2(-100, 100), result[0]);
			Assert.AreEqual(new Vector2(100, 100), result[1]);
			Assert.AreEqual(new Vector2(102, 98), result[2]);
		}

		[TestMethod]
		public void PathBStartOnCommonEdge()
		{
			var left = new Vector2(-200, 0);
			var right = new Vector2(200, 0);
			var top = new Vector2(0, 200);
			var bottom = new Vector2(0, 100);

			var a = new Triangle<TestVertexData>(left, top - new Vector2(100, 0), bottom - new Vector2(100, 0));
			var b = new Triangle<TestVertexData>(top - new Vector2(100, 0), bottom - new Vector2(100, 0), top + new Vector2(100, 0));
			var c = new Triangle<TestVertexData>(top + new Vector2(100, 0), bottom - new Vector2(100, 0), bottom + new Vector2(100, 0));
			var d = new Triangle<TestVertexData>(top + new Vector2(100, 0), bottom + new Vector2(100, 0), right);

			var mesh = new Path<TestVertexData>(a, b, c, d);

			var result = new List<Vector2>();
			mesh.FindPath(new Vector2(-100, 150), right, ref result);

			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(new Vector2(-100, 150), result[0]);
			Assert.AreEqual(bottom + new Vector2(100, 0), result[1]);
			Assert.AreEqual(right, result[2]);
		}

		[TestMethod]
		public void PathC()
		{
			var left = new Vector2(-200, 0);
			var right = new Vector2(200, 0);
			var top = new Vector2(0, 200);
			var bottom = new Vector2(0, 100);

			var a = new Triangle<TestVertexData>(left, top - new Vector2(100, 0), bottom - new Vector2(100, 0));
			var b = new Triangle<TestVertexData>(top - new Vector2(100, 0), bottom - new Vector2(100, 0), top + new Vector2(100, 0));
			var c = new Triangle<TestVertexData>(top + new Vector2(100, 0), bottom - new Vector2(100, 0), bottom + new Vector2(100, 0));
			var d = new Triangle<TestVertexData>(top + new Vector2(100, 0), bottom + new Vector2(100, 0), right);

			var mesh = new Path<TestVertexData>(a, b, c, d);

			var result = new List<Vector2>();
			mesh.FindPath(new Vector2(-129, 97), right, ref result);

			Assert.AreEqual(4, result.Count);
			Assert.AreEqual(new Vector2(-129, 97), result[0]);
			Assert.AreEqual(bottom - new Vector2(100, 0), result[1]);
			Assert.AreEqual(bottom + new Vector2(100, 0), result[2]);
			Assert.AreEqual(right, result[3]);
		}

		/// <summary>
		/// Tests a more complicated mesh
		/// </summary>
		[TestMethod]
		public void PathD()
		{
			var points = new DataVertex<TestVertexData>[14];
			points[0] = new DataVertex<TestVertexData>(new Vector2(0, 0));
			points[1] = new DataVertex<TestVertexData>(new Vector2(100, 0));
			points[2] = new DataVertex<TestVertexData>(new Vector2(0, 100));
			points[3] = new DataVertex<TestVertexData>(new Vector2(100, 110));
			points[4] = new DataVertex<TestVertexData>(new Vector2(50, 150));
			points[5] = new DataVertex<TestVertexData>(new Vector2(250, 250));
			points[6] = new DataVertex<TestVertexData>(new Vector2(500, 500));
			points[7] = new DataVertex<TestVertexData>(new Vector2(250, 0));
			points[8] = new DataVertex<TestVertexData>(new Vector2(500, 0));
			points[9] = new DataVertex<TestVertexData>(new Vector2(600, 100));
			points[10] = new DataVertex<TestVertexData>(new Vector2(700, 0));
			points[11] = new DataVertex<TestVertexData>(new Vector2(900, 400));
			points[12] = new DataVertex<TestVertexData>(new Vector2(602, 400));
			points[13] = new DataVertex<TestVertexData>(new Vector2(300, 800));


			var indices = new int[36];
			indices[0] = 0; indices[1] = 1; indices[2] = 2;
			indices[3] = 1; indices[4] = 2; indices[5] = 3;
			indices[6] = 2; indices[7] = 3; indices[8] = 4;
			indices[9] = 3; indices[10] = 4; indices[11] = 5;
			indices[12] = 4; indices[13] = 5; indices[14] = 6;
			indices[15] = 5; indices[16] = 6; indices[17] = 7;
			indices[18] = 6; indices[19] = 7; indices[20] = 8;
			indices[21] = 6; indices[22] = 8; indices[23] = 9;
			indices[24] = 8; indices[25] = 9; indices[26] = 10;
			indices[27] = 9; indices[28] = 10; indices[29] = 11;
			indices[30] = 9; indices[31] = 12; indices[32] = 11;
			indices[33] = 11; indices[34] = 12; indices[35] = 13;

			var collection = new Mesh<TestVertexData>(points, indices);
			var triangles = collection.Triangles.ToList();

			var mesh = new Path<TestVertexData>(points, indices);

			var result = new List<Vector2>();
			mesh.FindPath(triangles.First().A, triangles.Last().C, ref result);

			Assert.AreEqual(6, result.Count);
			Assert.AreEqual(triangles.First().A, result[0]);
			Assert.AreEqual(points[3].Point, result[1]);
			Assert.AreEqual(points[5].Point, result[2]);
			Assert.AreEqual(points[9].Point, result[3]);
			Assert.AreEqual(points[12].Point, result[4]);
			Assert.AreEqual(triangles.Last().C, result[5]);

			var subList = triangles.GetRange(4, 6);

			mesh.FindPath(subList.First().A, subList.Last().C, ref result);

			Assert.AreEqual(subList.First().A, result[0]);
			Assert.AreEqual(points[5].Point, result[1]);
			Assert.AreEqual(points[9].Point, result[2]);
			Assert.AreEqual(subList.Last().C, result[3]);
		}


		/// <summary>
		/// Tests a more complicated mesh enclosing a hole
		/// </summary>
		[TestMethod]
		public void PathE()
		{
			var points = new DataVertex<TestVertexData>[9 + 5 + 1];
			points[0] = new DataVertex<TestVertexData>(new Vector2(0, 0));
			points[1] = new DataVertex<TestVertexData>(new Vector2(100, 0));
			points[2] = new DataVertex<TestVertexData>(new Vector2(0, 100));
			points[3] = new DataVertex<TestVertexData>(new Vector2(100, 110));
			points[4] = new DataVertex<TestVertexData>(new Vector2(50, 150));
			points[5] = new DataVertex<TestVertexData>(new Vector2(250, 250));
			points[6] = new DataVertex<TestVertexData>(new Vector2(500, 500));
			points[7] = new DataVertex<TestVertexData>(new Vector2(250, 0));
			points[8] = new DataVertex<TestVertexData>(new Vector2(500, 0));
			points[9] = new DataVertex<TestVertexData>(new Vector2(600, 100));
			points[10] = new DataVertex<TestVertexData>(new Vector2(700, 0));
			points[11] = new DataVertex<TestVertexData>(new Vector2(900, 400));
			points[12] = new DataVertex<TestVertexData>(new Vector2(602, 400));
			points[13] = new DataVertex<TestVertexData>(new Vector2(700, 700));
			points[14] = new DataVertex<TestVertexData>(new Vector2(500, 700));


			var indices = new int[45];
			indices[0] = 0; indices[1] = 1; indices[2] = 2;
			indices[3] = 1; indices[4] = 2; indices[5] = 3;
			indices[6] = 2; indices[7] = 3; indices[8] = 4;
			indices[9] = 3; indices[10] = 4; indices[11] = 5;
			indices[12] = 4; indices[13] = 5; indices[14] = 6;
			indices[15] = 5; indices[16] = 6; indices[17] = 7;
			indices[18] = 6; indices[19] = 7; indices[20] = 8;
			indices[21] = 6; indices[22] = 8; indices[23] = 9;
			indices[24] = 8; indices[25] = 9; indices[26] = 10;
			indices[27] = 9; indices[28] = 10; indices[29] = 11;
			indices[30] = 9; indices[31] = 12; indices[32] = 11;
			indices[33] = 11; indices[34] = 12; indices[35] = 13;
			indices[36] = 6; indices[37] = 14; indices[38] = 4;
			indices[39] = 14; indices[40] = 6; indices[41] = 13;
			indices[42] = 12; indices[43] = 6; indices[44] = 13;

			var mesh = new Path<TestVertexData>(points, indices);
			var result = new List<Vector2>();

			mesh.FindPath(new Vector2(68, 159), new Vector2(509, 434), ref result);
			Assert.AreEqual(2, result.Count);

			mesh.FindPath(new Vector2(92, 171), new Vector2(509, 434), ref result);
			Assert.AreEqual(2, result.Count);

			mesh.FindPath(new Vector2(130, 190), new Vector2(509, 434), ref result);
			Assert.AreEqual(2, result.Count);

			mesh.FindPath(new Vector2(66, 158), new Vector2(509, 434), ref result);
			Assert.AreEqual(2, result.Count);
		}

		/// <summary>
		/// Performance test.
		/// </summary>
		[TestMethod]
		[TestCategory("performance")]
		public void Performance()
		{
			var points = new DataVertex<TestVertexData>[9 + 5 + 1];
			points[0] = new DataVertex<TestVertexData>(new Vector2(0, 0));
			points[1] = new DataVertex<TestVertexData>(new Vector2(100, 0));
			points[2] = new DataVertex<TestVertexData>(new Vector2(0, 100));
			points[3] = new DataVertex<TestVertexData>(new Vector2(100, 110));
			points[4] = new DataVertex<TestVertexData>(new Vector2(50, 150));
			points[5] = new DataVertex<TestVertexData>(new Vector2(250, 250));
			points[6] = new DataVertex<TestVertexData>(new Vector2(500, 500));
			points[7] = new DataVertex<TestVertexData>(new Vector2(250, 0));
			points[8] = new DataVertex<TestVertexData>(new Vector2(500, 0));
			points[9] = new DataVertex<TestVertexData>(new Vector2(600, 100));
			points[10] = new DataVertex<TestVertexData>(new Vector2(700, 0));
			points[11] = new DataVertex<TestVertexData>(new Vector2(900, 400));
			points[12] = new DataVertex<TestVertexData>(new Vector2(602, 400));
			points[13] = new DataVertex<TestVertexData>(new Vector2(700, 700));
			points[14] = new DataVertex<TestVertexData>(new Vector2(500, 700));


			var indices = new int[45];
			indices[0] = 0; indices[1] = 1; indices[2] = 2;
			indices[3] = 1; indices[4] = 2; indices[5] = 3;
			indices[6] = 2; indices[7] = 3; indices[8] = 4;
			indices[9] = 3; indices[10] = 4; indices[11] = 5;
			indices[12] = 4; indices[13] = 5; indices[14] = 6;
			indices[15] = 5; indices[16] = 6; indices[17] = 7;
			indices[18] = 6; indices[19] = 7; indices[20] = 8;
			indices[21] = 6; indices[22] = 8; indices[23] = 9;
			indices[24] = 8; indices[25] = 9; indices[26] = 10;
			indices[27] = 9; indices[28] = 10; indices[29] = 11;
			indices[30] = 9; indices[31] = 12; indices[32] = 11;
			indices[33] = 11; indices[34] = 12; indices[35] = 13;
			indices[36] = 6; indices[37] = 14; indices[38] = 4;
			indices[39] = 14; indices[40] = 6; indices[41] = 13;
			indices[42] = 12; indices[43] = 6; indices[44] = 13;

			var path = new Path<TestVertexData>();

			var watch = new System.Diagnostics.Stopwatch();
			watch.Start();

			for (var i = 0; i < 100; i++)
			{
				path = new Path<TestVertexData>(points, indices);
			}

			watch.Stop();
			System.Diagnostics.Debug.WriteLine("Constructor: " + watch.Elapsed);
			watch.Restart();

			for (var i = 0; i < 100; i++)
			{
				foreach (var pointA in points)
				{
					foreach (var pointB in points)
					{
						if (pointB != pointA)
						{
							var result = new List<Vector2>();
							path.FindPath(pointA, pointB, ref result);
							Assert.IsFalse(result.Count == 0);
						}
					}
				}
			}

			watch.Stop();
			System.Diagnostics.Debug.WriteLine("Searching: " + watch.Elapsed);
		}

		[TestMethod]
		public void PathVertexDataInterpolation()
		{
			var points = new DataVertex<TestVertexData>[3];
			points[0] = new DataVertex<TestVertexData>(new Vector2(0, 0), new TestVertexData(0));
			points[1] = new DataVertex<TestVertexData>(new Vector2(0, 100), new TestVertexData(1));
			points[2] = new DataVertex<TestVertexData>(new Vector2(100, 0), new TestVertexData(2));

			var indices = new int[3];
			indices[0] = 0; indices[1] = 1; indices[2] = 2;

			var mesh = new Path<TestVertexData>(points, indices);

			Assert.AreEqual(mesh.GetVertexData(new Vector2(100f / 3f, 100 / 3f)).Value, 1f, .0005f);
		}

		/// <summary>
		/// Tests a triangle made up of three smaller triangles.
		/// </summary>
		[TestMethod]
		public void ContiguousTriangles()
		{
			var left = new Vector2(-100, 0);
			var right = new Vector2(100, 0);
			var top = new Vector2(0, 200);
			var common = new Vector2(0, 100);

			var a = new Triangle<TestVertexData>(common, top, left);
			var b = new Triangle<TestVertexData>(right, top, common);
			var c = new Triangle<TestVertexData>(left, common, right);

			var path = new Path<TestVertexData>(a, b, c);

			var result = new List<Vector2>();
			path.FindPath(left, right, ref result);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(left, result[0]);
			Assert.AreEqual(right, result[1]);

			path.FindPath(right, left, ref result);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(left, result[1]);
			Assert.AreEqual(right, result[0]);
		}

	}
}
