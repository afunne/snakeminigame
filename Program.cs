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

            
            SoundManager sound = new SoundManager(); // sounds go br

            string difficulty = "Medium"; // default, will be changed when player chooses aka bug fix

            sound.PlayLoop("../../../sounds/menu.wav");  // wow cool menu music

            Console.Clear();
            Console.WriteLine("-_- MAO MÄNG (Hussein Tahmazov version) -_-");
            Console.WriteLine("Vali raskusaste:");
            Console.WriteLine("1 - Lihtne (väike kaart)");
            Console.WriteLine("2 - Keskmine (vaikimisi kaart)");
            Console.WriteLine("3 - Raske (juhuslikud lisaseinad)");
            ConsoleKey key = Console.ReadKey().Key;

            Console.Clear();

            // Default values
            int mapWidth = 80;
            int mapHeight = 25;
            int scorePerFood = 20;

            // Stop menu soundtrack then other sound comes in
            sound.Stop();


            if (key == ConsoleKey.D1) // it is easier rather consoleread
            {
                difficulty = "Easy";
                mapWidth = 50;
                mapHeight = 20;
                scorePerFood = 10;

                sound.PlayLoop("sounds/easymode.wav");  
            }
            else if (key == ConsoleKey.D2)
            {
                difficulty = "Medium";
                mapWidth = 80;
                mapHeight = 25;
                scorePerFood = 20;

                sound.PlayLoop("sounds/mediummode.wav");
            }
            else if (key == ConsoleKey.D3)
            {
                difficulty = "Hard";
                mapWidth = 80;
                mapHeight = 25;
                scorePerFood = 30;

                sound.PlayLoop("sounds/hardmode.wav");
            }
              
            // walls go brrrrrrr
            Walls walls = new Walls(mapWidth, mapHeight);
            walls.Draw();

            // For hard mode – extra random walls :P
            if (key == ConsoleKey.D3)
            {
                Random rnd = new Random();
                for (int i = 0; i < 3; i++)
                {
                    int y = rnd.Next(3, mapHeight - 3);
                    HorizontalLine extraWall = new HorizontalLine(10, mapWidth - 10, y, '#');
                    walls.AddWall(extraWall);   // bug fix for cololision
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

            // hard mode loop
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

                        // if the user gets at least 100 dolla dolla (this gamemode is hard as hell, Maksim dont try this)
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

                    // spawn bullets every few ticks (its very buggy)
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
                // other gamemodes loops
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

            //uh oh u got pwned XD
            sound.Stop(); // stops backround music
            WriteGameOver();

            string name = ""; // bug fix
            while (true)
            {
                Console.Write("Sisestage oma 3-täheline nimi: ");
                name = Console.ReadLine().ToUpper().Trim();

                // Checks if thhere is nothing
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Peate sisestama nime");
                    continue;
                }

                // Checks length
                if (name.Length != 3)
                {
                    Console.WriteLine("Nimi peab olema täpselt 3 tähemärki pikk. Proovi uuesti");
                    continue;
                }

                // Checks if there is ONLY A–Z letters >:[
                bool onlyLetters = true;
                foreach (char c in name)
                {
                    if (c < 'A' || c > 'Z')
                    {
                        onlyLetters = false;
                        break;
                    }
                }

                if (!onlyLetters) // if not true
                {
                    Console.WriteLine("Nimi peab sisaldama ainult tähti (A-Z). Proovi uuesti");
                    continue;
                }

                break; // valid name :D
            }

            Leaderboard.SaveScore(name, score, difficulty);
            Leaderboard.ShowTopScores();



            Console.ReadLine();

            // pwetty please play again :3333
            Console.WriteLine();
            ConsoleKey response;
            while (true)
            {
                Console.Write("Do you want to play again? (Y/N): ");
                response = Console.ReadKey(true).Key; // true to not show the key
                if (response == ConsoleKey.Y || response == ConsoleKey.N)
                    break; // valid input
                Console.WriteLine("Palun vajutage ainult Y või N");
            }

            if (response == ConsoleKey.Y)
            {
                Console.Clear();
                Main(args); // restart the game NOW!!!!
            }
            else
            {
                return; // exit program
            }
        }

        // u ded
        static void WriteGameOver()
        {
            int xOffset = 25;
            int yOffset = 8;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(xOffset, yOffset++);
            WriteText("============================", xOffset, yOffset++);
            WriteText("GAME OVER! :P", xOffset + 5, yOffset++);
            yOffset++;
            WriteText("Author: Hussein Tahmazov aka afunne aka Husseinkchik aka afunnejoke", xOffset + 2, yOffset++);
            WriteText("============================", xOffset, yOffset++);
            Console.ResetColor(); // yeah I wanted to add colors in the game didnt really work as i planned
        }

        static void WriteText(String text, int xOffset, int yOffset)
        {
            Console.SetCursorPosition(xOffset, yOffset);
            Console.WriteLine(text);
        }

    }
}


