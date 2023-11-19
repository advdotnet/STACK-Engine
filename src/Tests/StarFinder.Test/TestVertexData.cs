using System;

namespace StarFinder.Test
{
	[Serializable]
	public struct TestVertexData : IScalable<TestVertexData>
	{
		public float Value;

		public TestVertexData(float value)
		{
			Value = value;
		}

		public TestVertexData Multiply(float scalar) => new TestVertexData() { Value = Value * scalar };

		public TestVertexData Add(TestVertexData t) => new TestVertexData() { Value = Value + t.Value };

		public TestVertexData Default() => new TestVertexData(1);
	}
}
