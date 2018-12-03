using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day03
{
    class Rectangle
    {
        public int id;
        public int x;
        public int y;
        public int width;
        public int height;

        public static Rectangle Parse(string x)
        {
            var parts = x.Split(new char[] { ' ', '#', '@', ',', ':', 'x' }, StringSplitOptions.RemoveEmptyEntries);

            return new Rectangle
            {
                id = int.Parse(parts[0]),
                x = int.Parse(parts[1]),
                y = int.Parse(parts[2]),
                width = int.Parse(parts[3]),
                height = int.Parse(parts[4])
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var rectangles = input.Select(Rectangle.Parse).ToList();

            var field = new Dictionary<(int x, int y), List<Rectangle>>();

            foreach (var r in rectangles)
            {
                for (int x = r.x; x < r.x + r.width; x++)
                {
                    for (int y = r.y; y < r.y + r.height; y++)
                    {
                        if (!field.ContainsKey((x, y)))
                        {
                            field[(x, y)] = new List<Rectangle>();
                        }

                        field[(x, y)].Add(r);
                    }
                }
            }

            var answer1 = field.Count(kv => kv.Value.Count > 1);
            Console.WriteLine($"Answer 1: {answer1}");


            var duplicates = field.Where(kv => kv.Value.Count > 1).SelectMany(kv => kv.Value).Select(r => r.id).Distinct().ToList();

            var answer2 = rectangles.Single(r => !duplicates.Contains(r.id)).id;
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
