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
			var v1 = new Vector2(0, 0);
			var v2 = new Vector2(0.33f, 1);
			var v3 = new Vector2(1, 1);
			var v4 = new Vector2(1, 0);
			var v5 = new Vector2(1, 2);
			var v6 = new Vector2(2, 1);

			var a = new Triangle<TriangleVertexData>(v1, v4, v3);
			var b = new Triangle<TriangleVertexData>(v1, v2, v3);
			var c = new Triangle<TriangleVertexData>(v2, v3, v5);
			var d = new Triangle<TriangleVertexData>(v3, v5, v6);

			var path = new Path(a, b, c, d);

			var check = State.Serialization.SaveState(path);
			File.WriteAllBytes("pathstate.txt", check);

			Console.Write(check.Length + " bytes.");

			var newPath = State.Serialization.LoadState<Path>(check);
			Assert.AreEqual(2, newPath.Mesh.MaxX);
			Assert.AreEqual(0, newPath.Mesh.MinX);
			Assert.IsTrue(check.Length < 4089);
			Assert.AreEqual(4, newPath.Mesh.Triangles.Length);
			var wayPoints = new List<Vector2>();
			newPath.FindPath(v1, v6, ref wayPoints);
		}
	}
}
