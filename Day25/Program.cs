namespace Day25;

class Point
{
    public List<int> Values { get; set; } = new();

    public int Distance(Point other)
    {
        return Values.Zip(other.Values).Sum(((int a, int b) x) => Math.Abs(x.a - x.b));
    }

    public static Point Parse(string x)
    {
        var values = x.Split(',').Select(int.Parse).ToList();

        return new()
        {
            Values = values
        };
    }
}

class Constellation
{
    public List<Point> Points { get; set; } = new();

    public bool Contains(Point point)
    {
        return Points.Any(p => p.Distance(point) <= 3);
    }

    public static Constellation Merge(List<Constellation> constellations)
    {
        return new()
        {
            Points = constellations.SelectMany(c => c.Points).ToList()
        };
    }
}

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var points = input.Select(Point.Parse).ToList();

        var constellations = new List<Constellation>();

        foreach (var point in points)
        {
            var keep = new List<Constellation>();
            var merge = new List<Constellation>();

            foreach (var constellation in constellations)
            {
                if (constellation.Contains(point))
                {
                    merge.Add(constellation);
                }
                else
                {
                    keep.Add(constellation);
                }
            }

            var merged = Constellation.Merge(merge);
            merged.Points.Add(point);

            constellations = keep;
            constellations.Add(merged);
        }

        var answer = constellations.Count;
        Console.WriteLine($"Answer: {answer}");
    }
}
