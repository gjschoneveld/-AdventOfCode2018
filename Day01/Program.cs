using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day01
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");

            var values = input.Select(int.Parse).ToList();

            var answer1 = values.Sum();
            Console.WriteLine($"Answer 1: {answer1}");


            var frequency = 0;
            var seen = new HashSet<int> { frequency };

            var index = 0;
            while (true)
            {
                frequency += values[index];

                if (seen.Contains(frequency))
                {
                    break;
                }

                seen.Add(frequency);

                index++;
                if (index == values.Count)
                {
                    index = 0;
                }
            }

            var answer2 = frequency;
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
