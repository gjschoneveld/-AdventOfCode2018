class Unit
{
    public enum UnitType
    {
        Goblin,
        Elf
    }

    public UnitType Type { get; set; }
    public (int x, int y) Position { get; set; }

    public int AttackPower { get; set; } = 3;
    public int Health { get; set; } = 200;

    public bool IsAlive => Health >= 0;

    public override string ToString()
    {
        return Type == UnitType.Goblin ? "G" : "E";
    }
}

class Field
{
    public HashSet<(int x, int y)> Walls { get; set; } = new HashSet<(int x, int y)>();
    public Dictionary<(int x, int y), Unit> Units { get; set; } = new Dictionary<(int x, int y), Unit>();

    public void Print()
    {
        var maxX = Walls.Max(p => p.x);
        var maxY = Walls.Max(p => p.y);

        for (int y = 0; y <= maxY; y++)
        {
            for (int x = 0; x <= maxX; x++)
            {
                if (Walls.Contains((x, y)))
                {
                    Console.Write('#');
                    continue;
                }

                if (!Units.ContainsKey((x, y)))
                {
                    Console.Write('.');
                    continue;
                }

                Console.Write(Units[(x, y)]);
            }

            Console.WriteLine("   " + string.Join(", ", Units.Where(kv => kv.Key.y == y).OrderBy(kv => kv.Key.x).Select(kv => $"{kv.Value}({kv.Value.Health})")));
        }

        Console.WriteLine();
    }

    public List<(int x, int y)> GetNeighbours((int x, int y) position)
    {
        return new List<(int x, int y)>
        {
            (position.x, position.y - 1),
            (position.x - 1, position.y),
            (position.x + 1, position.y),
            (position.x, position.y + 1)
        };
    }

    public (int x, int y)? FindClosestInRange((int x, int y) start, List<Unit> targets)
    {
        var visited = new HashSet<(int x, int y)>();
        var positions = new List<(int x, int y)> { start };

        while (positions.Count > 0)
        {
            var inRange = positions.Where(p => targets.Any(u => GetNeighbours(p).Contains(u.Position))).ToList();

            if (inRange.Count > 0)
            {
                return inRange.OrderBy(p => p.y).ThenBy(p => p.x).First();
            }

            visited.UnionWith(positions);

            positions = positions
                .SelectMany(p => GetNeighbours(p))
                .Where(p => !visited.Contains(p) && !Walls.Contains(p) && !Units.ContainsKey(p))
                .Distinct()
                .ToList();
        }

        return null;
    }

    public bool Round()
    {
        var orderedUnits = Units.OrderBy(kv => kv.Key.y).ThenBy(kv => kv.Key.x).Select(kv => kv.Value).ToList();

        foreach (var unit in orderedUnits)
        {
            if (!unit.IsAlive)
            {
                continue;
            }

            var targets = orderedUnits.Where(u => u.IsAlive && u.Type != unit.Type).ToList();

            if (targets.Count == 0)
            {
                return false;
            }

            var neighbours = GetNeighbours(unit.Position);

            if (!targets.Any(u => neighbours.Contains(u.Position)))
            {
                // move
                var targetPosition = FindClosestInRange(unit.Position, targets);

                if (targetPosition != null)
                {
                    var newPosition = FindClosestInRange(targetPosition.Value, new List<Unit> { unit });

                    if (newPosition != null)
                    {
                        Units.Remove(unit.Position);
                        unit.Position = newPosition.Value;
                        Units[unit.Position] = unit;
                    }
                }
            }

            // attack
            neighbours = GetNeighbours(unit.Position);

            var target = targets
                .Where(u => neighbours.Contains(u.Position))
                .OrderBy(u => u.Health)
                .ThenBy(u => u.Position.y)
                .ThenBy(u => u.Position.x)
                .FirstOrDefault();

            if (target == null)
            {
                continue;
            }

            target.Health -= unit.AttackPower;

            if (!target.IsAlive)
            {
                Units.Remove(target.Position);
            }
        }

        return true;
    }

    public static Field Parse(string[] lines)
    {
        var field = new Field();

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '.')
                {
                    continue;
                }

                if (lines[y][x] == '#')
                {
                    field.Walls.Add((x, y));
                    continue;
                }

                field.Units[(x, y)] = lines[y][x] switch
                {
                    'G' => new Unit { Type = Unit.UnitType.Goblin, Position = (x, y) },
                    'E' => new Unit { Type = Unit.UnitType.Elf, Position = (x, y) },
                    _ => throw new Exception()
                };
            }
        }

        return field;
    }
}

class Program
{
    public static (int outcome, bool allElvesAlive) Simulate(string[] input, int elvenAttackPower)
    {
        var field = Field.Parse(input);

        var elves = field.Units.Select(kv => kv.Value).Where(u => u.Type == Unit.UnitType.Elf).ToList();

        foreach (var elf in elves)
        {
            elf.AttackPower = elvenAttackPower;
        }

        //Console.WriteLine("Initially:");
        //field.Print();

        var rounds = 0;

        while (field.Round())
        {
            rounds++;

            //Console.WriteLine($"After {rounds} rounds:");
            //field.Print();
        }

        var outcome = rounds * field.Units.Sum(kv => kv.Value.Health);
        var allElvesAlive = elves.All(u => u.IsAlive);

        return (outcome, allElvesAlive);
    }

    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");

        var elvenAttackPower = 3;
        var result = Simulate(input, elvenAttackPower);

        var answer1 = result.outcome;
        Console.WriteLine($"Answer 1: {answer1}");

        while (!result.allElvesAlive)
        {
            elvenAttackPower++;
            result = Simulate(input, elvenAttackPower);
        }

        var answer2 = result.outcome;
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
