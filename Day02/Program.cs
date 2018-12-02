using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day02
{
    class Program
    {
        static bool HasMultiple(string x, int count)
        {
            return x.GroupBy(c => c).Any(g => g.Count() == count);
        }

        static string CommonLetters(string a, string b)
        {
            var common = a.Where((c, i) => c == b[i]).ToArray();
            return new string(common);
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");

            var count2 = input.Count(l => HasMultiple(l, 2));
            var count3 = input.Count(l => HasMultiple(l, 3));

            var answer1 = count2 * count3;
            Console.WriteLine($"Answer 1: {answer1}");


            string answer2 = null;
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = i + 1; j < input.Length; j++)
                {
                    var common = CommonLetters(input[i], input[j]);
                    if (common.Length == input[0].Length - 1)
                    {
                        answer2 = common;
                        break;
                    }
                }
            }

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
