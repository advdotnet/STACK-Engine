using System;
using System.Collections.Generic;

namespace StarFinder
{
    /// <summary>
    /// A* based pathfinding on a generic structure which implements IMapPosition.
    /// </summary>    
    public class AStar<T> : IComparer<AStarNode<T>> where T : IMapPosition
    {        
        PriorityQueue<AStarNode<T>> Open;
        List<AStarNode<T>> Closed = new List<AStarNode<T>>();        
        Func<T, IEnumerable<T>> GetNeighbors;        

        public AStar(Func<T, IEnumerable<T>> getNeighbours)
        {
            GetNeighbors = getNeighbours;                       
            Open = new PriorityQueue<AStarNode<T>>(this);
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

            if (GetNeighbors == null)
            {                
                return;
            }

            var H = HeuristicResult(heuristic, start, end);
            var ParentNode = new AStarNode<T>(0, H, start, start);            

            Open.Clear();
            Closed.Clear();            

            Open.Push(ParentNode);
            while (Open.Count > 0)
            {
                ParentNode = Open.Pop();
                Closed.Add(ParentNode);

                if (ParentNode.Pos.Equals(end))
                {
                    Closed.Add(ParentNode);
                    PrepareResult(ref results);
                    return;
                }
                                
                foreach (T Next in GetNeighbors(ParentNode.Pos))
                {
                    bool ClosedExists = false;

                    foreach (var Item in Closed)
                    {
                        if (Item.Pos.Equals(Next))
                        {
                            ClosedExists = true;
                            break;
                        }
                    }

                    if (ClosedExists) 
                    {
                        continue;
                    }

                    var NewG = ParentNode.G + Next.Cost(ParentNode.Pos);

                    int Index = -1;

                    for(int i = 0; i < Open.Count; i++)
                    {
                        if (Open[i].Pos.Equals(Next))
                        {
                            Index = i;
                            break;
                        }
                    }                    

                    if (Index == -1 || (Index > -1 && NewG < Open[Index].G))
                    {
                        H = HeuristicResult(heuristic, Next, end);
                        var NewNode = new AStarNode<T>(NewG, H, Next, ParentNode.Pos);

                        if (Index == -1)
                        {
                            Open.Push(NewNode);
                        }
                        else
                        {
                            Open[Index] = NewNode;
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
            AStarNode<T> Goal = Closed[Closed.Count - 1];

            for (int i = Closed.Count - 1; i >= 0; i--)
            {
                if (Goal.ParentPos.Equals(Closed[i].Pos) || i == Closed.Count - 1)
                {
                    Goal = Closed[i];
                }
                else
                {
                    Closed.RemoveAt(i);
                }
            }            

            for (int i = 0; i < Closed.Count; i++)
            {
                results.Add(Closed[i].Pos);
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
