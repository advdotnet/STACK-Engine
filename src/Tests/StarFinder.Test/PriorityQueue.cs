using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StarFinder.Test
{
	[TestClass]
	public class PrioQueueTest
	{
		[TestMethod]
		public void PopsInCorrectOrder()
		{
			var queue = new PriorityQueue<int>();

			queue.Push(4);
			queue.Push(2);
			queue.Push(6);
			queue.Push(1);
			queue.Push(7);

			Assert.AreEqual(1, queue.Pop());
			Assert.AreEqual(2, queue.Pop());
			Assert.AreEqual(4, queue.Pop());
			Assert.AreEqual(6, queue.Pop());
			Assert.AreEqual(7, queue.Pop());
		}

		[TestMethod]
		public void UpdateItem()
		{
			var queue = new PriorityQueue<int>();

			queue.Push(4);
			queue.Push(2);
			queue.Push(6);
			queue.Push(1);
			queue.Push(7);

			queue[queue.Find(i => i == 7)] = 0;

			Assert.AreEqual(0, queue.Pop());
			Assert.AreEqual(1, queue.Pop());
			Assert.AreEqual(2, queue.Pop());
			Assert.AreEqual(4, queue.Pop());
			Assert.AreEqual(6, queue.Pop());
		}
	}
}
