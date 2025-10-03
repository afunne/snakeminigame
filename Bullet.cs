using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Bullet : Point
    {
        public Direction direction;

        public Bullet(int x, int y, char sym, Direction direction) : base(x, y, sym)
        {
            this.direction = direction;
        }

        public void Move()
        {
            base.Move(1, direction);
        }

        public bool IsOutOfBounds(int width, int height)
        {
            return x < 0 || x >= width || y < 0 || y >= height;
        }

        public new void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.Write(sym);
        }
    }
}


