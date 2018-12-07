using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day07
{
    class Worker
    {
        public char? current;
        public int seconds;
    }

    class Program
    {
        public static (char require, char node) Parse(string x)
        {
            var parts = x.Split(' ');

            return (parts[1][0], parts[7][0]);
        }

        public static char? NextNode(Dictionary<char, List<char>> required, List<char> nodes, List<char> visited)
        {
            foreach (var node in nodes)
            {
                if (required[node].All(n => visited.Contains(n)))
                {
                    return node;
                }
            }

            return null;
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var rules = input.Select(Parse).ToList();

            var nodes = rules.SelectMany(r => new char[] { r.require, r.node }).Distinct().OrderBy(n => n).ToList();
            var nodes2 = nodes.ToList();

            var required = nodes.ToDictionary(n => n, n => rules.Where(r => r.node == n).Select(r => r.require).ToList());

            var visited = new List<char>();

            while (nodes.Count > 0)
            {
                var next = NextNode(required, nodes, visited);

                nodes.Remove(next.Value);
                visited.Add(next.Value);
            }

            var result = new StringBuilder();
            foreach (var node in visited)
            {
                result.Append(node);
            }

            var answer1 = result.ToString();

            Console.WriteLine($"Answer 1: {answer1}");


            var numberOfWorkers = 5;
            var workers = new List<Worker>();
            for (int i = 0; i < numberOfWorkers; i++)
            {
                workers.Add(new Worker());
            }

            var visited2 = new List<char>();

            int second = 0;

            while (nodes2.Count > 0 || workers.Any(w => w.seconds > 0))
            {
                // pick new
                foreach (var w in workers.Where(w => w.seconds == 0))
                {
                    var next = NextNode(required, nodes2, visited2);
                    if (next != null)
                    {
                        w.current = next.Value;
                        w.seconds = next.Value - 'A' + 61;

                        nodes2.Remove(next.Value);
                    }
                }

                // progress time
                second++;
                foreach (var w in workers.Where(w => w.seconds > 0))
                {
                    w.seconds--;
                }

                // finish jobs
                foreach (var w in workers.Where(w => w.seconds == 0 && w.current != null))
                {
                    visited2.Add(w.current.Value);
                    w.current = null;
                }
            }

            var answer2 = second;
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
