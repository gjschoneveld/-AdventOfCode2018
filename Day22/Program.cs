using System.Text;

class Region
{
    public enum TypeType
    {
        Rocky,
        Wet,
        Narrow
    }

    public int GeologicIndex { get; set; }
    public int ErosionLevel { get; set; }
    public TypeType Type { get; set; }
}

class Field
{
    public int Depth { get; set; }
    public (int x, int y) Target { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }
    public Dictionary<(int x, int y), Region> Regions { get; set; } = new();

    public Region GetRegion((int x, int y) position)
    {
        var result = new Region();

        if (position == (0, 0) || position == Target)
        {
            result.GeologicIndex = 0;
        }
        else if (position.y == 0)
        {
            result.GeologicIndex = position.x * 16807;
        }
        else if (position.x == 0)
        {
            result.GeologicIndex = position.y * 48271;
        }
        else
        {
            result.GeologicIndex = Regions[(position.x - 1, position.y)].ErosionLevel * Regions[(position.x, position.y - 1)].ErosionLevel;
        }

        result.ErosionLevel = (result.GeologicIndex + Depth) % 20183;

        result.Type = (Region.TypeType)(result.ErosionLevel % 3);

        return result;
    }

    public void AddRow()
    {
        for (int x = 0; x < Width; x++)
        {
            Regions[(x, Height)] = GetRegion((x, Height));
        }

        Height++;
    }

    public void AddColumn()
    {
        for (int y = 0; y < Height; y++)
        {
            Regions[(Width, y)] = GetRegion((Width, y));
        }

        Width++;
    }

    public void EnsureSize(int height, int width)
    {
        while (Height < height)
        {
            AddRow();
        }

        while (Width < width)
        {
            AddColumn();
        }
    }

    public override string ToString()
    {
        var result = new StringBuilder();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                result.Append(Regions[(x, y)].Type switch
                {
                    _ when (x, y) == (0, 0) => 'M',
                    _ when (x, y) == Target => 'T',
                    Region.TypeType.Rocky => '.',
                    Region.TypeType.Wet => '=',
                    Region.TypeType.Narrow => '|',
                    _ => '?'
                });
            }

            result.AppendLine();
        }

        return result.ToString();
    }
}

class Program
{
    public enum ToolType
    {
        Torch, 
        ClimbingGear,
        Neither
    }

    public static ToolType NextTool(ToolType tool)
    {
        return (ToolType)(((int)tool + 1) % 3);
    }

    public static bool IsValid(Field field, (int x, int y) position, ToolType tool)
    {
        if (position.x < 0 || position.y < 0)
        {
            return false;
        }

        field.EnsureSize(position.y + 1, position.x + 1);

        switch (field.Regions[position].Type)
        {
            case Region.TypeType.Rocky:
                return tool != ToolType.Neither;
            case Region.TypeType.Wet:
                return tool != ToolType.Torch;
            case Region.TypeType.Narrow:
                return tool != ToolType.ClimbingGear;
        }

        throw new Exception();
    }

    public static int FindShortestWay(Field field)
    {
        var toVisit = new Dictionary<((int x, int y) position, ToolType tool), int>
        {
            [((0, 0), ToolType.Torch)] = 0
        };

        var visited = new HashSet<((int x, int y) position, ToolType tool)>();

        while (toVisit.Count > 0)
        {
            (((int x, int y) position, ToolType tool) state, int time) = toVisit.MinBy(kv => kv.Value);

            toVisit.Remove(state);
            visited.Add(state);

            if (state.position == field.Target && state.tool == ToolType.Torch)
            {
                return time;
            }

            var candidates = new List<(((int x, int y) position, ToolType tool) state, int time)>
            {
                (((state.position.x, state.position.y - 1), state.tool), time + 1),
                (((state.position.x - 1, state.position.y), state.tool), time + 1),
                (((state.position.x + 1, state.position.y), state.tool), time + 1),
                (((state.position.x, state.position.y + 1), state.tool), time + 1),
                ((state.position, NextTool(state.tool)), time + 7),
                ((state.position, NextTool(NextTool(state.tool))), time + 7)
            };

            var newVisits = candidates
                .Where(c => IsValid(field, c.state.position, c.state.tool))
                .Where(c => !visited.Contains(c.state))
                .Where(c => !toVisit.ContainsKey(c.state) || c.time < toVisit[c.state]);

            foreach (var visit in newVisits)
            {
                toVisit[visit.state] = visit.time;
            }
        }

        throw new Exception();
    }

    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        var parts = input.SelectMany(line => line.Split(new[] { ' ', ',' })).ToList();

        var depth = int.Parse(parts[1]);
        var target = (x: int.Parse(parts[3]), y: int.Parse(parts[4]));

        var field = new Field
        {
            Depth = depth,
            Target = target
        };

        field.EnsureSize(target.y + 1, target.x + 1);

        var risk = field
            .Regions
            .Where(kv => kv.Key.x <= target.x && kv.Key.y <= target.y)
            .Sum(kv => (int)kv.Value.Type);

        var answer1 = risk;
        Console.WriteLine($"Answer 1: {answer1}");

        var answer2 = FindShortestWay(field);
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
