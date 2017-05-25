using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK;
using STACK.Input;
using STACK.Components;

namespace STACK.Test.Room1
{
    public class MyObj : Entity { }
    public class MyExit : Entity 
    {
        public MyExit()
        {
            Add<Exit>();
        }
    }
}

namespace STACK.Test
{
    [TestClass]
    public class GameObjectTest
    {               
        [TestMethod]
        public void GameObjectSetsNameSpaceAsID()
        {
            Room1.MyObj GameObject = new Room1.MyObj();
            Assert.AreEqual("STACK.Test.Room1.MyObj", GameObject.ID);            
        }

        [TestMethod]
        public void ExitSetsNameSpaceAsID()
        {
            Room1.MyExit Exit = new Room1.MyExit();
            Assert.AreEqual("STACK.Test.Room1.MyExit", Exit.ID);
        }

        [TestMethod]
        public void KeepsManualID()
        {
            Entity GameObject = new STACK.Entity("myID");
            Assert.AreEqual("myID", GameObject.ID);
        }            
    }
}
