﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace STACK.Components
{
	/// <summary>
	/// Represents a sequence of frames used for animations.
	/// </summary>
	[Serializable]
	public class Frames : List<int>
	{
		public Frames(IEnumerable<int> collection) : base(collection)
		{
		}

		/// <summary>
		/// Adds the given value to all frame numbers.
		/// </summary>
		/// <param name="value">value to add</param>
		/// <returns></returns>
		public Frames Shift(int value)
		{
			for (var i = 0; i < Count; i++)
			{
				this[i] += value;
			}

			return this;
		}

		/// <summary>
		/// Reverses the order of the elements in the entire list.
		/// </summary>
		/// <returns></returns>
		public new Frames Reverse()
		{
			base.Reverse();

			return this;
		}

		/// <summary>
		/// Repeats each element in the sequence delay times.
		/// </summary>
		/// <param name="delay"></param>
		/// <returns></returns>
		public Frames AddDelay(int delay)
		{
			var result = this.SelectMany(x => Enumerable.Range(0, delay), (x, e) => x).ToArray();

			Clear();
			AddRange(result);

			return this;
		}

		public static Frames Create(params int[] frames)
		{
			return new Frames(frames.ToList());
		}

		/// <summary>
		///  Generates a sequence of integers within a specified range.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static Frames CreateRange(int start, int count)
		{
			var range = Enumerable.Range(start, count);

			return new Frames(range);
		}

		/// <summary>
		/// Adds a range similar to Enumerable.Range
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public Frames AddRange(int start, int count)
		{
			for (var i = 0; i < count; i++)
			{
				Add(i + start);
			}

			return this;
		}

		/// <summary>
		/// Returns an empty sequence.
		/// </summary>
		public static Frames Empty => Create();

		/// <summary>
		/// Creates a copy of this sequence.
		/// </summary>
		/// <returns></returns>
		public Frames Copy()
		{
			return new Frames(this.ToList());
		}

		/// <summary>
		/// Returns an random element in this sequence
		/// </summary>
		/// <param name="randomizer"></param>
		/// <returns></returns>
		public int GetRandom(Randomizer randomizer)
		{
			var randomIndex = randomizer.CreateInt(0, Count);

			return this[randomIndex];
		}

		/// <summary>
		/// Returns a random element in this sequence not matching valueToExclude.
		/// </summary>
		/// <param name="randomizer"></param>
		/// <param name="valueToExclude"></param>
		/// <returns></returns>
		public int GetRandomExcluding(Randomizer randomizer, int valueToExclude)
		{
			if (Count == 1 && valueToExclude == this[0])
			{
				throw new InvalidOperationException("The only sequence value can't be excluded.");
			}

			var randomResult = GetRandom(randomizer);

			while (randomResult == valueToExclude)
			{
				randomResult = GetRandom(randomizer);
			}

			return randomResult;
		}
	}
}
