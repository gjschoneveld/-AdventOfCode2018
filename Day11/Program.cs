using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day11
{
    class Program
    {
        public static ((int x, int y) position, int power) FindBestPosition(int[,] grid, int squareSize)
        {
            var maxPower = int.MinValue;
            var maxPosition = (x: 0, y: 0);

            var size = grid.GetLength(0);

            for (int x = 1; x <= size - squareSize + 1; x++)
            {
                for (int y = 1; y <= size - squareSize + 1; y++)
                {
                    int power = 0;
                    var position = (x: x, y: y);

                    for (int i = 0; i < squareSize; i++)
                    {
                        for (int j = 0; j < squareSize; j++)
                        {
                            power += grid[x + i - 1, y + j - 1];
                        }
                    }

                    if (power > maxPower)
                    {
                        maxPower = power;
                        maxPosition = position;
                    }
                }
            }

            return (maxPosition, maxPower);
        }

        static void Main(string[] args)
        {
            var input = 2568;

            int size = 300;

            var grid = new int[size, size];

            for (int x = 1; x <= size; x++)
            {
                for (int y = 1; y <= size; y++)
                {
                    var rack = x + 10;
                    var powerLevel = rack * y;
                    powerLevel += input;
                    powerLevel *= rack;
                    powerLevel = (powerLevel / 100) % 10;
                    powerLevel -= 5;

                    grid[x - 1, y - 1] = powerLevel;
                }
            }

            var best = FindBestPosition(grid, 3);

            var answer1 = $"{best.position.x},{best.position.y}";
            Console.WriteLine($"Answer 1: {answer1}");


            var maxPower = int.MinValue;
            var maxPosition = (x: 0, y: 0);
            var maxSquareSize = 0;

            for (int squareSize = 1; squareSize <= size; squareSize++)
            {
                best = FindBestPosition(grid, squareSize);

                if (best.power > maxPower)
                {
                    maxPower = best.power;
                    maxPosition = best.position;
                    maxSquareSize = squareSize;
                }
            }

            var answer2 = $"{maxPosition.x},{maxPosition.y},{maxSquareSize}";
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
