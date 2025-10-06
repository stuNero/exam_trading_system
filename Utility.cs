namespace App;

/// <summary>
/// Abstract Utility class for reusable code. 
/// </summary>
abstract class Utility
{
    /// <summary>
    /// Generates a numbered menu with a title and menu options based on the amount of input strings in 'choices'.
    /// </summary>
    /// <param name="title">string value with a default state to be the title in a menu</param>
    /// <param name="choices">string array with params keyword for inputting infinite menu options </param>
    public static void GenerateMenu(string title = "Choose a Menu Option:", params string[]choices)
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
    /// <summary>
    /// Prompts the user with an input prompt, prints option to cancel prompt and then returns Console input.
    /// </summary>
    /// <remarks>
    /// DOES NOT HANDLE USER INPUT, this requires outside implementation. 
    /// </remarks>
    /// <param name="input">Consoles prompt</param>
    /// <param name="clear">Boolean variable for option to clear previous console text</param>
    /// <returns></returns>
    public static string Prompt(string input, bool clear = true)
    {
        if (clear)
        { Console.Clear(); }
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n(Empty line and 'ENTER' to cancel..)");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(input);
        Console.ResetColor();
        return Console.ReadLine();
    }
    /// <summary>
    /// Prints a colored Error message and an option to return to menu
    /// </summary>
    /// <param name="msg">Specific Error message</param>
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
    /// <summary>
    /// Prints a colored Success message with a bool option for returning to menu
    /// </summary>
    /// <param name="msg">Specific Success message</param>
    /// <param name="menuChoice">boolean option to include menu message</param>
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