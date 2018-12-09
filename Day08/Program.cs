using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day08
{
    class Node
    {
        public List<Node> children;
        public List<int> metadata;

        public List<int> GatherMetadata()
        {
            var result = new List<int>(metadata);

            foreach (var child in children)
            {
                result.AddRange(child.GatherMetadata());              
            }

            return result;
        }

        public int CalculateValue()
        {
            if (children.Count == 0)
            {
                return metadata.Sum();
            }

            var childValues = children.Select(c => c.CalculateValue()).ToList();

            int result = 0;
            foreach (var index in metadata)
            {
                if (1 <= index && index <= childValues.Count)
                {
                    result += childValues[index - 1];
                }
            }

            return result;
        }

        public static Node Parse(List<int> values, ref int position)
        {
            var childrenCount = values[position++];
            var metadataCount = values[position++];

            var children = new List<Node>();
            for (int i = 0; i < childrenCount; i++)
            {
                var child = Parse(values, ref position);
                children.Add(child);
            }

            var metadata = new List<int>();
            for (int i = 0; i < metadataCount; i++)
            {
                var md = values[position++];
                metadata.Add(md);
            }

            return new Node
            {
                children = children,
                metadata = metadata
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var values = input.Split(' ').Select(int.Parse).ToList();

            var position = 0;
            var tree = Node.Parse(values, ref position);

            var answer1 = tree.GatherMetadata().Sum();
            Console.WriteLine($"Answer 1: {answer1}");


            var answer2 = tree.CalculateValue();
            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
