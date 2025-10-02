using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using System;
using System.Threading;
using NAudio.Wave;   // ✅ for audio playback with NAudio

namespace Snake
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(80, 25);
            Console.SetBufferSize(80, 25);

            // ================================
            // 🎵 INIT SOUND MANAGER
            // ================================
            SoundManager sound = new SoundManager();

            // ================================
            // MENU
            // ================================
            sound.PlayLoop("../../../sounds/menu.wav");  // ✅ menu soundtrack loop

            Console.Clear();
            Console.WriteLine("=== SNAKE GAME ===");
            Console.WriteLine("Choose difficulty:");
            Console.WriteLine("1 - Easy (small map)");
            Console.WriteLine("2 - Medium (default map)");
            Console.WriteLine("3 - Hard (random extra walls)");
            ConsoleKey key = Console.ReadKey().Key;

            Console.Clear();

            // Default values
            int mapWidth = 80;
            int mapHeight = 25;
            int scorePerFood = 20;

            // Stop menu soundtrack
            sound.Stop();

            // ================================
            // SELECT DIFFICULTY
            // ================================
            if (key == ConsoleKey.D1)
            {
                mapWidth = 50;
                mapHeight = 20;
                scorePerFood = 10;

                sound.PlayLoop("sounds/easymode.wav");   // ✅ easy soundtrack
            }
            else if (key == ConsoleKey.D2)
            {
                mapWidth = 80;
                mapHeight = 25;
                scorePerFood = 20;

                sound.PlayLoop("sounds/mediummode.wav"); // ✅ medium soundtrack
            }
            else if (key == ConsoleKey.D3)
            {
                mapWidth = 80;
                mapHeight = 25;
                scorePerFood = 30;

                sound.PlayLoop("sounds/hardmode.wav");   // ✅ hard soundtrack
            }

            // ================================
            // INIT GAME OBJECTS
            // ================================
            Walls walls = new Walls(mapWidth, mapHeight);
            walls.Draw();

            // For hard mode – extra random walls
            if (key == ConsoleKey.D3)
            {
                Random rnd = new Random();
                for (int i = 0; i < 3; i++)
                {
                    int y = rnd.Next(3, mapHeight - 3);
                    HorizontalLine extraWall = new HorizontalLine(10, mapWidth - 10, y, '#');
                    extraWall.Draw();
                }
            }

            Point p = new Point(4, 5, '*');
            Snake snake = new Snake(p, 4, Direction.RIGHT);
            snake.Draw();

            FoodCreator foodCreator = new FoodCreator(mapWidth, mapHeight, '$');
            Point food = foodCreator.CreateFood();
            food.Draw();

            int score = 0;

            // ================================
            // GAME LOOP
            // ================================
            while (true)
            {
                // Collision detection
                if (walls.IsHit(snake) || snake.IsHitTail())
                {
                    break;
                }

                if (snake.Eat(food))
                {
                    // update score
                    score += scorePerFood;

                    // 🎵 play eat sound
                    sound.PlayOnce("sounds/eat.wav");

                    // spawn new food
                    food = foodCreator.CreateFood();
                    food.Draw();

                    // draw score
                    Console.SetCursorPosition(0, 0);
                    Console.Write($"Score: {score}   ");
                }
                else
                {
                    snake.Move();
                }

                Thread.Sleep(100);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    snake.HandleKey(keyInfo.Key);
                }
            }

            // ================================
            // GAME OVER
            // ================================
            sound.Stop(); // stop background soundtrack
            WriteGameOver();

            Console.Write("Enter your 3-letter name: ");
            string name = Console.ReadLine().ToUpper();
            if (name.Length > 3)
                name = name.Substring(0, 3);

            Leaderboard.SaveScore(name, score);
            Leaderboard.ShowTopScores();

            Console.ReadLine();
        }

        // ================================
        // GAME OVER SCREEN
        // ================================
        static void WriteGameOver()
        {
            int xOffset = 25;
            int yOffset = 8;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(xOffset, yOffset++);
            WriteText("============================", xOffset, yOffset++);
            WriteText("GAME OVER!", xOffset + 5, yOffset++);
            yOffset++;
            WriteText("Author: Hussein Tahmazov", xOffset + 2, yOffset++);
            WriteText("============================", xOffset, yOffset++);
            Console.ResetColor();
        }

        static void WriteText(String text, int xOffset, int yOffset)
        {
            Console.SetCursorPosition(xOffset, yOffset);
            Console.WriteLine(text);
        }
    }
}


