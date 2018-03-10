using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK;
using Microsoft.Xna.Framework.Graphics;

namespace STACK.Functional.Test
{
    [TestClass]
    public class ContentManagement
    {

        [TestMethod, TestCategory("GPU")]        
        public void SceneContentDisposed()
        {
			Texture2D SharedTexture;
			using (var GraphicsDevice = Mock.CreateGraphicsDevice())
			using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), null))
			{
				SharedTexture = Runner.EngineContent.Load<Texture2D>("stacklogo");
				var Scene = Runner.Game.World["1"];
				var SceneTexture = Scene.Content.Load<Texture2D>("stacklogo");

				Scene.UnloadContent();
				Assert.IsTrue(SceneTexture.IsDisposed);
				Assert.IsFalse(SharedTexture.IsDisposed);

			}
            Assert.IsTrue(SharedTexture.IsDisposed);
        }      
    }
}
