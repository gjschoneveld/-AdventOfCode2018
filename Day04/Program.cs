using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day04
{
    class Shift
    {
        public int id;
        public List<(int from, int to)> sleeping;

        public List<int> GetSleepingMinutes()
        {
            var result = new List<int>();

            foreach (var (from, to) in sleeping)
            {
                for (int i = from; i < to; i++)
                {
                    result.Add(i);
                }
            }

            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").ToList();
            input.Sort();

            // parsing
            var shifts = new List<Shift>();

            Shift currentShift = null;

            int sleepTime = 0;

            foreach (var line in input)
            {
                var parts = line.Split(new char[] { ' ', '[', ']', ':', '#' }, StringSplitOptions.RemoveEmptyEntries);

                var time = int.Parse(parts[2]);

                switch (parts[3])
                {
                    case "Guard":
                        if (currentShift != null)
                        {
                            shifts.Add(currentShift);
                        }

                        currentShift = new Shift
                        {
                            id = int.Parse(parts[4]),
                            sleeping = new List<(int from, int to)>()
                        };
                        break;
                    case "falls":
                        sleepTime = time;
                        break;
                    case "wakes":
                        currentShift.sleeping.Add((sleepTime, time));
                        break;
                }
            }

            shifts.Add(currentShift);

            // processing
            var totals = new Dictionary<int, List<int>>();

            foreach (var shift in shifts)
            {
                if (!totals.ContainsKey(shift.id))
                {
                    totals[shift.id] = new List<int>();
                }

                totals[shift.id].AddRange(shift.GetSleepingMinutes());
            }

            var max = totals.Max(x => x.Value.Count);
            var maxID = totals.Single(x => x.Value.Count == max).Key;
            var maxMinute = totals[maxID].GroupBy(x => x).OrderByDescending(g => g.Count()).First().Key;

            var answer1 = maxID * maxMinute;
            Console.WriteLine($"Answer 1: {answer1}");


            var maxFrequencies = new List<(int id, int number, int count)>();
            foreach (var kv in totals)
            {
                if (kv.Value.Count == 0)
                {
                    continue;
                }

                var id = kv.Key;
                var group = kv.Value.GroupBy(x => x).OrderByDescending(g => g.Count()).First();
                var number = group.Key;
                var count = group.Count();

                maxFrequencies.Add((id, number, count));
            }

            var maxCount = maxFrequencies.Max(f => f.count);
            var maxFrequency = maxFrequencies.Single(f => f.count == maxCount);

            var answer2 = maxFrequency.id * maxFrequency.number;
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
