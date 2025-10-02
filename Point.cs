using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Snake
{
    class Point
    {
        public int x;
        public int y;
        public char sym;

        public Point() { }

        public Point(int x, int y, char sym)
        {
            this.x = x;
            this.y = y;
            this.sym = sym;
        }

        public Point(Point p)
        {
            x = p.x;
            y = p.y;
            sym = p.sym;
        }

        public void Move(int offset, Direction direction)
        {
            if (direction == Direction.RIGHT) x += offset;
            else if (direction == Direction.LEFT) x -= offset;
            else if (direction == Direction.UP) y -= offset;
            else if (direction == Direction.DOWN) y += offset;
        }

        public bool IsHit(Point p) => p.x == this.x && p.y == this.y;

        public void Draw()
        {
            try
            {
                Console.SetCursorPosition(x, y);
                Console.Write(sym);
            }
            catch { }
        }

        public void Clear()
        {
            sym = ' ';
            Draw();
        }

        public override string ToString() => $"{x}, {y}, {sym}";
    }
}

