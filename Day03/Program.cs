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
        private int x;
        private int y;
        private int width;
        private int height;

        public IEnumerable<(int x, int y)> GetOccupiedPositions()
        {
            for (int x = this.x; x < this.x + width; x++)
            {
                for (int y = this.y; y < this.y + height; y++)
                {
                    yield return (x, y);
                }
            }
        }

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
                foreach (var p in r.GetOccupiedPositions())
                {
                    if (!field.ContainsKey(p))
                    {
                        field[p] = new List<Rectangle>();
                    }

                    field[p].Add(r);
                }
            }

            var answer1 = field.Count(kv => kv.Value.Count > 1);
            Console.WriteLine($"Answer 1: {answer1}");


            var overlaps = field.Where(kv => kv.Value.Count > 1).SelectMany(kv => kv.Value).Select(r => r.id).Distinct().ToList();

            var answer2 = rectangles.Single(r => !overlaps.Contains(r.id)).id;
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
