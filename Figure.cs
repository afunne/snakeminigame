using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace Snake
{
    class Figure
    {
        public List<Point> pList;

        public void Draw()
        {
            foreach (Point p in pList)
                p.Draw();
        }

        internal bool IsHit(Figure figure)
        {
            foreach (var p in pList)
                if (figure.IsHit(p))
                    return true;
            return false;
        }

        internal bool IsHit(Point point)
        {
            foreach (var p in pList)
                if (p.IsHit(point))
                    return true;
            return false;
        }
    }
}

