using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day05
{
    class Program
    {
        static int GetRemainingLength(string polymer)
        {
            bool reactionSeen = true;

            while (reactionSeen)
            {
                reactionSeen = false;

                var remaining = new StringBuilder();

                for (int i = 0; i < polymer.Length; i++)
                {
                    if (i < polymer.Length - 1 &&
                            polymer[i] != polymer[i + 1] &&
                            char.ToLower(polymer[i]) == char.ToLower(polymer[i + 1]))
                    {
                        i++;
                        reactionSeen = true;
                    }
                    else
                    {
                        remaining.Append(polymer[i]);
                    }
                }

                polymer = remaining.ToString();
            }

            return polymer.Length;
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");

            var answer1 = GetRemainingLength(input);
            Console.WriteLine($"Answer 1: {answer1}");


            var minLength = int.MaxValue;
            for (char c = 'a'; c <= 'z'; c++)
            {
                var start = input.Replace(c.ToString(), "").Replace(c.ToString().ToUpper(), "");
                var length = GetRemainingLength(start);
                minLength = Math.Min(length, minLength);
            }

            var answer2 = minLength;
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
