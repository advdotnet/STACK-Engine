using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.Graphics;
using STACK.Input;
using System;
using System.Linq;
using System.Reflection;

namespace STACK.Test
{
	[TestClass]
	public class ComponentsTest
	{
		[TestMethod]
		public void AddComponent()
		{
			var test = new Entity();
			test.Add<Transform>();
			Assert.IsNotNull(test.Get<Transform>());
		}

		[TestMethod]
		public void AddComponentSetsEntity()
		{
			var test = new Entity();
			var transform = test.Add<Transform>();
			Assert.AreEqual(test, transform.Entity);
		}

		[TestMethod]
		public void RemoveComponent()
		{
			var test = new Entity();
			Assert.IsNull(test.Get<Transform>());
			test.Add<Transform>();
			Assert.AreEqual(0, test.Items.Count);
			Assert.AreEqual(1, test.Components.Count);
			test.Remove<Transform>();
			Assert.IsNull(test.Get<Transform>());
			Assert.AreEqual(0, test.Items.Count);
			Assert.AreEqual(0, test.Components.Count);
		}

		[TestMethod]
		public void ModifyComponent()
		{
			var test = new Entity();
			test.Add<Transform>();
			test.Get<Transform>().Z = 7;
			Assert.AreEqual(7, test.Get<Transform>().Z);
		}

		[TestMethod]
		public void RectangleHotspotAsHotspot()
		{
			var test = new Entity();
			test.Add<HotspotRectangle>();

			Assert.IsNotNull(test.Get<Hotspot>());
		}

		[TestMethod]
		public void GetsInterface()
		{
			var test = new Entity();
			var spriteCustomAnimation = test.Add<SpriteCustomAnimation>();
			var gotInterface = test.GetInterface<IPlayAnimation>();

			Assert.AreEqual(gotInterface, spriteCustomAnimation);
		}

		[TestMethod]
		public void NotifyDoesNotThrowWithoutEntity()
		{
			new Transform().Position = new Vector2(1, 1);
		}

		[TestMethod]
		public void NoNotificationBeforeInitialization()
		{
			var test = new Entity();
			test.Add<SpriteData>();
			test.Notify(Messages.ColorChanged, Color.Aqua);

			Assert.AreNotEqual(Color.Aqua, test.Get<SpriteData>().Color);
		}

		[TestMethod]
		public void NotificationAfterInitialization()
		{
			var test = new Entity();
			test.Add<SpriteData>();

			test.Initialize(false);

			test.Notify(Messages.ColorChanged, Color.Aqua);

			Assert.AreEqual(Color.Aqua, test.Get<SpriteData>().Color);
		}

		private class CustomComponent : Component, IUpdate, IDraw, IInitialize, IContent, IInteractive, INotify
		{
			public bool Enabled { get; set; }
			public float UpdateOrder { get; set; }
			public bool UpdateCalled { get; private set; }
			public void Update()
			{
				UpdateCalled = true;
			}

			public bool Visible { get; set; }
			public float DrawOrder { get; set; }
			public bool DrawCalled { get; private set; }

			public void Draw(Renderer renderer)
			{
				DrawCalled = true;
			}

			public bool InitializeCalled { get; private set; }
			public void Initialize(bool restore)
			{
				InitializeCalled = true;
			}

			public bool LoadContentCalled { get; private set; }
			public void LoadContent(ContentLoader content)
			{
				LoadContentCalled = true;
			}

			public bool UnloadContentCalled { get; private set; }
			public void UnloadContent()
			{
				UnloadContentCalled = true;
			}

			public bool HandleInputEventCalled { get; private set; }
			public void HandleInputEvent(Vector2 mouse, InputEvent inputEvent)
			{
				HandleInputEventCalled = true;
			}

			public bool NotifyCalled { get; private set; }
			public void Notify<T>(string message, T data)
			{
				NotifyCalled = true;
			}
		}

		[TestMethod]
		public void ComponentUpdateTest()
		{
			var test = new Entity();
			var component = test.Add<CustomComponent>();

			component.Enabled = false;
			test.Update();
			Assert.IsFalse(component.UpdateCalled);

			component.Enabled = true;
			test.Update();
			Assert.IsTrue(component.UpdateCalled);
		}

		[TestMethod]
		public void ComponentDrawTest()
		{
			var test = new Entity();
			var component = test.Add<CustomComponent>();

			component.Visible = false;
			test.Draw(null);
			Assert.IsFalse(component.DrawCalled);

			component.Visible = true;
			test.Draw(null);
			Assert.IsTrue(component.DrawCalled);
		}

		[TestMethod]
		public void ComponentInitializeTest()
		{
			var test = new Entity();
			var component = test.Add<CustomComponent>();

			test.Initialize(false);
			Assert.IsTrue(component.InitializeCalled);
		}

		[TestMethod]
		public void ComponentContentTest()
		{
			var test = new Entity();
			var component = test.Add<CustomComponent>();

			test.LoadContent(null);
			Assert.IsTrue(component.LoadContentCalled);

			test.UnloadContent();
			Assert.IsTrue(component.UnloadContentCalled);
		}

		[TestMethod]
		public void ComponentInteractiveTest()
		{
			var test = new Entity();
			var component = test.Add<CustomComponent>();

			test.HandleInputEvent(default, default);
			Assert.IsTrue(component.HandleInputEventCalled);
		}

		[TestMethod]
		public void ComponentNotifyTest()
		{
			var test = new Entity();
			var component = test.Add<CustomComponent>();
			test.Initialize(false);
			test.Notify(default, 1);
			Assert.IsTrue(component.NotifyCalled);
		}

		[TestMethod]
		public void IUpdateComponentEnabledAfterConstructing()
		{
			var q = from t in Assembly.GetAssembly(typeof(Component)).GetTypes()
					where t.IsClass && typeof(IUpdate).IsAssignableFrom(t) &&
						typeof(Component).IsAssignableFrom(t) &&
						!t.IsAbstract
					select t;

			foreach (var componentType in q)
			{
				var entity = new Entity();
				var instance = (IUpdate)Activator.CreateInstance(componentType);
				Assert.IsTrue(instance.Enabled, componentType.ToString() + " not default enabled.");
			}
		}

		[TestMethod]
		public void IDrawComponentVisibleAfterConstructing()
		{
			var q = from t in Assembly.GetAssembly(typeof(Component)).GetTypes()
					where t.IsClass && typeof(IDraw).IsAssignableFrom(t) &&
						typeof(Component).IsAssignableFrom(t) &&
						!t.IsAbstract
					select t;

			foreach (var componentType in q)
			{
				var entity = new Entity();
				var instance = (IDraw)Activator.CreateInstance(componentType);
				Assert.IsTrue(instance.Visible, $"{componentType} not default enabled.");
			}
		}
	}
}