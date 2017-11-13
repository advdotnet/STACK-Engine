using System;

namespace STACK.Components
{
    /// <summary>
    /// Provides methods to generate random numbers.
    /// </summary>
    [Serializable]
    public class Randomizer : Component
    {
        Random Random;

        public Randomizer()
        {
            Random = new Random(23);
            Visible = false;
        }

        public double CreateDouble()
        {
            return Random.NextDouble();
        }

        public int CreateInt()
        {
            return Random.Next();
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int CreateInt(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public int CreateInt(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

        public static Randomizer Create(World addTo)
        {
            return addTo.Add<Randomizer>();
        }
    }
}
