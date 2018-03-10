using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK;
using STACK.Components;
using System.Collections;

namespace STACK.Test
{
    [TestClass]
    public class PassagesTest
    {

        class ExitEntity : Entity
        {
            public ExitEntity()
            {
                Exit
                    .Create(this)
                    .SetTargetEntrance("STACK.Test.PassagesTest+EntranceEntity");
            }
        }

        class EntranceEntity : Entity
        {
            public EntranceEntity()
            {
                Entrance
                    .Create(this)
                    .SetScript(Enter);
            }

            public IEnumerator Enter(Entity player)
            {
                player.Get<Transform>().SetPosition(10, 10);
                yield return 0;
            }
        }

        class Player : Entity
        {
            public Player()
            {
                Transform
                    .Create(this);

                Scripts
                    .Create(this);
            }
        }

        class SkipContent : ISkipContent
        {
            public SkipCutscene SkipCutscene => null;
            public SkipText SkipText => null;
        }

        /// <summary>
        /// Asserts the entrance script is executed immediately after changing the scene.        
        /// </summary>
        [TestMethod]
        public void PassingSetsDrawScene()
        {            
            var World = new World(WorldTest.ServiceProvider);
            var ExitScene = new Scene("s1") { Enabled = true, Visible = true };
            var EntranceScene = new Scene("s2") { Enabled = true, Visible = true };
            var ExitEntity = new ExitEntity();
            var Player = new Player();

            ExitScene.Push(ExitEntity);
            ExitScene.Push(Player);
            EntranceScene.Push(new EntranceEntity());

            World.Push(ExitScene);
            World.Push(EntranceScene);

            World.Initialize(false);

            ExitEntity.Get<Exit>().Use(Player);
            World.Update();

            Assert.AreEqual(EntranceScene, Player.DrawScene);
            Assert.AreEqual(new Vector2(10, 10), Player.Get<Transform>().Position);
        }


        Entity CreateEntity()
        {
            var Entity = new Entity();

            Scripts.Create(Entity);
            Transform.Create(Entity);
            var TextComponent = Text.Create(Entity);
            TextComponent.MeasureStringFn = MeasureString;

            var Scene = new Scene("s1");
            Scene.Push(Entity);
            var World = new World(WorldTest.ServiceProvider);

            World.Push(Scene);

            return Entity;
        }

        public Vector2 MeasureString(string text)
        {
            var Result = Vector2.Zero;

            Result.X = text.Length * 10;
            Result.Y = 20;

            return Result;
        }

        [TestMethod]
        public void FinishedTalkingSetsState()
        {
            var Entity = CreateEntity();

            var Script = Entity.Get<Scripts>().Say("text");

            int i = 0;
            while (!Script.Done && i <= 1000)
            {

                Entity.Update();
                i++;
            }

            Assert.AreEqual(Components.State.Idle, Entity.Get<Transform>().State);
        }

        [TestMethod]
        public void FinishedTalkingSetsStateWhileWalking()
        {
            var Entity = CreateEntity();
            var Scripts = Entity.Get<Scripts>();
            var Transform = Entity.Get<Transform>();

            var WalkingScript = Scripts.GoTo(1000000000, 0);

            Entity.Update();

            Assert.IsTrue(Transform.State.Has(Components.State.Walking));

            var Script = Scripts.Say("text", GameSpeed.TickDuration);

            Assert.IsTrue(Transform.State.Has(Components.State.Talking));

            Entity.Update();

            Assert.IsTrue(Script.Done);
            Assert.IsFalse(Transform.State.Has(Components.State.Talking));
            Assert.IsTrue(Transform.State.Has(Components.State.Walking));
        }
    }
}
