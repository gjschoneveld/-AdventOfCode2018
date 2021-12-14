using System.Text.RegularExpressions;

List<(int x, int y)> Parse(string line)
{
    var regex = new Regex(@"^(?<dim>\w)=(?<value>\d+), \w=(?<from>\d+)..(?<to>\d+)$");

    var match = regex.Match(line);
    var dimension = match.Groups["dim"].Value;
    var value = int.Parse(match.Groups["value"].Value);
    var from = int.Parse(match.Groups["from"].Value);
    var to = int.Parse(match.Groups["to"].Value);

    return Enumerable
        .Range(from, to - from + 1)
        .Select(other => dimension == "x" ? (value, other) : (other, value))
        .ToList();
}

void Print(HashSet<(int x, int y)> clay, HashSet<(int x, int y)> settledWater, HashSet<(int x, int y)> movingWater)
{
    var minX = clay.Min(p => p.x);
    var maxX = clay.Max(p => p.x);

    var minY = clay.Min(p => p.y);
    var maxY = clay.Max(p => p.y);

    for (int y = minY; y <= maxY; y++)
    {
        for (int x = minX; x <= maxX; x++)
        {
            Console.Write(clay.Contains((x, y)) ? "#" : settledWater.Contains((x, y)) ? "~" : movingWater.Contains((x, y)) ? "|" : ".");
        }

        Console.WriteLine();
    }

    Console.WriteLine();
}

(int x, bool barrier) WalkHorizontal(HashSet<(int x, int y)> clay, HashSet<(int x, int y)> settledWater, (int x, int y) position, int direction)
{
    while (true)
    {
        var side = (position.x + direction, position.y);
        var below = (position.x, position.y + 1);

        if (clay.Contains(side))
        {
            return (position.x, true);
        }

        if (!clay.Contains(below) && !settledWater.Contains(below))
        {
            return (position.x, false);
        }

        position = (position.x + direction, position.y);
    }

    throw new Exception();
}


var input = File.ReadAllLines("input.txt");
var clay = input.SelectMany(Parse).ToHashSet();

var movingWater = new HashSet<(int x, int y)>();
var settledWater = new HashSet<(int x, int y)>();

var minY = clay.Min(p => p.y);
var maxY = clay.Max(p => p.y);

var toProcess = new Stack<(int x, int y)>();
toProcess.Push((500, 0));

while (toProcess.Count > 0)
{
    var position = toProcess.Pop();
    movingWater.Add(position);

    var below = (position.x, y: position.y + 1);

    if (below.y > maxY)
    {
        continue;
    }

    if (!clay.Contains(below) && !settledWater.Contains(below) && !movingWater.Contains(below))
    {
        toProcess.Push(below);

        continue;
    }

    if (clay.Contains(below) || settledWater.Contains(below))
    {
        var left = WalkHorizontal(clay, settledWater, position, -1);
        var right = WalkHorizontal(clay, settledWater, position, 1);

        if (left.barrier && right.barrier)
        {
            for (int x = left.x; x <= right.x; x++)
            {
                movingWater.Remove((x, position.y));
                settledWater.Add((x, position.y));

                var up = (x, position.y - 1);

                if (movingWater.Contains(up))
                {
                    toProcess.Push(up);
                }
            }

            continue;
        }

        for (int x = left.x; x <= right.x; x++)
        {
            movingWater.Add((x, position.y));
        }

        var belowSides = new List<(int x, int y)> { (left.x, position.y + 1), (right.x, position.y + 1) };

        foreach (var belowSide in belowSides)
        {
            if (!clay.Contains(belowSide) && !settledWater.Contains(belowSide) && !movingWater.Contains(belowSide))
            {
                toProcess.Push(belowSide);
            }
        }
    }
}

//Print(clay, settledWater, movingWater);

movingWater.RemoveWhere(p => p.y < minY || p.y > maxY);

var answer1 = settledWater.Count + movingWater.Count;
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = settledWater.Count;
Console.WriteLine($"Answer 2: {answer2}");
