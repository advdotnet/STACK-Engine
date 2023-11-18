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
		private class ExitEntity : Entity
		{
			public ExitEntity()
			{
				Exit
					.Create(this)
					.SetTargetEntrance("STACK.Test.PassagesTest+EntranceEntity");
			}
		}

		private class EntranceEntity : Entity
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

		private class Player : Entity
		{
			public Player()
			{
				Transform
					.Create(this);

				Scripts
					.Create(this);
			}
		}

		/// <summary>
		/// Asserts the entrance script is executed immediately after changing the scene.        
		/// </summary>
		[TestMethod]
		public void PassingSetsDrawScene()
		{
			var world = new World(WorldTest.ServiceProvider);
			var exitScene = new Scene("s1") { Enabled = true, Visible = true };
			var entranceScene = new Scene("s2") { Enabled = true, Visible = true };
			var exitEntity = new ExitEntity();
			var player = new Player();

			exitScene.Push(exitEntity);
			exitScene.Push(player);
			entranceScene.Push(new EntranceEntity());

			world.Push(exitScene);
			world.Push(entranceScene);

			world.Initialize(false);

			exitEntity.Get<Exit>().Use(player);
			world.Update();

			Assert.AreEqual(entranceScene, player.DrawScene);
			Assert.AreEqual(new Vector2(10, 10), player.Get<Transform>().Position);
		}

		private Entity CreateEntity()
		{
			var entity = new Entity();

			Scripts.Create(entity);
			Transform.Create(entity);
			var textComponent = Text.Create(entity);
			textComponent.MeasureStringFn = MeasureString;

			var scene = new Scene("s1");
			scene.Push(entity);
			var world = new World(WorldTest.ServiceProvider);

			world.Push(scene);

			return entity;
		}

		public Vector2 MeasureString(string text)
		{
			var result = Vector2.Zero;

			result.X = text.Length * 10;
			result.Y = 20;

			return result;
		}

		[TestMethod]
		public void FinishedTalkingSetsState()
		{
			var entity = CreateEntity();

			var script = entity.Get<Scripts>().Say("text");

			var i = 0;
			while (!script.Done && i <= 1000)
			{

				entity.Update();
				i++;
			}

			Assert.AreEqual(Components.State.Idle, entity.Get<Transform>().State);
		}

		[TestMethod]
		public void FinishedTalkingSetsStateWhileWalking()
		{
			var entity = CreateEntity();
			var scripts = entity.Get<Scripts>();
			var transform = entity.Get<Transform>();
			_ = scripts.GoTo(1000000000, 0);

			entity.Update();

			Assert.IsTrue(transform.State.Has(Components.State.Walking));

			var script = scripts.Say("text", GameSpeed.TickDuration);

			Assert.IsTrue(transform.State.Has(Components.State.Talking));

			entity.Update();

			Assert.IsTrue(script.Done);
			Assert.IsFalse(transform.State.Has(Components.State.Talking));
			Assert.IsTrue(transform.State.Has(Components.State.Walking));
		}
	}
}
