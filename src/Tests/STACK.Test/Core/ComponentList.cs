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
		private class CustomComponent : Component, IUpdate, IDraw, IInitialize, IContent, IInteractive, INotify
		{
			public float DrawOrder
			{
				get => throw new NotImplementedException();

				set => throw new NotImplementedException();
			}

			public bool Enabled
			{
				get => throw new NotImplementedException();

				set => throw new NotImplementedException();
			}

			public float UpdateOrder
			{
				get => throw new NotImplementedException();

				set => throw new NotImplementedException();
			}

			public bool Visible
			{
				get => throw new NotImplementedException();

				set => throw new NotImplementedException();
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
			var list = new ComponentList();
			var transform = list.Add<Transform>();
			Assert.IsNotNull(transform);
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void AddComponentTwice()
		{
			var list = new ComponentList();
			list.Add<Transform>();
			list.Add<Transform>();
		}

		[TestMethod]
		public void GetComponent()
		{
			var list = new ComponentList();
			var transform = list.Add<Transform>();
			var getTransform = list.Get<Transform>();
			Assert.IsNotNull(getTransform);
			Assert.AreEqual(transform, getTransform);
		}

		[TestMethod]
		public void RemoveComponent()
		{
			var list = new ComponentList();
			_ = list.Add<Transform>();
			list.Remove<Transform>();
			var getTransform = list.Get<Transform>();
			Assert.IsNull(getTransform);
		}

		[TestMethod]
		public void RemoveMissingComponent()
		{
			var list = new ComponentList();
			var removeResult = list.Remove<CustomComponent>();

			Assert.IsFalse(removeResult);
		}

		[TestMethod]
		public void ComponentInterfaces()
		{
			var list = new ComponentList();
			list.Add<CustomComponent>();

			foreach (var componentList in GetComponentTypeLists(list))
			{
				Assert.AreEqual(1, componentList.Count);
			}

			list.Remove<CustomComponent>();

			foreach (var componentList in GetComponentTypeLists(list))
			{
				Assert.AreEqual(0, componentList.Count);
			}
		}

		[TestMethod]
		public void ComponentNoInterfaces()
		{
			var list = new ComponentList();
			list.Add<Transform>();

			foreach (var componentList in GetComponentTypeLists(list))
			{
				Assert.AreEqual(0, componentList.Count);
			}

			list.Remove<Transform>();

			foreach (var componentList in GetComponentTypeLists(list))
			{
				Assert.AreEqual(0, componentList.Count);
			}
		}

		[TestMethod]
		public void Deserialize()
		{
			var list = new ComponentList();
			list.Add<CustomComponent>();

			var bytes = State.Serialization.SaveState(list);
			var deserialized = State.Serialization.LoadState<ComponentList>(bytes);

			foreach (var componentList in GetComponentTypeLists(deserialized))
			{
				Assert.AreEqual(1, componentList.Count);
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
