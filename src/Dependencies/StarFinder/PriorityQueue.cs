using System;
using System.Collections.Generic;

namespace StarFinder
{
    /// <summary>
    /// Priority queue using a binary heap.
    /// </summary>    
    public class PriorityQueue<T>
    {
        List<T> InnerList = new List<T>();
        IComparer<T> Comparer;

        public PriorityQueue()
        {
            Comparer = Comparer<T>.Default;
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            Comparer = comparer;
        }

        void Switch(int a, int b)
        {
            T c = InnerList[a];
            InnerList[a] = InnerList[b];
            InnerList[b] = c;
        }

        private int Compare(int a, int b)
        {
            return Comparer.Compare(InnerList[a], InnerList[b]);
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
            InnerList.Add(item);

            return BubbleUp(InnerList.Count - 1);
        }

        public T Pop()
        {
            var result = InnerList[0];
            InnerList[0] = InnerList[InnerList.Count - 1];
            InnerList.RemoveAt(InnerList.Count - 1);
            BubbleDown();

            return result;
        }

        void BubbleDown()
        {
            int p = 0, p1, p2, pn;

            do
            {
                pn = p;
                p1 = 2 * p + 1;
                p2 = 2 * p + 2;

                if (InnerList.Count > p1 && Compare(p, p1) > 0)
                {
                    p = p1;
                }

                if (InnerList.Count > p2 && Compare(p, p2) > 0)
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

        void Update(int i)
        {
            if (BubbleUp(i) < i)
            {
                return;
            }

            BubbleDown();
        }

        public void Clear()
        {
            InnerList.Clear();
        }

        public int Count
        {
            get
            {
                return InnerList.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                return InnerList[index];
            }
            set
            {
                InnerList[index] = value;
                Update(index);
            }
        }

        /// <summary>
        /// Finds an item in the queue satisfying the predicate.
        /// </summary>        
        /// <returns>The index of the item or -1.</returns>
        public int Find(Func<T, bool> predicate)
        {
            for (int i = 0; i < this.Count; i++)
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
