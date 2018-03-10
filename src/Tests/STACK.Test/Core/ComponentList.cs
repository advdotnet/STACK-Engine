using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.Graphics;
using STACK.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace STACK.Test
{
    [TestClass]
    public class ComponentListTest
    {
        [Serializable]
        class CustomComponent : Component, IUpdate, IDraw, IInitialize, IContent, IInteractive, INotify
        {
            public float DrawOrder
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public bool Enabled
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public float UpdateOrder
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public bool Visible
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public void Draw(Renderer renderer)
            {
                throw new NotImplementedException();
            }

            public void HandleInputEvent(Vector2 mouse, InputEvent inputEvent)
            {
                throw new NotImplementedException();
            }

            public void Initialize(bool restore)
            {
                throw new NotImplementedException();
            }

            public void LoadContent(ContentLoader content)
            {
                throw new NotImplementedException();
            }

            public void Notify<T>(string message, T data)
            {
                throw new NotImplementedException();
            }

            public void UnloadContent()
            {
                throw new NotImplementedException();
            }

            public void Update()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void AddComponent()
        {
            var List = new ComponentList();
            var Transform = List.Add<Transform>();
            Assert.IsNotNull(Transform);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void AddComponentTwice()
        {
            var List = new ComponentList();
            List.Add<Transform>();
            List.Add<Transform>();
        }

        [TestMethod]
        public void GetComponent()
        {
            var List = new ComponentList();
            var Transform = List.Add<Transform>();
            var GetTransform = List.Get<Transform>();
            Assert.IsNotNull(GetTransform);
            Assert.AreEqual(Transform, GetTransform);
        }

        [TestMethod]
        public void RemoveComponent()
        {
            var List = new ComponentList();
            var Transform = List.Add<Transform>();
            List.Remove<Transform>();
            var GetTransform = List.Get<Transform>();
            Assert.IsNull(GetTransform);
        }

        [TestMethod]
        public void RemoveMissingComponent()
        {
            var List = new ComponentList();
            var RemoveResult = List.Remove<CustomComponent>();

            Assert.IsFalse(RemoveResult);
        }

        [TestMethod]
        public void ComponentInterfaces()
        {
            var List = new ComponentList();
            List.Add<CustomComponent>();

            foreach (var ComponentList in GetComponentTypeLists(List))
            {
                Assert.AreEqual(1, ComponentList.Count);
            }

            List.Remove<CustomComponent>();

            foreach (var ComponentList in GetComponentTypeLists(List))
            {
                Assert.AreEqual(0, ComponentList.Count);
            }
        }

        [TestMethod]
        public void ComponentNoInterfaces()
        {
            var List = new ComponentList();
            List.Add<Transform>();

            foreach (var ComponentList in GetComponentTypeLists(List))
            {
                Assert.AreEqual(0, ComponentList.Count);
            }

            List.Remove<Transform>();

            foreach (var ComponentList in GetComponentTypeLists(List))
            {
                Assert.AreEqual(0, ComponentList.Count);
            }
        }

        [TestMethod]
        public void Deserialize()
        {
            var List = new ComponentList();
            List.Add<CustomComponent>();

            var Bytes = STACK.State.Serialization.SaveState(List);
            var Deserialized = STACK.State.Serialization.LoadState<ComponentList>(Bytes);

            foreach (var ComponentList in GetComponentTypeLists(Deserialized))
            {
                Assert.AreEqual(1, ComponentList.Count);
            }

        }

        private IEnumerable<ICollection> GetComponentTypeLists(ComponentList componentList)
        {
            yield return componentList.UpdateCompontents;
            yield return componentList.DrawCompontents;
            yield return componentList.InitializeCompontents;
            yield return componentList.InteractiveCompontents;
            yield return componentList.NotifyCompontents;
            yield return componentList.ContentCompontents;
        }
    }
}
