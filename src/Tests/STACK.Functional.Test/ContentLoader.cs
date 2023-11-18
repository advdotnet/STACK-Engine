using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Graphics;
using STACK.TestBase;

namespace STACK.Functional.Test
{
	[TestClass]
	public class ContentManagement
	{
		[TestMethod, TestCategory("GPU")]
		public void SceneContentDisposed()
		{
			Texture2D sharedTexture;
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), null))
			{
				sharedTexture = runner.EngineContent.Load<Texture2D>("stacklogo");
				var scene = runner.Game.World["1"];
				var sceneTexture = scene.Content.Load<Texture2D>("stacklogo");

				scene.UnloadContent();
				Assert.IsTrue(sceneTexture.IsDisposed);
				Assert.IsFalse(sharedTexture.IsDisposed);

			}
			Assert.IsTrue(sharedTexture.IsDisposed);
		}
	}
}
