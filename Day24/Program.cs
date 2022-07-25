using System.Text;
using System.Text.RegularExpressions;

namespace Day24;

class Army
{
    public string Name { get; init; } = "";
    public List<Group> Groups { get; init; } = new();

    public bool Dead => Groups.All(g => g.Dead);

    public void Prepare(int boost)
    {
        foreach (var group in Groups)
        {
            group.Prepare(boost);
        }
    }

    public override string ToString()
    {
        var result = new StringBuilder();

        result.AppendLine($"{Name}:");

        foreach (var group in Groups)
        {
            result.AppendLine(group.ToString());
        }

        return result.ToString();
    }

    public static Army Parse(string[] input)
    {
        var name = input[0][..^1];
        var groups = input[1..].Select(Group.Parse).ToList();

        return new()
        {
            Name = name,
            Groups = groups
        };
    }
}

class Group
{
    public int StartUnits { get; set; }
    public int Units { get; set; }
    public int HitPoints { get; set; }

    public List<string> ImmuneTo { get; set; } = new();
    public List<string> WeakTo { get; set; } = new();

    public string DamageType { get; set; } = "";
    public int Damage { get; set; }
    public int Boost { get; set; }

    public int Initiative { get; set; }


    public int EffectivePower => Units * (Damage + Boost);

    public bool Dead => Units == 0;

    public int DamageTo(Group other)
    {
        if (other.ImmuneTo.Contains(DamageType))
        {
            return 0;
        }

        if (other.WeakTo.Contains(DamageType))
        {
            return 2 * EffectivePower;
        }

        return EffectivePower;
    }

    public bool TakeDamage(int damage)
    {
        var unitsSlain = damage / HitPoints;

        if (unitsSlain == 0)
        {
            return false;
        }

        Units -= unitsSlain;

        if (Units < 0)
        {
            Units = 0;
        }

        return true;
    }

    public void Prepare(int boost)
    {
        Units = StartUnits;
        Boost = boost; 
    }

    public override string ToString()
    {
        List<string> properties = new();

        if (ImmuneTo.Count > 0)
        {
            properties.Add($"immune to {string.Join(", ", ImmuneTo)}");
        }

        if (WeakTo.Count > 0)
        {
            properties.Add($"weak to {string.Join(", ", WeakTo)}");
        }

        var propertiesText = "";

        if (properties.Count > 0)
        {
            propertiesText = $"({string.Join("; ", properties)}) ";
        }

        return $"{Units} units each with {HitPoints} hit points {propertiesText}with an attack that does {Damage} {DamageType} damage at initiative {Initiative}";
    }

    public static List<int> ExtractNumbers(string line)
    {
        return new Regex(@"\d+", RegexOptions.Compiled)
            .Matches(line)
            .Select(m => int.Parse(m.Value))
            .ToList();
    }

    public static Group Parse(string input)
    {
        var numbers = ExtractNumbers(input);

        var words = input.Split(' ').Select(w => w.Trim('(', ')', ';', ',')).ToList();

        var damageIndex = words.IndexOf("damage");
        var damageType = words[damageIndex - 1];

        var weakIndex = words.IndexOf("weak");
        var immuneIndex = words.IndexOf("immune");

        var weakTo = new List<string>();
        var immuneTo = new List<string>();

        if (weakIndex >= 0)
        {
            weakTo = words.Skip(weakIndex + 2).TakeWhile(w => w != "immune" && w != "with").ToList();
        }

        if (immuneIndex >= 0)
        {
            immuneTo = words.Skip(immuneIndex + 2).TakeWhile(w => w != "weak" && w != "with").ToList();
        }

        return new()
        {
            StartUnits = numbers[0],
            HitPoints = numbers[1],
            ImmuneTo = immuneTo,
            WeakTo = weakTo,
            DamageType = damageType,
            Damage = numbers[2],
            Initiative = numbers[3]
        };
    }
}

class Program
{
    static List<string[]> SplitInput(string[] input)
    {
        var divider = Array.IndexOf(input, "");

        return new List<string[]> { input[..divider], input[(divider + 1)..] };
    }

    static Army? Battle(List<Army> armies, int boost = 0)
    {
        foreach (var army in armies)
        {
            army.Prepare(army.Name == "Immune System" ? boost : 0);
        }

        var otherArmy = new Dictionary<Army, Army>
        {
            { armies[0], armies[1] },
            { armies[1], armies[0] },
        };

        while (armies.All(a => !a.Dead))
        {
            // target selection
            var availableTargets = armies
                .ToDictionary(a => otherArmy[a], a => a.Groups
                .Where(g => !g.Dead)
                .ToHashSet());

            var targets = new Dictionary<Group, Group>();

            foreach (var army in armies)
            {
                foreach (var group in army.Groups.OrderByDescending(g => g.EffectivePower).ThenByDescending(g => g.Initiative))
                {
                    var target = availableTargets[army]
                        .OrderByDescending(g => group.DamageTo(g))
                        .ThenByDescending(g => g.EffectivePower)
                        .ThenByDescending(g => g.Initiative)
                        .FirstOrDefault();

                    if (target == null || group.DamageTo(target) == 0)
                    {
                        continue;
                    }

                    targets[group] = target;
                    availableTargets[army].Remove(target);
                }
            }

            // attacking
            bool damageTaken = false;

            foreach (var group in targets.Keys.OrderByDescending(g => g.Initiative))
            {
                var target = targets[group];
                var damage = group.DamageTo(target);
                damageTaken = target.TakeDamage(damage) || damageTaken;
            }

            if (!damageTaken)
            {
                // no damage means endless loop; no winner in that case
                return null;
            }
        }

        var winner = armies.First(a => !a.Dead);

        return winner;
    }

    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");

        var armies = SplitInput(input).Select(Army.Parse).ToList();

        var winner = Battle(armies);

        var answer1 = winner!.Groups.Sum(g => g.Units);
        Console.WriteLine($"Answer 1: {answer1}");


        int boost = 0;

        do
        {
            winner = Battle(armies, boost);
            boost++;
        } while (winner?.Name != "Immune System");

        var answer2 = winner.Groups.Sum(g => g.Units);
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
