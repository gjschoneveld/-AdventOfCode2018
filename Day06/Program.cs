using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day06
{
    class Program
    {
        public static int Distance((int x, int y) a, (int x, int y) b)
        {
            var x = Math.Abs(a.x - b.x);
            var y = Math.Abs(a.y - b.y);

            return x + y;
        }

        public static (int id, (int x, int y) position) Parse(string x, int id)
        {
            var parts = x.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return (id, (int.Parse(parts[0]), int.Parse(parts[1])));
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var coordinates = input.Select(Parse).ToList();

            var minX = coordinates.Min(c => c.position.x);
            var maxX = coordinates.Max(c => c.position.x);
            var minY = coordinates.Min(c => c.position.y);
            var maxY = coordinates.Max(c => c.position.y);

            var width = maxX - minX + 1;
            var height = maxY - minY + 1;

            var grid1 = new int?[width, height];

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    var postion = (x, y);
                    var minDistance = coordinates.Min(c => Distance(c.position, postion));
                    var closestCoordinates = coordinates.Where(c => Distance(c.position, postion) == minDistance).ToList();

                    if (closestCoordinates.Count == 1)
                    {
                        grid1[x - minX, y - minY] = closestCoordinates.Single().id;
                    }
                }
            }

            var borderIDs = new HashSet<int>();

            for (int y = minY; y <= maxY; y++)
            {
                var left = grid1[0, y - minY];
                var right = grid1[width - 1, y - minY];

                if (left != null && !borderIDs.Contains(left.Value))
                {
                    borderIDs.Add(left.Value);
                }
                if (right != null && !borderIDs.Contains(right.Value))
                {
                    borderIDs.Add(right.Value);
                }
            }

            for (int x = minX; x <= maxX; x++)
            {
                var top = grid1[x - minX, 0];
                var bottom = grid1[x - minX, height - 1];

                if (top != null && !borderIDs.Contains(top.Value))
                {
                    borderIDs.Add(top.Value);
                }
                if (bottom != null && !borderIDs.Contains(bottom.Value))
                {
                    borderIDs.Add(bottom.Value);
                }
            }

            var largestArea = grid1.OfType<int>().Where(id => !borderIDs.Contains(id)).GroupBy(id => id).OrderByDescending(g => g.Count()).First().Count();

            var answer1 = largestArea;
            Console.WriteLine($"Answer 1: {answer1}");


            var threshold = 10000;

            var maxExtra = threshold / coordinates.Count;

            var minX2 = minX - maxExtra;
            var maxX2 = maxX + maxExtra;
            var minY2 = minY - maxExtra;
            var maxY2 = maxY + maxExtra;

            var width2 = maxX2 - minX2 + 1;
            var height2 = maxY2 - minY2 + 1;

            var grid2 = new int[width2, height2];

            for (int y = minY2; y <= maxY2; y++)
            {
                for (int x = minX2; x <= maxX2; x++)
                {
                    var postion = (x, y);
                    var sum = coordinates.Sum(c => Distance(c.position, postion));
                    grid2[x - minX2, y - minY2] = sum;
                }
            }

            var regionSize = grid2.OfType<int>().Count(s => s < threshold);

            var answer2 = regionSize;
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
