using System;
using System.Collections.Generic;

namespace CloudRating.Processor
{
    public class TupleComparer: IComparer<Tuple<int, int>>
    {
        public int Compare(Tuple<int, int> x, Tuple<int, int> y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
    }
}
