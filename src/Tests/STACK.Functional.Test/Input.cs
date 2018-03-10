using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using STACK.Components;

namespace STACK.Functional.Test
{
    [TestClass]
    public class Input
    {

        public class TestGame : StackGame
        {
            class TestScene : Scene
            {
                public TestScene()
                {
                    InputDispatcher
                        .Create(this)
                        .SetOnMouseUpFn(OnMouseUp);
                }

                public void OnMouseUp(Vector2 position, MouseButton button)
                {
                    throw new Exception();                    
                }
            }

            protected override List<Scene> GetScenes()
            {
                return new List<Scene> { new TestScene() { Enabled = true } };
            }

            protected override void OnStart()
            {
                this.StartWorld();
            }
        }

        [TestMethod, TestCategory("GPU")]        
        public void InputNotSendToWorldWhenPaused()
        {
			using (var GraphicsDevice = Mock.CreateGraphicsDevice())
			using (var Runner = new TestEngine(new TestGame(), Mock.Wrap(GraphicsDevice), Mock.Input))
			{
				Runner.Pause();
				Runner.MouseClick(10, 10);
			}
        }      
    }
}
