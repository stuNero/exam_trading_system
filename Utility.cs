namespace App;

class Utility
{
    public static void GenerateMenu(string title = "Välj Ett Menyval:", params string[] choices)
    {
        string msg = "________________________\n";

        msg += title + "\n";

        for (int i = 0; i < choices.Length; i++)
        {
            msg += $"[{i + 1}] {choices[i]}\n";
        }
        msg += "________________________";
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
    public static void Error(string msg)
    {

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(msg);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("'ENTER' för att återgå till menyn...");
        Console.ResetColor();
        Console.ReadLine();
        Console.Clear();
    }
    public static void Prompt(string input)
    {
        Console.WriteLine(input + "\n(Skriv 'Exit' för att lämna..)");
    }
    public static void Success(string msg, bool menuChoice = true)
    {

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(msg);
        Console.ResetColor();
        if (menuChoice)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("'ENTER' för att återgå till menyn...");
            Console.ResetColor();
            Console.ReadLine();
        }
        Thread.Sleep(1000);
    }
}