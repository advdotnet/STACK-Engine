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
            var List = new ComponentList();
            // 327
            //List.Add<CameraLocked>();
            var Bytes = State.Serialization.SaveState(List);

            var DeserializedList = State.Serialization.LoadState<ComponentList>(Bytes);
            Assert.IsNotNull(DeserializedList.Components);
            //File.WriteAllBytes("list.state", Bytes);
        }

        [Serializable]
        class MyEntity : Entity
        {

        }

        [TestMethod]
        public void SerializeBaseentity()
        {
            var Cam = new Text();

            var Bytes = State.Serialization.SaveState(Cam);
            File.WriteAllBytes("Text.state", Bytes);
        }
    }
}
