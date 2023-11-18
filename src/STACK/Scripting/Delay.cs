using System;
using System.Collections;

namespace STACK
{
	public static class Delay
	{
		/// <summary>
		/// Delay for a duration in seconds.
		/// </summary>  
		public static IEnumerator Seconds(float seconds)
		{
			float elapsed = 0;

			while (elapsed < seconds)
			{
				elapsed += GameSpeed.TickDuration;
				yield return 0;
			}
		}

		/// <summary>
		/// Delay for a number of updates.
		/// </summary>
		public static IEnumerator Updates(int count)
		{
			return Seconds(Math.Max(0, (count - 1)) * GameSpeed.TickDuration);
		}
	}
}
