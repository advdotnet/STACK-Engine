using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace StarFinder.Test
{
    [TestClass]
    public class NodeCollectionTests
    {
        Vertex Vertex1 = new Vertex(2, 7);
        Vertex Vertex2 = new Vertex(3, 5);
        Vertex Vertex3 = new Vertex(1, 6);
        Vertex Vertex4 = new Vertex(4, 4);

        [TestMethod]
        public void GetsLinkPositive()
        {
            var NodeCollection = new NodeCollection();
            NodeCollection.Add(Vertex1);
            NodeCollection.Add(Vertex2);
            NodeCollection.CalculateStaticLinks(Return(true));
            var Result = NodeCollection.GetLinks(Vertex1).First();

            Assert.AreEqual(Vertex2, Result);
        }

        [TestMethod]
        public void GetsLinkNegative()
        {
            var NodeCollection = new NodeCollection();
            NodeCollection.Add(Vertex1);
            NodeCollection.Add(Vertex2);
            NodeCollection.CalculateStaticLinks(Return(false));
            var Count = NodeCollection.GetLinks(Vertex1).Count();

            Assert.AreEqual(0, Count);
        }

        [TestMethod]
        public void ClearsDynamicLinks()
        {
            var NodeCollection = new NodeCollection();
            NodeCollection.Add(Vertex1);
            NodeCollection.CalculateDynamicLinks(Vertex2, Vertex3, Return(true));
            NodeCollection.CalculateDynamicLinks(Vertex3, Vertex4, Return(true));

            var Count = NodeCollection.GetLinks(Vertex1).Count(); // Vertex3, Vertex4

            Assert.AreEqual(2, Count);
        }

        private Func<Vector2, Vector2, bool> Return(bool result)
        {
            return (v1, v2) => result;
        }
    }
}
