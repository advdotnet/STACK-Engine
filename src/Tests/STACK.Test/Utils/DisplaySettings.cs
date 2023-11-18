using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Graphics;

namespace STACK.Test
{
	[TestClass]
	public class DisplaySettingsTests
	{
		[TestMethod]
		public void DisplaySettingsNoTargetResolutionTest()
		{
			var settings = new DisplaySettings(new Point(640, 400), new Point(1280, 800), null);

			Assert.AreEqual(1280, settings.Viewport.Width);
			Assert.AreEqual(800, settings.Viewport.Height);
			Assert.AreEqual(2, settings.ScaleMatrix.M11);
			Assert.AreEqual(2, settings.ScaleMatrix.M22);

			settings.OnClientSizeChanged(640, 500);

			Assert.AreEqual(0, settings.Viewport.X);
			Assert.AreEqual(50, settings.Viewport.Y);
			Assert.AreEqual(640, settings.Viewport.Width);
			Assert.AreEqual(400, settings.Viewport.Height);
		}

		[TestMethod]
		public void DisplaySettingsTargetResolutionTest()
		{
			var settings = new DisplaySettings(new Point(640, 400), new Point(1280, 800), new Point(640, 400));

			Assert.AreEqual(320, settings.Viewport.X);
			Assert.AreEqual(200, settings.Viewport.Y);
			Assert.AreEqual(640, settings.Viewport.Width);
			Assert.AreEqual(400, settings.Viewport.Height);
		}
	}
}
