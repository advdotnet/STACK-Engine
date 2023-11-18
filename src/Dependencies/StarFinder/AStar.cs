using System;
using System.Collections.Generic;

namespace StarFinder
{
	/// <summary>
	/// A* based pathfinding on a generic structure which implements IMapPosition.
	/// </summary>    
	public class AStar<T> : IComparer<AStarNode<T>> where T : IMapPosition
	{
		private readonly PriorityQueue<AStarNode<T>> _open;
		private readonly List<AStarNode<T>> _closed = new List<AStarNode<T>>();
		private readonly Func<T, IEnumerable<T>> _getNeighbors;

		public AStar(Func<T, IEnumerable<T>> getNeighbours)
		{
			_getNeighbors = getNeighbours;
			_open = new PriorityQueue<AStarNode<T>>(this);
		}

		/// <summary>
		/// Searches a path between the given 'start' and 'end' nodes.
		/// </summary>
		/// <param name="start">Start node</param>
		/// <param name="end">End node</param>
		/// <param name="results">The list is populated with the results</param>
		/// <param name="heuristic">Heuristic function</param>
		public void Search(T start, T end, ref List<T> results, Func<T, T, float> heuristic = null)
		{
			results.Clear();

			if (_getNeighbors == null)
			{
				return;
			}

			var h = HeuristicResult(heuristic, start, end);
			var parentNode = new AStarNode<T>(0, h, start, start);

			_open.Clear();
			_closed.Clear();

			_open.Push(parentNode);
			while (_open.Count > 0)
			{
				parentNode = _open.Pop();
				_closed.Add(parentNode);

				if (parentNode.Pos.Equals(end))
				{
					_closed.Add(parentNode);
					PrepareResult(ref results);
					return;
				}

				foreach (var next in _getNeighbors(parentNode.Pos))
				{
					var closedExists = false;

					foreach (var item in _closed)
					{
						if (item.Pos.Equals(next))
						{
							closedExists = true;
							break;
						}
					}

					if (closedExists)
					{
						continue;
					}

					var newG = parentNode.G + next.Cost(parentNode.Pos);
					var index = -1;

					for (var i = 0; i < _open.Count; i++)
					{
						if (_open[i].Pos.Equals(next))
						{
							index = i;
							break;
						}
					}

					if (index == -1 || (index > -1 && newG < _open[index].G))
					{
						h = HeuristicResult(heuristic, next, end);
						var newNode = new AStarNode<T>(newG, h, next, parentNode.Pos);

						if (index == -1)
						{
							_open.Push(newNode);
						}
						else
						{
							_open[index] = newNode;
						}
					}
				}
			}

			return;
		}

		private float HeuristicResult(Func<T, T, float> heuristic, T current, T goal)
		{
			return (heuristic == null) ? 0 : heuristic(current, goal);
		}

		private void PrepareResult(ref List<T> results)
		{
			var goal = _closed[_closed.Count - 1];

			for (var i = _closed.Count - 1; i >= 0; i--)
			{
				if (goal.ParentPos.Equals(_closed[i].Pos) || i == _closed.Count - 1)
				{
					goal = _closed[i];
				}
				else
				{
					_closed.RemoveAt(i);
				}
			}

			for (var i = 0; i < _closed.Count; i++)
			{
				results.Add(_closed[i].Pos);
			}
		}

		public int Compare(AStarNode<T> a, AStarNode<T> b)
		{
			if (a.F > b.F)
			{
				return 1;
			}
			else if (a.F < b.F)
			{
				return -1;
			}

			return 0;
		}
	}
}
