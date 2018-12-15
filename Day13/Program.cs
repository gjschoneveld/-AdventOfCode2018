using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day13
{
    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    class Cart
    {
        public (int x, int y) position;
        public Direction direction;

        public bool crashed;

        private Direction intersetionDirection = Direction.Left;

        public void Step(string[] map)
        {
            switch (direction)
            {
                case Direction.Left:
                    position = (position.x - 1, position.y);
                    break;
                case Direction.Right:
                    position = (position.x + 1, position.y);
                    break;
                case Direction.Up:
                    position = (position.x, position.y - 1);
                    break;
                case Direction.Down:
                    position = (position.x, position.y + 1);
                    break;
            }

            var symbol = map[position.y][position.x];

            switch (symbol)
            {
                case '\\':
                    switch (direction)
                    {
                        case Direction.Left:
                            direction = Direction.Up;
                            break;
                        case Direction.Right:
                            direction = Direction.Down;
                            break;
                        case Direction.Up:
                            direction = Direction.Left;
                            break;
                        case Direction.Down:
                            direction = Direction.Right;
                            break;
                    }
                    break;
                case '/':
                    switch (direction)
                    {
                        case Direction.Left:
                            direction = Direction.Down;
                            break;
                        case Direction.Right:
                            direction = Direction.Up;
                            break;
                        case Direction.Up:
                            direction = Direction.Right;
                            break;
                        case Direction.Down:
                            direction = Direction.Left;
                            break;
                    }
                    break;
                case '+':
                    switch (direction)
                    {
                        case Direction.Left:
                            switch (intersetionDirection)
                            {
                                case Direction.Left:
                                    direction = Direction.Down;
                                    break;
                                case Direction.Right:
                                    direction = Direction.Up;
                                    break;
                            }
                            break;
                        case Direction.Right:
                            switch (intersetionDirection)
                            {
                                case Direction.Left:
                                    direction = Direction.Up;
                                    break;
                                case Direction.Right:
                                    direction = Direction.Down;
                                    break;
                            }
                            break;
                        case Direction.Up:
                            switch (intersetionDirection)
                            {
                                case Direction.Left:
                                    direction = Direction.Left;
                                    break;
                                case Direction.Right:
                                    direction = Direction.Right;
                                    break;
                            }
                            break;
                        case Direction.Down:
                            switch (intersetionDirection)
                            {
                                case Direction.Left:
                                    direction = Direction.Right;
                                    break;
                                case Direction.Right:
                                    direction = Direction.Left;
                                    break;
                            }
                            break;
                    }

                    switch (intersetionDirection)
                    {
                        case Direction.Left:
                            intersetionDirection = Direction.Up;
                            break;
                        case Direction.Up:
                            intersetionDirection = Direction.Right;
                            break;
                        case Direction.Right:
                            intersetionDirection = Direction.Left;
                            break;
                    }
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var map = File.ReadAllLines("input.txt");

            var symbolToDirection = new Dictionary<char, Direction>
            {
                ['<'] = Direction.Left,
                ['>'] = Direction.Right,
                ['^'] = Direction.Up,
                ['v'] = Direction.Down
            };

            var carts = new List<Cart>();
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (symbolToDirection.ContainsKey(map[y][x]))
                    {
                        carts.Add(new Cart
                        {
                            position = (x, y),
                            direction = symbolToDirection[map[y][x]]
                        });
                    }

                }
            }

            (int x, int y)? firstCrash = null;

            while (carts.Count(c => !c.crashed) > 1)
            {
                carts = carts.OrderBy(c => c.position.y).ThenBy(c => c.position.x).ToList();

                foreach (var cart in carts)
                {
                    if (cart.crashed)
                    {
                        continue;
                    }

                    cart.Step(map);

                    var other = carts.FirstOrDefault(c => !c.crashed && c != cart && c.position == cart.position);

                    if (other != null)
                    {
                        cart.crashed = true;
                        other.crashed = true;

                        if (firstCrash == null)
                        {
                            firstCrash = cart.position;
                        }
                    }
                }
            }

            var answer1 = $"{firstCrash.Value.x},{firstCrash.Value.y}";
            Console.WriteLine($"Answer 1: {answer1}");

            var survivor = carts.Single(c => !c.crashed);
            var answer2 = $"{survivor.position.x},{survivor.position.y}";
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
