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
#pragma warning disable CS1718 // Vergleich erfolgte mit derselben Variable
			Assert.IsTrue(Projection2D.Zero == Projection2D.Zero);
#pragma warning restore CS1718 // Vergleich erfolgte mit derselben Variable			
		}

		[TestMethod]
		public void TransformationInverse()
		{
			const float half = 0.5f;

			var transformation = new Projection2D().SetQuadliteral(new Vector2(346, 517), new Vector2(1180, 515), new Vector2(1280, 720), new Vector2(0, 720));

			var transformed = transformation.Transform(new Vector2(half, half));
			var inverse = transformation.TransformInverse(transformed);

			Assert.IsTrue(Math.Abs(inverse.X - half) < 0.001f);
			Assert.IsTrue(Math.Abs(inverse.Y - half) < 0.001f);
		}

		[TestMethod]
		public void UnitTransformation()
		{
			var a = new Vector2(0, 0);
			var b = new Vector2(1, 0);
			var c = new Vector2(1, 1);
			var d = new Vector2(0, 1);

			var transformation = new Projection2D().SetQuadliteral(a, b, c, d);

			foreach (var vector in new Vector2[] { Vector2.Zero, Vector2.UnitY, Vector2.UnitX, new Vector2(1, 1) })
			{
				var transformed = transformation.Transform(vector);
				Assert.AreEqual(vector, transformed);
			}
		}

		[TestMethod]
		public void Transformation()
		{
			Vector2 transformed, inverse;

			var a = new Vector2(0, 0);
			var b = new Vector2(3, 0);
			var c = new Vector2(3, 1);
			var d = new Vector2(1, 1);

			var expected = new Vector2[4] { a, d, b, c };
			var transformation = new Projection2D().SetQuadliteral(a, b, c, d);
			var i = 0;

			foreach (var vector in new Vector2[] { Vector2.Zero, Vector2.UnitY, Vector2.UnitX, new Vector2(1, 1) })
			{
				transformed = transformation.Transform(vector);
				Assert.AreEqual(expected[i++], transformed);
				inverse = transformation.TransformInverse(transformed);
				Assert.AreEqual(vector, inverse);
			}
		}
	}
}
