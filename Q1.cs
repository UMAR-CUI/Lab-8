using System;
using System.Collections.Generic;

class BottomUpParser
{
    static void Main(string[] args)
    {
        // Grammar: S -> aSb | ε
        // Input: "aabb"
        string input = "aabb";
        Stack<string> stack = new Stack<string>();
        int pointer = 0;

        Console.WriteLine("Input: " + input);
        Console.WriteLine("Parsing steps:");

        while (pointer <= input.Length)
        {
            if (pointer < input.Length)
            {
                // Shift
                stack.Push(input[pointer].ToString());
                pointer++;
                Console.WriteLine($"Shift: Stack = [{string.Join(", ", stack.ToArray())}]");
            }

            // Reduce
            bool reduced = true;
            while (reduced)
            {
                reduced = false;

                // Check for "aSb" pattern
                if (stack.Count >= 3)
                {
                    string top1 = stack.Pop();
                    string top2 = stack.Pop();
                    string top3 = stack.Pop();

                    if (top1 == "b" && top2 == "S" && top3 == "a")
                    {
                        stack.Push("S");
                        reduced = true;
                        Console.WriteLine($"Reduce: Stack = [{string.Join(", ", stack.ToArray())}]");
                    }
                    else
                    {
                        // Push back if no reduction
                        stack.Push(top3);
                        stack.Push(top2);
                        stack.Push(top1);
                    }
                }
            }

            // Reduce ε (empty string)
            if (stack.Count == 0 && pointer == input.Length)
            {
                stack.Push("S");
                Console.WriteLine($"Reduce ε: Stack = [{string.Join(", ", stack.ToArray())}]");
            }
        }

        // Check if parsing is successful
        if (stack.Count == 1 && stack.Peek() == "S")
        {
            Console.WriteLine("Parsing successful!");
        }
        else
        {
            Console.WriteLine("Parsing failed.");
        }
    }
}