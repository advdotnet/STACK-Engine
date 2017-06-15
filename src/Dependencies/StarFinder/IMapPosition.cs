using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarFinder
{
    public interface IMapPosition
    {
        float Cost(IMapPosition parent);
        bool Equals(IMapPosition b);
    }
}
