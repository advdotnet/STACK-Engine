using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using System;

namespace STACK.Test
{
    [TestClass]
    public class Projection2DTests
    {
        [TestMethod]
        public void EqualityTest()
        {
            Assert.IsTrue(Projection2D.Zero.Equals(Projection2D.Zero));
            Assert.IsTrue(Projection2D.Zero == Projection2D.Zero);
            var Test = Projection2D.Zero.GetHashCode();
        }

        [TestMethod]
        public void TransformationInverse()
        {
            const float half = 0.5f;

            var Transformation = new Projection2D().SetQuadliteral(new Vector2(346, 517), new Vector2(1180, 515), new Vector2(1280, 720), new Vector2(0, 720));

            var Transformed = Transformation.Transform(new Vector2(half, half));
            var Inverse = Transformation.TransformInverse(Transformed);

            Assert.IsTrue(Math.Abs(Inverse.X - half) < 0.001f);
            Assert.IsTrue(Math.Abs(Inverse.Y - half) < 0.001f);
        }

        [TestMethod]
        public void UnitTransformation()
        {
            var A = new Vector2(0, 0);
            var B = new Vector2(1, 0);
            var C = new Vector2(1, 1);
            var D = new Vector2(0, 1);

            var Transformation = new Projection2D().SetQuadliteral(A, B, C, D);

            foreach (var Vector in new Vector2[] { Vector2.Zero, Vector2.UnitY, Vector2.UnitX, new Vector2(1, 1) })
            {
                var Transformed = Transformation.Transform(Vector);
                Assert.AreEqual(Vector, Transformed);
            }
        }

        [TestMethod]
        public void Transformation()
        {
            Vector2 Transformed, Inverse;

            var A = new Vector2(0, 0);
            var B = new Vector2(3, 0);
            var C = new Vector2(3, 1);
            var D = new Vector2(1, 1);

            Vector2[] Expected = new Vector2[4] { A, D, B, C };
            var Transformation = new Projection2D().SetQuadliteral(A, B, C, D);
            var i = 0;

            foreach (var Vector in new Vector2[] { Vector2.Zero, Vector2.UnitY, Vector2.UnitX, new Vector2(1, 1) })
            {
                Transformed = Transformation.Transform(Vector);
                Assert.AreEqual(Expected[i++], Transformed);
                Inverse = Transformation.TransformInverse(Transformed);
                Assert.AreEqual(Vector, Inverse);
            }
        }
    }
}
