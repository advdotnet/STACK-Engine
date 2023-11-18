using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace STACK.Test
{
	[TestClass]
	public class TransformState
	{
		[TestMethod]
		public void SerializePath()
		{
			var list = new ComponentList();
			// 327
			//List.Add<CameraLocked>();
			var bytes = State.Serialization.SaveState(list);

			var deserializedList = State.Serialization.LoadState<ComponentList>(bytes);
			Assert.IsNotNull(deserializedList.Components);
			//File.WriteAllBytes("list.state", Bytes);
		}

		[Serializable]
		private class MyEntity : Entity
		{

		}

		[TestMethod]
		public void SerializeBaseentity()
		{
			var cam = new Text();

			var bytes = State.Serialization.SaveState(cam);
			File.WriteAllBytes("Text.state", bytes);
		}
	}
}
