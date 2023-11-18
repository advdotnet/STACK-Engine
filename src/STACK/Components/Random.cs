using System;

namespace STACK.Components
{
	/// <summary>
	/// Provides methods to generate random numbers.
	/// </summary>
	[Serializable]
	public class Randomizer : Component
	{
		private readonly Random _random;

		public Randomizer()
		{
			_random = new Random(23);
		}

		public double CreateDouble() => _random.NextDouble();

		public int CreateInt() => _random.Next();

		/// <summary>
		/// Returns a nonnegative random number less than the specified maximum.
		/// </summary>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public int CreateInt(int maxValue) => _random.Next(maxValue);

		public int CreateInt(int minValue, int maxValue) => _random.Next(minValue, maxValue);

		public static Randomizer Create(World addTo)
		{
			return addTo.Add<Randomizer>();
		}
	}
}
