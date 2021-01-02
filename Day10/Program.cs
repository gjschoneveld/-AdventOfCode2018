using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day10
{
    class Point
    {
        public (int x, int y) position;
        public (int x, int y) velocity;

        public void Step()
        {
            position = (position.x + velocity.x, position.y + velocity.y);
        }

        public void StepBack()
        {
            position = (position.x - velocity.x, position.y - velocity.y);
        }

        public static Point Parse(string x)
        {
            var parts = x.Split(new[] { ' ', ',', '<', '>', '=' }, StringSplitOptions.RemoveEmptyEntries);

            return new Point
            {
                position = (int.Parse(parts[1]), int.Parse(parts[2])),
                velocity = (int.Parse(parts[4]), int.Parse(parts[5]))
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var points = input.Select(Point.Parse).ToList();

            var seconds = 0;

            var minY = points.Min(p => p.position.y);
            var maxY = points.Max(p => p.position.y);
            var height = maxY - minY + 1;

            var minHeight = height;

            while (height <= minHeight)
            {
                minHeight = height;

                foreach (var p in points)
                {
                    p.Step();
                }

                minY = points.Min(p => p.position.y);
                maxY = points.Max(p => p.position.y);
                height = maxY - minY + 1;

                seconds++;
            }

            // we exceeded the minimum by 1 step, so we do 1 step back
            foreach (var p in points)
            {
                p.StepBack();
            }
            seconds--;

            Console.WriteLine("Answer 1:");

            var minX = points.Min(p => p.position.x);
            var maxX = points.Max(p => p.position.x);

            minY = points.Min(p => p.position.y);
            maxY = points.Max(p => p.position.y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    var position = (x, y);
                    if (points.Any(p => p.position == position))
                    {
                        Console.Write('█');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            var answer2 = seconds;
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
