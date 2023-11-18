using System;
using System.Collections.Generic;

namespace StarFinder
{
	/// <summary>
	/// Priority queue using a binary heap.
	/// </summary>    
	public class PriorityQueue<T>
	{
		private readonly List<T> _innerList = new List<T>();
		private readonly IComparer<T> _comparer;

		public PriorityQueue()
		{
			_comparer = Comparer<T>.Default;
		}

		public PriorityQueue(IComparer<T> comparer)
		{
			_comparer = comparer;
		}

		private void Switch(int a, int b)
		{
			var c = _innerList[a];
			_innerList[a] = _innerList[b];
			_innerList[b] = c;
		}

		private int Compare(int a, int b)
		{
			return _comparer.Compare(_innerList[a], _innerList[b]);
		}

		private int BubbleUp(int i)
		{
			int p = i, p2;

			do
			{
				if (p == 0)
				{
					break;
				}

				p2 = (p - 1) / 2;

				if (Compare(p, p2) < 0)
				{
					Switch(p, p2);
					p = p2;
				}
				else
				{
					break;
				}

			} while (true);

			return p;
		}

		public int Push(T item)
		{
			_innerList.Add(item);

			return BubbleUp(_innerList.Count - 1);
		}

		public T Pop()
		{
			var result = _innerList[0];
			_innerList[0] = _innerList[_innerList.Count - 1];
			_innerList.RemoveAt(_innerList.Count - 1);
			BubbleDown();

			return result;
		}

		private void BubbleDown()
		{
			int p = 0, p1, p2, pn;

			do
			{
				pn = p;
				p1 = (2 * p) + 1;
				p2 = (2 * p) + 2;

				if (_innerList.Count > p1 && Compare(p, p1) > 0)
				{
					p = p1;
				}

				if (_innerList.Count > p2 && Compare(p, p2) > 0)
				{
					p = p2;
				}

				if (p == pn)
				{
					break;
				}

				Switch(p, pn);

			} while (true);
		}

		private void Update(int i)
		{
			if (BubbleUp(i) < i)
			{
				return;
			}

			BubbleDown();
		}

		public void Clear()
		{
			_innerList.Clear();
		}

		public int Count => _innerList.Count;

		public T this[int index]
		{
			get => _innerList[index];
			set
			{
				_innerList[index] = value;
				Update(index);
			}
		}

		/// <summary>
		/// Finds an item in the queue satisfying the predicate.
		/// </summary>        
		/// <returns>The index of the item or -1.</returns>
		public int Find(Func<T, bool> predicate)
		{
			for (var i = 0; i < Count; i++)
			{
				if (predicate(this[i]))
				{
					return i;
				}
			}

			return -1;
		}

	}
}
