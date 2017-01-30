using Microsoft.VisualStudio.TestTools.UnitTesting;
using StarFinder;

namespace STACK.Test
{
    [TestClass]
    public class PrioQueueTest
    {
        [TestMethod]
        public void PopsInCorrectOrder()
        {
            var Queue = new PriorityQueue<int>();

            Queue.Push(4);
            Queue.Push(2);
            Queue.Push(6);
            Queue.Push(1);
            Queue.Push(7);

            Assert.AreEqual(1, Queue.Pop());
            Assert.AreEqual(2, Queue.Pop());
            Assert.AreEqual(4, Queue.Pop());
            Assert.AreEqual(6, Queue.Pop());
            Assert.AreEqual(7, Queue.Pop());
        }

        [TestMethod]
        public void UpdateItem()
        {
            var Queue = new PriorityQueue<int>();

            Queue.Push(4);
            Queue.Push(2);
            Queue.Push(6);
            Queue.Push(1);
            Queue.Push(7);

            Queue[Queue.Find(i => i == 7)] = 0;

            Assert.AreEqual(0, Queue.Pop());
            Assert.AreEqual(1, Queue.Pop());
            Assert.AreEqual(2, Queue.Pop());
            Assert.AreEqual(4, Queue.Pop());
            Assert.AreEqual(6, Queue.Pop());            
        }
    }
}
