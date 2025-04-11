using System;
using System.Collections.Generic;

class BottomUpParser
{
    static readonly string[,] parseTable = new string[12, 6]
    {
        //   id     +      *      (      )     $
        { "s5", "",  "",  "s4", "",  "" },       // state 0
        { "",  "s6", "",  "",  "",  "acc" },     // state 1
        { "",  "r2", "s7", "",  "r2", "r2" },    // state 2
        { "",  "r4", "r4", "",  "r4", "r4" },    // state 3
        { "s5", "",  "",  "s4", "",  "" },       // state 4
        { "",  "r6", "r6", "",  "r6", "r6" },    // state 5
        { "s5", "",  "",  "s4", "",  "" },       // state 6
        { "s5", "",  "",  "s4", "",  "" },       // state 7
        { "",  "s6", "",  "",  "s11", "" },      // state 8
        { "",  "r1", "s7", "",  "r1", "r1" },    // state 9
        { "",  "r3", "r3", "",  "r3", "r3" },    // state 10
        { "",  "r5", "r5", "",  "r5", "r5" }     // state 11
    };

    static readonly Dictionary<int, Dictionary<string, int>> gotoTable = new()
    {
        { 0, new Dictionary<string, int> { {"E", 1}, {"T", 2}, {"F", 3} } },
        { 4, new Dictionary<string, int> { {"E", 8}, {"T", 2}, {"F", 3} } },
        { 6, new Dictionary<string, int> { {"T", 9}, {"F", 3} } },
        { 7, new Dictionary<string, int> { {"F", 10} } }
    };

    static readonly List<(string head, int length)> productions = new()
    {
        ("E", 3),  // E → E + T
        ("E", 1),  // E → T
        ("T", 3),  // T → T * F
        ("T", 1),  // T → F
        ("F", 3),  // F → ( E )
        ("F", 1)   // F → id
    };

    static readonly Dictionary<string, int> terminalIndex = new()
    {
        {"id", 0}, {"+", 1}, {"*", 2}, {"(", 3}, {")", 4}, {"$", 5}
    };

    static void Main()
    {
        Console.WriteLine("Enter an expression like: id + id * id");
        string input = Console.ReadLine()?.Trim() ?? "";

        var tokens = new List<string>(input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        tokens.Add("$");

        Stack<int> stateStack = new();
        Stack<string> symbolStack = new();
        stateStack.Push(0);

        int pointer = 0;

        while (true)
        {
            int currentState = stateStack.Peek();
            string currentToken = tokens[pointer];

            string action = parseTable[currentState, terminalIndex[currentToken]];

            if (action == "")
            {
                Console.WriteLine($"Error: No valid action at state {currentState} for token '{currentToken}'");
                break;
            }

            if (action.StartsWith("s"))
            {
                int newState = int.Parse(action.Substring(1));
                symbolStack.Push(currentToken);
                stateStack.Push(newState);
                pointer++;
                Console.WriteLine($"Shift '{currentToken}', goto state {newState}");
            }
            else if (action.StartsWith("r"))
            {
                int prodNum = int.Parse(action.Substring(1)) - 1;
                var (head, length) = productions[prodNum];

                for (int i = 0; i < length; i++)
                {
                    symbolStack.Pop();
                    stateStack.Pop();
                }

                symbolStack.Push(head);
                int topState = stateStack.Peek();
                int gotoState = gotoTable.ContainsKey(topState) && gotoTable[topState].ContainsKey(head)
                    ? gotoTable[topState][head]
                    : -1;

                if (gotoState == -1)
                {
                    Console.WriteLine($"Error: No goto for non-terminal '{head}' from state {topState}");
                    break;
                }

                stateStack.Push(gotoState);
                Console.WriteLine($"Reduce by {head} → ..., push goto state {gotoState}");
            }
            else if (action == "acc")
            {
                Console.WriteLine("Accepted! Parsing successful.");
                break;
            }
        }
    }
}
