using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day09
{
    class Node
    {
        public Node left;
        public Node right;
        public int value;
    }

    class Program
    {
        public static long Simulate(int playerCount, int maxValue)
        {
            var players = new long[playerCount];

            var marble = new Node { value = 0 };
            marble.left = marble;
            marble.right = marble;

            int value = 1;
            int player = 1;

            while (value <= maxValue)
            {
                if (value % 23 == 0)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        marble = marble.left;
                    }

                    var score = value + marble.value;
                    players[player - 1] += score;

                    var left = marble.left;
                    var right = marble.right;

                    left.right = right;
                    right.left = left;

                    marble = right;
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        marble = marble.right;
                    }

                    var left = marble.left;
                    var right = marble;

                    var insert = new Node { value = value, left = left, right = right };
                    left.right = insert;
                    right.left = insert;

                    marble = insert;
                }

                value++;
                player = (player % playerCount) + 1;
            }

            return players.Max();
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var parts = input.Split(' ');

            int playerCount = int.Parse(parts[0]);
            int maxValue = int.Parse(parts[6]);

            var answer1 = Simulate(playerCount, maxValue);
            Console.WriteLine($"Answer 1: {answer1}");

            var answer2 = Simulate(playerCount, 100 * maxValue);
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
