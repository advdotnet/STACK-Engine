using System;
using System.Collections.Generic;
using StarFinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace StarFinder.Test
{

    struct MapPosition : IMapPosition
    {
        public int X, Y, Z;               

        public bool Equals(IMapPosition b)
        {
            if (!(b is MapPosition)) return false;
            var Comp = (MapPosition)b;
            return (Comp.X == X && Comp.Y == Y && Comp.Z == Z);
        }

        public float Cost(IMapPosition parent)
        {
            var Other = (MapPosition)parent;
            int dx = (X - Other.X);
            int dy = (Y - Other.Y);
            int dz = (Z - Other.Z);
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static float Heuristic(MapPosition current, MapPosition goal)
        {
            return current.Cost(goal);
        }
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
            var Start = new MapPosition { Z = 0, X = 0, Y = 0 };
            var End = new MapPosition { Z = 4, X = 9, Y = 9 };

            var PathFinder = new AStar<MapPosition>(GetNeighbours);
            var Result = new List<MapPosition>(15);

            PathFinder.Search(Start, End, ref Result, MapPosition.Heuristic);

            PrintGrid(Result);

            Assert.IsTrue(Result.Count > 0);
        }

        static private void PrintGrid(List<MapPosition> solution = null)
        {
            Trace.WriteLine("");
            for (int z = 0; z <= 4; z++)
            {
                for (int x = 0; x <= 9; x++)
                {
                    for (int y = 0; y <= 9; y++)
                    {
                        var Pos = new MapPosition { X = x, Y = y, Z = z };
                        var Current = GetWalkMap(Pos);
                        var InSolution = false;
                        if (solution != null)
                        {
                            foreach (var Step in solution)
                            {
                                if (Pos.Equals(Step))
                                {
                                    InSolution = true;
                                    break;
                                }
                            }
                        }
                        if (InSolution)
                        {
                            Trace.Write("# ");
                        }
                        else
                        {
                            Trace.Write((Current == -1 ? "X" : Current.ToString()) + " ");
                        }
                    }
                    Trace.WriteLine("");
                }
                Trace.WriteLine(System.Environment.NewLine);
            }
        }

        static private void AddNeighbour(List<MapPosition> list, MapPosition pos)
        {
            int Result = GetWalkMap(pos);
            if (Result != -1)
            {
                list.Add(new MapPosition { X = pos.X, Y = pos.Y, Z = pos.Z });
            }
        }

        static private int GetWalkMap(MapPosition pos) 
        {            
            int[,,] Map =
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

            return Map[pos.Z, pos.X, pos.Y];            
        }
        
        static private IEnumerable<MapPosition> GetNeighbours(MapPosition pos)
        {            
            var Result = new List<MapPosition>();

            AddNeighbour(Result, new MapPosition { X = pos.X - 1, Y = pos.Y, Z = pos.Z});            // left
            AddNeighbour(Result, new MapPosition { X = pos.X, Y = pos.Y - 1, Z = pos.Z});            // bottom
            AddNeighbour(Result, new MapPosition { X = pos.X + 1, Y = pos.Y, Z = pos.Z});            // right
            AddNeighbour(Result, new MapPosition { X = pos.X, Y = pos.Y + 1, Z = pos.Z});            // top
            AddNeighbour(Result, new MapPosition { X = pos.X - 1, Y = pos.Y + 1, Z = pos.Z});        // left bottom
            AddNeighbour(Result, new MapPosition { X = pos.X + 1, Y = pos.Y + 1, Z = pos.Z});        // right bottom
            AddNeighbour(Result, new MapPosition { X = pos.X - 1, Y = pos.Y - 1, Z = pos.Z});        // left top
            AddNeighbour(Result, new MapPosition { X = pos.X + 1, Y = pos.Y - 1, Z = pos.Z});        // right top
            AddNeighbour(Result, new MapPosition { X = pos.X, Y = pos.Y, Z = pos.Z+1});              // up
            AddNeighbour(Result, new MapPosition { X = pos.X, Y = pos.Y, Z = pos.Z-1});              // bottom

            return Result;
        }
	}
}
