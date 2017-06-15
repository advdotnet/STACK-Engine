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

        public TestVertexData Multiply(float scalar)
        {
            return new TestVertexData() { Value = this.Value * scalar };
        }

        public TestVertexData Add(TestVertexData t)
        {
            return new TestVertexData() { Value = this.Value + t.Value };
        }

        public TestVertexData Default()
        {
            return new TestVertexData(1);
        }
    }
}
