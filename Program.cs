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

            string difficulty = "Medium"; // default, will be changed when player chooses

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
                difficulty = "Easy";
                mapWidth = 50;
                mapHeight = 20;
                scorePerFood = 10;

                sound.PlayLoop("sounds/easymode.wav");   // ✅ easy soundtrack
            }
            else if (key == ConsoleKey.D2)
            {
                difficulty = "Medium";
                mapWidth = 80;
                mapHeight = 25;
                scorePerFood = 20;

                sound.PlayLoop("sounds/mediummode.wav"); // ✅ medium soundtrack
            }
            else if (key == ConsoleKey.D3)
            {
                difficulty = "Hard";
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
            // For hard mode – extra random walls
            if (key == ConsoleKey.D3)
            {
                Random rnd = new Random();
                for (int i = 0; i < 3; i++)
                {
                    int y = rnd.Next(3, mapHeight - 3);
                    HorizontalLine extraWall = new HorizontalLine(10, mapWidth - 10, y, '#');
                    walls.AddWall(extraWall);   // ✅ add to wallList for collision
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
            if (difficulty == "Hard")
            {
                BulletManager bulletManager = new BulletManager();
                int tickCount = 0;

                while (true)
                {
                    if (walls.IsHit(snake) || snake.IsHitTail() || bulletManager.CheckCollision(snake))
                    {
                        break; // Game Over
                    }

                    if (snake.Eat(food))
                    {
                        score += scorePerFood;
                        sound.PlayOnce("sounds/eat.wav");

                        food = foodCreator.CreateFood();
                        food.Draw();

                        Console.SetCursorPosition(0, 0);
                        Console.Write($"Score: {score}   ");

                        // 🎯 WIN CONDITION for Hard Mode
                        if (score >= 100)
                        {
                            sound.Stop();
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("🎉 YOU WIN! The boss is defeated!");
                            Console.ResetColor();
                            Thread.Sleep(4000);
                            return; // exit game
                        }
                    }
                    else
                    {
                        snake.Move();
                    }

                    // spawn bullets every few ticks
                    tickCount++;
                    if (tickCount % 10 == 0) // adjust frequency
                    {
                        bulletManager.SpawnBullet(mapWidth, mapHeight);
                    }

                    bulletManager.Update(mapWidth, mapHeight);

                    Thread.Sleep(100);

                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        snake.HandleKey(keyInfo.Key);
                    }
                }
            }
            else
            {
                // === NORMAL LOOP FOR EASY + MEDIUM ===
                while (true)
                {
                    if (walls.IsHit(snake) || snake.IsHitTail())
                    {
                        break;
                    }

                    if (snake.Eat(food))
                    {
                        score += scorePerFood;
                        sound.PlayOnce("sounds/eat.wav");

                        food = foodCreator.CreateFood();
                        food.Draw();

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
            }

            // ================================
            // GAME OVER
            // ================================
            sound.Stop(); // stop background soundtrack
            WriteGameOver();

            string name = "";
            while (true)
            {
                Console.Write("Enter your 3-letter name: ");
                name = Console.ReadLine().ToUpper().Trim();

                // Check empty input
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("❌ You must enter a name.");
                    continue;
                }

                // Check length
                if (name.Length != 3)
                {
                    Console.WriteLine("❌ Name must be exactly 3 letters. Try again.");
                    continue;
                }

                // Check only A–Z letters
                bool onlyLetters = true;
                foreach (char c in name)
                {
                    if (c < 'A' || c > 'Z')
                    {
                        onlyLetters = false;
                        break;
                    }
                }

                if (!onlyLetters)
                {
                    Console.WriteLine("❌ Name must contain only letters (A-Z). Try again.");
                    continue;
                }

                break; // ✅ valid name
            }

            Leaderboard.SaveScore(name, score, difficulty);
            Leaderboard.ShowTopScores();



            Console.ReadLine();

            // ================================
            // ASK TO PLAY AGAIN
            // ================================
            Console.WriteLine();
            ConsoleKey response;
            while (true)
            {
                Console.Write("Do you want to play again? (Y/N): ");
                response = Console.ReadKey(true).Key; // true to not show the key
                if (response == ConsoleKey.Y || response == ConsoleKey.N)
                    break; // valid input
                Console.WriteLine(" ❌ Please press Y or N only.");
            }

            if (response == ConsoleKey.Y)
            {
                Console.Clear();
                Main(args); // restart the game
            }
            else
            {
                return; // exit program
            }
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


