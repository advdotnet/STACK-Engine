using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StarFinder.Test
{
	internal struct MapPosition : IMapPosition
	{
		public int X, Y, Z;

		public bool Equals(IMapPosition b)
		{
			if (!(b is MapPosition))
			{
				return false;
			}

			var comp = (MapPosition)b;
			return comp.X == X && comp.Y == Y && comp.Z == Z;
		}

		public float Cost(IMapPosition parent)
		{
			var other = (MapPosition)parent;
			var dx = X - other.X;
			var dy = Y - other.Y;
			var dz = Z - other.Z;
			return (float)Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
		}

		public static float Heuristic(MapPosition current, MapPosition goal) => current.Cost(goal);
	}


	/// <summary>
	/// Test class for doing A* pathfinding on a 3D map.
	/// </summary>
	[TestClass]
	public class AStarTest
	{
		[TestMethod]
		public void FindsPath()
		{
			var start = new MapPosition { Z = 0, X = 0, Y = 0 };
			var end = new MapPosition { Z = 4, X = 9, Y = 9 };

			var pathFinder = new AStar<MapPosition>(GetNeighbours);
			var result = new List<MapPosition>(15);

			pathFinder.Search(start, end, ref result, MapPosition.Heuristic);

			PrintGrid(result);

			Assert.IsTrue(result.Count > 0);
		}

		private static void PrintGrid(List<MapPosition> solution = null)
		{
			Trace.WriteLine("");
			for (var z = 0; z <= 4; z++)
			{
				for (var x = 0; x <= 9; x++)
				{
					for (var y = 0; y <= 9; y++)
					{
						var pos = new MapPosition { X = x, Y = y, Z = z };
						var current = GetWalkMap(pos);
						var inSolution = false;
						if (solution != null)
						{
							foreach (var step in solution)
							{
								if (pos.Equals(step))
								{
									inSolution = true;
									break;
								}
							}
						}
						if (inSolution)
						{
							Trace.Write("# ");
						}
						else
						{
							Trace.Write((current == -1 ? "X" : current.ToString()) + " ");
						}
					}
					Trace.WriteLine("");
				}
				Trace.WriteLine(System.Environment.NewLine);
			}
		}

		private static void AddNeighbour(List<MapPosition> list, MapPosition pos)
		{
			var result = GetWalkMap(pos);
			if (result != -1)
			{
				list.Add(new MapPosition { X = pos.X, Y = pos.Y, Z = pos.Z });
			}
		}

		private static int GetWalkMap(MapPosition pos)
		{
			int[,,] map =
			{
				{
					{ 1,-1, 1, 1, 1,-1, 1, 1, 1, 1 },
					{ 1,-1, 1,-1, 1,-1, 1, 1, 1, 1 },
					{ 1,-1, 1,-1, 1,-1, 1, 1, 1, 1 },
					{ 1,-1, 1,-1, 1,-1, 1, 1, 1, 1 },
					{ 1,-1, 1,-1, 1,-1, 1, 1, 1, 1 },
					{ 1,-1, 1,-1, 1,-1, 1, 1, 1, 1 },
					{ 1,-1, 1,-1, 1,-1, 1, 1, 1, 1 },
					{ 1,-1, 1,-1, 1,-1, 1, 1, 1, 1 },
					{ 1,-1, 1,-1, 1,-1, 1, 1, 2, 1 },
					{ 1, 1, 1,-1, 1, 1, 1, 3, 1, 1 }
				},
				{
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1, 1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1, 9,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1, -1 }
				},
				{
					{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1,-1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1,-1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1,-1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1,-1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1,-1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1,-1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1,-1, 1 },
					{ 1,-1,-1,-1,-1,-1,-1,-1,-1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
				},
				{
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1, 1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
					{ -1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }
				},
				{
					{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
					{ 1, 1, 1, 1, 1, 2, 1, 1, 1, 1 },
					{ 1, 1, 1, 1, 2, 3, 2, 1, 1, 1 },
					{ 1, 1, 1, 2, 3, 4, 3, 2, 1, 1 },
					{ 1, 1, 2, 3, 4, 5, 4, 3, 2, 1 },
					{ 1, 1, 1, 2, 3, 4, 3, 2, 1, 1 },
					{ 1, 1, 1, 1, 2, 3, 2, 1, 1, 1 },
					{ 1, 1, 1, 1, 1, 2, 1, 1, 1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
					{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
				}
			};

			if ((pos.X < 0) || (pos.X > 9) ||
				(pos.Y < 0) || (pos.Y > 9) ||
				(pos.Z < 0) || (pos.Z > 4))
			{
				return -1;
			}

			return map[pos.Z, pos.X, pos.Y];
		}

		private static IEnumerable<MapPosition> GetNeighbours(MapPosition pos)
		{
			var result = new List<MapPosition>();

			AddNeighbour(result, new MapPosition { X = pos.X - 1, Y = pos.Y, Z = pos.Z });            // left
			AddNeighbour(result, new MapPosition { X = pos.X, Y = pos.Y - 1, Z = pos.Z });            // bottom
			AddNeighbour(result, new MapPosition { X = pos.X + 1, Y = pos.Y, Z = pos.Z });            // right
			AddNeighbour(result, new MapPosition { X = pos.X, Y = pos.Y + 1, Z = pos.Z });            // top
			AddNeighbour(result, new MapPosition { X = pos.X - 1, Y = pos.Y + 1, Z = pos.Z });        // left bottom
			AddNeighbour(result, new MapPosition { X = pos.X + 1, Y = pos.Y + 1, Z = pos.Z });        // right bottom
			AddNeighbour(result, new MapPosition { X = pos.X - 1, Y = pos.Y - 1, Z = pos.Z });        // left top
			AddNeighbour(result, new MapPosition { X = pos.X + 1, Y = pos.Y - 1, Z = pos.Z });        // right top
			AddNeighbour(result, new MapPosition { X = pos.X, Y = pos.Y, Z = pos.Z + 1 });              // up
			AddNeighbour(result, new MapPosition { X = pos.X, Y = pos.Y, Z = pos.Z - 1 });              // bottom

			return result;
		}
	}
}
