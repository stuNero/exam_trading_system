namespace App;

abstract class Utility
{
    public static void GenerateMenu(string title = "Choose a Menu Option:", params string[] choices)
    {
        string msg = "_______________________________\n";
        msg += title + "\n";

        for (int i = 0; i < choices.Length; i++)
        {
            msg += $"\n[{i + 1}] [{choices[i]}]\n";
        }
        msg += "_______________________________";
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
    public static string Prompt(string input, bool clear = true)
    {
        if (clear)
        { Console.Clear();}
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n(Empty line and 'ENTER' to cancel..)");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(input);
        Console.ResetColor();
        return Console.ReadLine();
    }
    public static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(msg);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("'ENTER' to return to menu...");
        Console.ResetColor();
        Console.ReadLine();
        Console.Clear();
    }
    public static void Success(string msg, bool menuChoice = true)
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(msg);
        if (menuChoice)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("'ENTER' to return to menu...");
            Console.ReadLine();
        }
        Console.ResetColor();
        Console.Clear();
    }
}