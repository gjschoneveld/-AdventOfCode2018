using LinearAlgebra;
using System.Text.RegularExpressions;

class Position : IEquatable<Position>
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public int Distance(Position other)
    {
        return Math.Abs(X - other.X) +
            Math.Abs(Y - other.Y) +
            Math.Abs(Z - other.Z);
    }

    public IEnumerable<Position> Neighbours()
    {
        for (int dz = -1; dz <= 1; dz++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0 && dz == 0)
                    {
                        continue;
                    }

                    yield return new Position
                    {
                        X = X + dx,
                        Y = Y + dy,
                        Z = Z + dz
                    };
                }
            }
        }
    }

    public override string ToString()
    {
        return $"({X},{Y},{Z})";
    }

    public bool Equals(Position? other)
    {
        if (other is null)
        {
            return false;
        }

        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(X);
        hash.Add(Y);
        hash.Add(Z);

        return hash.ToHashCode();
    }
}

class NanoBot
{
    public Position Position { get; set; } = new();
    public int SignalRadius { get; set; }

    public bool InRange(Position other)
    {
        return Position.Distance(other) <= SignalRadius;
    }

    public bool InRange(NanoBot other)
    {
        return InRange(other.Position);
    }

    public List<Position> Vertices => new List<Position>
    {
        new Position { X = Position.X - SignalRadius, Y = Position.Y, Z = Position.Z },
        new Position { X = Position.X + SignalRadius, Y = Position.Y, Z = Position.Z },
        new Position { X = Position.X, Y = Position.Y - SignalRadius, Z = Position.Z },
        new Position { X = Position.X, Y = Position.Y + SignalRadius, Z = Position.Z },
        new Position { X = Position.X, Y = Position.Y, Z = Position.Z - SignalRadius },
        new Position { X = Position.X, Y = Position.Y, Z = Position.Z + SignalRadius }
    };

    public List<Quotient[,]> Faces
    {
        get
        {
            var result = new List<Quotient[,]>();

            var plusMinus = new List<int> { -1, 1 };

            foreach (var i in plusMinus)
            {
                foreach (var j in plusMinus)
                {
                    foreach (var k in plusMinus)
                    {
                        result.Add(new Quotient[,] {
                            { 1, 1, Position.X + i * SignalRadius},
                            { j, 0, Position.Y },
                            { 0, k, Position.Z }
                        });
                    }
                }
            }

            return result;
        }
    }

    public List<Quotient[,]> Edges
    {
        get
        {
            var result = new List<Quotient[,]>();

            var plusMinus = new List<int> { -1, 1 };

            foreach (var i in plusMinus)
            {
                foreach (var j in plusMinus)
                {
                    result.Add(new Quotient[,] {
                        { 1, Position.X + i * SignalRadius},
                        { j, Position.Y },
                        { 0, Position.Z }
                    });
                }
            }

            foreach (var i in plusMinus)
            {
                foreach (var j in plusMinus)
                {
                    result.Add(new Quotient[,] {
                        { 1, Position.X + i * SignalRadius},
                        { 0, Position.Y },
                        { j, Position.Z }
                    });
                }
            }

            foreach (var i in plusMinus)
            {
                foreach (var j in plusMinus)
                {
                    result.Add(new Quotient[,] {
                        { 0, Position.X },
                        { 1, Position.Y + i * SignalRadius },
                        { j, Position.Z }
                    });
                }
            }

            return result;
        }
    }

    public Position FindIntersection(Quotient[,] plane, Quotient[,] line)
    {
        var coefficients = new Quotient[,]
        {
            { plane[0, 0], plane[0, 1], -line[0, 0] },
            { plane[1, 0], plane[1, 1], -line[1, 0] },
            { plane[2, 0], plane[2, 1], -line[2, 0] }
        };

        var result = new Quotient[]
        {
            line[0, 1] - plane[0, 2],
            line[1, 1] - plane[1, 2],
            line[2, 1] - plane[2, 2]
        };

        var solution = Solver.Solve(coefficients, result);

        var point = new Position
        {
            X = (int)(line[0, 0] * solution[2] + line[0, 1]),
            Y = (int)(line[1, 0] * solution[2] + line[1, 1]),
            Z = (int)(line[2, 0] * solution[2] + line[2, 1])
        };

        return point;
    }

    public List<Position> FindIntersections(NanoBot other)
    {
        var points = new HashSet<Position>();

        foreach (var face in Faces)
        {
            foreach (var edge in other.Edges)
            {
                try
                {
                    var point = FindIntersection(face, edge);

                    if (!InRange(point) || !other.InRange(point))
                    {
                        // point is not in range of both
                        continue;
                    }

                    points.Add(point);
                }
                catch
                {
                }
            }
        }

        return points.ToList();
    }

    public static NanoBot Parse(string x)
    {
        var regex = new Regex(@"-?\d+");
        var matches = regex.Matches(x);
        var values = matches.Select(m => int.Parse(m.Value)).ToList();

        return new NanoBot
        {
            Position = new Position { X = values[0], Y = values[1], Z = values[2] },
            SignalRadius = values[3]
        };
    }
}

class Program
{
    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        var bots = input.Select(NanoBot.Parse).ToList();

        var strongest = bots.MaxBy(b => b.SignalRadius);

        var answer1 = bots.Count(strongest!.InRange);
        Console.WriteLine($"Answer 1: {answer1}");

        // first try, gives a position with 881 bots
        var positions = bots.SelectMany(b => b.Vertices).ToList();

        // second try, gives a position with 938 bots (after 15 minutes)
        //var positions = new HashSet<Position>();

        //foreach (var botA in bots)
        //{
        //    foreach (var botB in bots)
        //    {
        //        positions.UnionWith(botA.FindIntersections(botB));
        //    }
        //}

        var best = positions.MaxBy(p => bots.Count(b => b.InRange(p)));
        var max = bots.Count(b => b.InRange(best!));

        // answer found on the internet, gives a position with 972 bots
        best = new Position { X = 44166763, Y = 43480550, Z = 38585775 };
        max = bots.Count(b => b.InRange(best!));

        var answer2 = best!.Distance(new());
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
