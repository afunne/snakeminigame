using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Snake
{
    class Leaderboard
    {
        private static string filePath = "leaderboard.txt";

        public static void SaveScore(string name, int score, string difficulty)
        {
            string entry = $"{name} {score} {difficulty}";
            File.AppendAllLines(filePath, new[] { entry });
        }

        public static void ShowTopScores()
        {
            Console.Clear();
            Console.WriteLine("-__- TULEMUSED -__-");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Punkte pole veel!");
                return;
            }

            var scores = File.ReadAllLines(filePath)
             .Select(line =>
             {
                 var parts = line.Split(' ');
                 if (parts.Length < 3) return null; // skip old/bad entries aka bug fix

                 int score;
                 if (!int.TryParse(parts[1], out score)) return null; // skip invalid numbers

                 return new
                 {
                     Name = parts[0],
                     Score = score,
                     Difficulty = parts[2]
                 };
             })
             .Where(x => x != null) // remove skipped
             .OrderByDescending(s => s.Score)
             .Take(5);

            int rank = 1;
            foreach (var s in scores)
            {
                Console.WriteLine($"{rank}. {s.Name} - {s.Score} pts - {s.Difficulty}");
                rank++;
            }
        }
    }
}


