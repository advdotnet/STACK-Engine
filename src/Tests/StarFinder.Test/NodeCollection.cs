using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace StarFinder.Test
{
	[TestClass]
	public class NodeCollectionTests
	{
		private readonly Vertex _vertex1 = new Vertex(2, 7);
		private readonly Vertex _vertex2 = new Vertex(3, 5);
		private readonly Vertex _vertex3 = new Vertex(1, 6);
		private readonly Vertex _vertex4 = new Vertex(4, 4);

		[TestMethod]
		public void GetsLinkPositive()
		{
			var nodeCollection = new NodeCollection();
			nodeCollection.Add(_vertex1);
			nodeCollection.Add(_vertex2);
			nodeCollection.CalculateStaticLinks(Return(true));
			var result = nodeCollection.GetLinks(_vertex1).First();

			Assert.AreEqual(_vertex2, result);
		}

		[TestMethod]
		public void GetsLinkNegative()
		{
			var nodeCollection = new NodeCollection();
			nodeCollection.Add(_vertex1);
			nodeCollection.Add(_vertex2);
			nodeCollection.CalculateStaticLinks(Return(false));
			var count = nodeCollection.GetLinks(_vertex1).Count();

			Assert.AreEqual(0, count);
		}

		[TestMethod]
		public void ClearsDynamicLinks()
		{
			var nodeCollection = new NodeCollection();
			nodeCollection.Add(_vertex1);
			nodeCollection.CalculateDynamicLinks(_vertex2, _vertex3, Return(true));
			nodeCollection.CalculateDynamicLinks(_vertex3, _vertex4, Return(true));

			var count = nodeCollection.GetLinks(_vertex1).Count(); // Vertex3, Vertex4

			Assert.AreEqual(2, count);
		}

		private Func<Vector2, Vector2, bool> Return(bool result) => (v1, v2) => result;
	}
}
