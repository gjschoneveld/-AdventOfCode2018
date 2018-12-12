using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    class Program
    {
        public static long Simulate(List<string> rules, List<long> initialPlants, long generations)
        {
            var patternsSeen = new Dictionary<string, (long min, long generation)>();
            var jumped = false;

            var currentPlants = new HashSet<long>();

            foreach (var p in initialPlants)
            {
                currentPlants.Add(p);
            }

            for (long gen = 1; gen <= generations; gen++)
            {
                var extra = 2;
                var min = currentPlants.Min() - extra;
                var max = currentPlants.Max() + extra;

                var nextPlants = new HashSet<long>();

                for (long pos = min; pos <= max; pos++)
                {
                    var pattern = "";
                    for (long i = pos - extra; i <= pos + extra; i++)
                    {
                        if (currentPlants.Contains(i))
                        {
                            pattern += "#";
                        }
                        else
                        {
                            pattern += ".";
                        }
                    }

                    var rule = rules.FirstOrDefault(r => r.StartsWith(pattern));
                    if (rule != null && rule.Last() == '#')
                    {
                        nextPlants.Add(pos);
                    }
                }

                currentPlants = nextPlants;

                if (!jumped)
                {
                    min = currentPlants.Min();
                    max = currentPlants.Max();

                    var currentPatternBuilder = new StringBuilder();
                    for (long pos = min; pos <= max; pos++)
                    {
                        if (currentPlants.Contains(pos))
                        {
                            currentPatternBuilder.Append("#");
                        }
                        else
                        {
                            currentPatternBuilder.Append(".");
                        }
                    }
                    var currentPattern = currentPatternBuilder.ToString();

                    if (!patternsSeen.ContainsKey(currentPattern))
                    {
                        patternsSeen[currentPattern] = (min, gen);
                        continue;
                    }

                    // jump
                    var previousPattern = patternsSeen[currentPattern];

                    var period = gen - previousPattern.generation;
                    var offset = min - previousPattern.min;

                    var fullPeriodsLeft = (generations - gen) / period;
                    gen += fullPeriodsLeft * period;

                    var jumpOffset = fullPeriodsLeft * offset;

                    nextPlants = new HashSet<long>();
                    foreach (var p in currentPlants)
                    {
                        nextPlants.Add(p + jumpOffset);
                    }
                    currentPlants = nextPlants;

                    jumped = true;
                }
            }

            return currentPlants.Sum();
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");

            var rules = input.Skip(2).ToList();

            var initialPlants = input[0].Substring(15).Select((c, i) => (c, i)).Where(x => x.c == '#').Select(x => (long)x.i).ToList();

            var answer1 = Simulate(rules, initialPlants, 20);
            Console.WriteLine($"Answer 1: {answer1}");

            var answer2 = Simulate(rules, initialPlants, 50000000000L);
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
