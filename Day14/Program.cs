using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day14
{
    class Node
    {
        public Node next;
        public Node previous;

        public int value;
        public int index;
    }

    class Program
    {
        public static List<int> GetDigits(int x)
        {
            var result = new List<int>();

            do
            {
                var digit = x % 10;
                result.Insert(0, digit);

                x /= 10;
            } while (x > 0);

            return result;
        }

        public enum Part
        {
            Part1,
            Part2
        }

        public static string Simulate(int input, Part part)
        {
            var inputString = input.ToString();

            var needed = 10;

            var elves = new List<Node>();

            var first = new Node { value = 3, index = 1 };
            var last = new Node { value = 7, index = 2 };
            first.previous = last;
            first.next = last;
            last.previous = first;
            last.next = first;

            elves.Add(first);
            elves.Add(last);

            while (true)
            {
                var sum = elves.Sum(e => e.value);
                var digits = GetDigits(sum);

                foreach (var d in digits)
                {
                    var next = new Node { value = d, index = last.index + 1, previous = last, next = first };
                    last.next = next;
                    first.previous = next;
                    last = next;

                    if (part == Part.Part1 && last.index >= input + needed)
                    {
                        var current = first;
                        while (current.index <= input)
                        {
                            current = current.next;
                        }

                        var result = "";
                        for (int i = 0; i < needed; i++)
                        {
                            result += current.value;
                            current = current.next;
                        }

                        return result;
                    }

                    if (part == Part.Part2 && last.index >= inputString.Length)
                    {
                        var test = "";
                        var current = last;
                        for (int i = 0; i < inputString.Length; i++)
                        {
                            test = current.value.ToString() + test;
                            current = current.previous;
                        }

                        if (test == inputString)
                        {
                            return current.index.ToString();
                        }
                    }
                }

                for (int i = 0; i < elves.Count; i++)
                {
                    var steps = elves[i].value + 1;
                    for (int j = 0; j < steps; j++)
                    {
                        elves[i] = elves[i].next;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var input = 540391;

            var answer1 = Simulate(input, Part.Part1);
            Console.WriteLine($"Answer 1: {answer1}");

            var answer2 = Simulate(input, Part.Part2);
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
