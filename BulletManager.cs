using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace Snake
{
    class BulletManager
    {
        public List<Bullet> bullets = new List<Bullet>();
        public Random random = new Random();

        public void SpawnBullet(int mapWidth, int mapHeight)
        {
            int side = random.Next(4); // 0=left,1=right,2=top,3=bottom
            int x = 0, y = 0;
            Direction dir = Direction.RIGHT;

            switch (side)
            {
                case 0: x = 0; y = random.Next(1, mapHeight - 1); dir = Direction.RIGHT; break;
                case 1: x = mapWidth - 1; y = random.Next(1, mapHeight - 1); dir = Direction.LEFT; break;
                case 2: x = random.Next(1, mapWidth - 1); y = 0; dir = Direction.DOWN; break;
                case 3: x = random.Next(1, mapWidth - 1); y = mapHeight - 1; dir = Direction.UP; break;
            }

            bullets.Add(new Bullet(x, y, 'o', dir));
        }

        public void Update(int mapWidth, int mapHeight)
        {
            foreach (var bullet in bullets)
            {
                bullet.Clear();
                bullet.Move();
                if (!bullet.IsOutOfBounds(mapWidth, mapHeight))
                    bullet.Draw(); // This must be called after Move and only if in bounds
            }
        }

        public bool CheckCollision(Snake snake)
        {
            foreach (var bullet in bullets)
            {
                List<Point> list = snake.pList;
                for (int i = 0; i < list.Count; i++)
                {
                    Point? part = list[i];
                    if (bullet.IsHit(part))
                        return true;
                }
            }
            return false;
        }
    }
}


