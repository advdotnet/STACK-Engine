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
            Entity Test = new Entity();
            Test.Add<Transform>();
            Assert.IsNotNull(Test.Get<Transform>());
        }

        [TestMethod]
        public void AddComponentSetsEntity()
        {
            Entity Test = new Entity();
            var Transform = Test.Add<Transform>();
            Assert.AreEqual(Test, Transform.Entity);
        }

        [TestMethod]
        public void RemoveComponent()
        {
            Entity Test = new Entity();
            Assert.IsNull(Test.Get<Transform>());
            Test.Add<Transform>();
            Assert.AreEqual(0, Test.Items.Count);
            Assert.AreEqual(1, Test.Components.Count);
            Test.Remove<Transform>();
            Assert.IsNull(Test.Get<Transform>());
            Assert.AreEqual(0, Test.Items.Count);
            Assert.AreEqual(0, Test.Components.Count);
        }

        [TestMethod]
        public void ModifyComponent()
        {
            Entity Test = new Entity();
            Test.Add<Transform>();
            Test.Get<Transform>().Z = 7;
            Assert.AreEqual(7, Test.Get<Transform>().Z);
        }

        [TestMethod]
        public void RectangleHotspotAsHotspot()
        {
            Entity Test = new Entity();
            Test.Add<HotspotRectangle>();

            Assert.IsNotNull(Test.Get<Hotspot>());
        }

        [TestMethod]
        public void GetsInterface()
        {
            var Test = new Entity();
            var SpriteCustomAnimation = Test.Add<SpriteCustomAnimation>();
            var GotInterface = Test.GetInterface<IPlayAnimation>();

            Assert.AreEqual(GotInterface, SpriteCustomAnimation);
        }

        [TestMethod]
        public void NotifyDoesNotThrowWithoutEntity()
        {
            new Transform().Position = new Vector2(1, 1);
        }

        [TestMethod]
        public void NoNotificationBeforeInitialization()
        {
            Entity Test = new Entity();
            Test.Add<SpriteData>();
            Test.Notify(Messages.ColorChanged, Color.Aqua);

            Assert.AreNotEqual(Color.Aqua, Test.Get<SpriteData>().Color);
        }

        [TestMethod]
        public void NotificationAfterInitialization()
        {
            Entity Test = new Entity();
            Test.Add<SpriteData>();

            Test.Initialize(false);

            Test.Notify(Messages.ColorChanged, Color.Aqua);

            Assert.AreEqual(Color.Aqua, Test.Get<SpriteData>().Color);
        }

        class CustomComponent : Component, IUpdate, IDraw, IInitialize, IContent, IInteractive, INotify
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
            Entity Test = new Entity();
            var Component = Test.Add<CustomComponent>();

            Component.Enabled = false;
            Test.Update();
            Assert.IsFalse(Component.UpdateCalled);

            Component.Enabled = true;
            Test.Update();
            Assert.IsTrue(Component.UpdateCalled);
        }

        [TestMethod]
        public void ComponentDrawTest()
        {
            Entity Test = new Entity();
            var Component = Test.Add<CustomComponent>();

            Component.Visible = false;
            Test.Draw(null);
            Assert.IsFalse(Component.DrawCalled);

            Component.Visible = true;
            Test.Draw(null);
            Assert.IsTrue(Component.DrawCalled);
        }

        [TestMethod]
        public void ComponentInitializeTest()
        {
            Entity Test = new Entity();
            var Component = Test.Add<CustomComponent>();

            Test.Initialize(false);
            Assert.IsTrue(Component.InitializeCalled);
        }

        [TestMethod]
        public void ComponentContentTest()
        {
            Entity Test = new Entity();
            var Component = Test.Add<CustomComponent>();

            Test.LoadContent(null);
            Assert.IsTrue(Component.LoadContentCalled);

            Test.UnloadContent();
            Assert.IsTrue(Component.UnloadContentCalled);
        }

        [TestMethod]
        public void ComponentInteractiveTest()
        {
            Entity Test = new Entity();
            var Component = Test.Add<CustomComponent>();

            Test.HandleInputEvent(default(Vector2), default(InputEvent));
            Assert.IsTrue(Component.HandleInputEventCalled);
        }

        [TestMethod]
        public void ComponentNotifyTest()
        {
            Entity Test = new Entity();
            var Component = Test.Add<CustomComponent>();
            Test.Initialize(false);
            Test.Notify<int>(default(string), 1);
            Assert.IsTrue(Component.NotifyCalled);
        }

        [TestMethod]
        public void IUpdateComponentEnabledAfterConstructing()
        {
            var q = from t in Assembly.GetAssembly(typeof(Component)).GetTypes()
                    where t.IsClass && typeof(IUpdate).IsAssignableFrom(t) &&
                        typeof(Component).IsAssignableFrom(t) &&
                        !t.IsAbstract
                    select t;

            foreach (var ComponentType in q)
            {
                var Entity = new Entity();
                var Instance = (IUpdate)Activator.CreateInstance(ComponentType);
                Assert.IsTrue(Instance.Enabled, ComponentType.ToString() + " not default enabled.");
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

            foreach (var ComponentType in q)
            {
                var Entity = new Entity();
                var Instance = (IDraw)Activator.CreateInstance(ComponentType);
                Assert.IsTrue(Instance.Visible, ComponentType.ToString() + " not default enabled.");
            }
        }
    }
}