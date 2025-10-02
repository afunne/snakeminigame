using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snake
{
    static class Leaderboard
    {
        private const string FileName = "leaderboard.txt";

        public static void SaveScore(string name, int score)
        {
            string entry = $"{name}:{score}:{DateTime.Now}";
            File.AppendAllLines(FileName, new[] { entry });
        }

        public static void ShowTopScores()
        {
            Console.Clear();
            Console.WriteLine("=== Leaderboard (Top 5) ===");

            if (!File.Exists(FileName))
            {
                Console.WriteLine("No scores yet!");
                return;
            }

            var scores = File.ReadAllLines(FileName)
                .Select(line =>
                {
                    var parts = line.Split(':');
                    return new { Name = parts[0], Score = int.Parse(parts[1]), Date = parts[2] };
                })
                .OrderByDescending(s => s.Score)
                .Take(5);

            int rank = 1;
            foreach (var s in scores)
            {
                Console.WriteLine($"{rank}. {s.Name} - {s.Score} pts ({s.Date})");
                rank++;
            }
        }
    }
}

