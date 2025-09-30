using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Threading.Channels;
using App;

string path = "users.csv";
List<User> users = new List<User>();
List<Item> items = new List<Item>();
List<string[]> formatFileLines = FormatFileRead();
UpdateUsers(formatFileLines);

Menu currentMenu = Menu.None;
User? active_user = null;

while (true)
{
    Console.Clear();
    switch (currentMenu)
    {
        case Menu.None:
            Utility.GenerateMenu(title: "     ---Welcome to Gadgetzan Trading---",
                                 choices: new[] { "Login", "Register Account", "Quit" });
            int.TryParse(Console.ReadLine(), out int choice);
            switch (choice)
            {
                case 1:
                    currentMenu = Menu.Login;
                    break;
                case 2:
                    currentMenu = Menu.RegisterAccount;
                    break;
                case 3:
                    Environment.Exit(0);
                    break;
                default:
                    Utility.Error("Invalid choice, please try again.");
                    currentMenu = Menu.None;
                    break;
            }
            break;
        case Menu.RegisterAccount:
            Console.WriteLine("     --Register Account--");

            string name = Utility.Prompt("Enter name: ");
            if (string.IsNullOrWhiteSpace(name))
            { currentMenu = Menu.None; break; }

            string email = Utility.Prompt("Enter email: ");
            if (string.IsNullOrWhiteSpace(email))
            { currentMenu = Menu.None; break; }

            RegisterUser(name, email);

            currentMenu = Menu.None;
            break;
        case Menu.Login:
            Console.WriteLine("     --Login-- ");

            email = Utility.Prompt("Enter email: ");
            if (string.IsNullOrWhiteSpace(email))
            { currentMenu = Menu.None; break; }

            string password = Utility.Prompt("Enter password: ");
            if (string.IsNullOrWhiteSpace(password))
            { currentMenu = Menu.None; break; }

            bool check = false;
            foreach (User user in users)
            {
                if (user.TryLogin(email, password))
                {
                    Console.Clear();
                    check = true;
                    Utility.Success($"User {user.Name} logged in.");
                    active_user = user;
                    currentMenu = Menu.Main;
                }
            }
            if (!check)
            { Utility.Error("Login failed\nWrong credentials.."); }
            break;
        case Menu.Main:
            while (true)
            {
                Utility.GenerateMenu(title: "     ---Main Menu---",
                                     choices: new[] { "Trade", "Add item to system", "View Your Items", "View all items in system", "Log out" });
                int.TryParse(Console.ReadLine(), out int input);
                switch (input)
                {
                    case 1:
                        Console.WriteLine("WIP");
                        Console.ReadLine();
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("     ---Add an item---");

                        string itemName = Utility.Prompt("Item name: ");
                        if (string.IsNullOrWhiteSpace(itemName)) { currentMenu = Menu.Main; break; }

                        string itemDesc = Utility.Prompt("Item description: ");
                        if (string.IsNullOrWhiteSpace(itemDesc)) { currentMenu = Menu.Main; break; }

                        Item newItem = new Item(itemName, itemDesc, active_user);
                        items.Add(newItem);
                        Utility.Success($"Item Added: \n{newItem.Info()}");
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("     ---Your Items---");
                        int ctr = 0;
                        foreach (Item item in items)
                        {
                            if (ctr % 2 == 0)
                            { Console.ForegroundColor = ConsoleColor.Cyan; }
                            else
                            { Console.ForegroundColor = ConsoleColor.Magenta; }

                            if (item.Owner == active_user)
                            {
                                Console.WriteLine($"_\n{item.Info()}\n_");
                            }
                            ctr += 1;
                        }
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                    case 4:
                        Console.Clear();
                        Console.WriteLine("     ---Market---");
                        ctr = 0;
                        foreach (Item item in items)
                        {
                            if (ctr % 2 == 0)
                            { Console.ForegroundColor = ConsoleColor.Cyan; }
                            else
                            { Console.ForegroundColor = ConsoleColor.Magenta; }

                            Console.WriteLine($"_\n{item.Info()}\n_");
                            ctr += 1;
                        }
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                    case 5:
                        Console.Clear();
                        Utility.Success($"User {active_user?.Name} logged out.");
                        active_user = null;
                        currentMenu = Menu.None;
                        break;
                }
                break;
            }
            break;
        default:
            Utility.Error("An unexpected error occurred. Returning to main menu.");
            currentMenu = Menu.Main;
            break;
    }
}
List<string[]> FormatFileRead()
{
    string[] lines = File.ReadAllLines(path);
    List<string[]> formattedLines = new List<string[]>();

    foreach (string line in lines)
    {
        string[] formatLine = line.Split(",");
        formattedLines.Add(formatLine);
    }
    return formattedLines;
}
void UpdateUsers(List<string[]> formattedLines)
{
    foreach (string[] line in formattedLines)
    {
        string email = line[0];
        string password = line[1];
        string name = line[2];
        User newUser = new User(name, email);
        newUser.SetPassword(password, fromFile: true);
        users.Add(newUser);
    }
}
void FileWrite(string path, params string[] toExport)
{
    List<string[]> formattedLines = FormatFileRead();
    bool check = false;
    foreach (string[] line in formattedLines)
    {
        if (line.Contains(toExport[0]))
        { check = true;}
    }
    if (!check)
    {
        using (StreamWriter writer = File.AppendText(path))
        {
            string txt = "";
            for (int i = 0; i < toExport.Length; i++)
            {
                txt += toExport[i] + ",";
            }
            txt = txt.Substring(0, txt.Length - 1);
            writer.WriteLine(txt);
        }
    }
}
void RegisterUser(string? name, string? email)
{
    bool check = false;
    foreach (User user in users)
    {
        if (user.Email == email)
        {
            check = true;
        }
    }
    if (!check)
    {
        User? newUser = new User(name, email);
        newUser.SetPassword();
        if (!newUser.HasPassword())
        {
            Utility.Error("Password cannot be empty");
        }
        else
        {
            users.Add(newUser);
            
            string[] userToExport = new string[3];
            userToExport[0] = newUser.Email;
            userToExport[1] = newUser._password;
            userToExport[2] = newUser.Name;

            FileWrite(path,toExport: userToExport);
            Utility.Success($"Account registered!\nAccount details:\n{newUser.Info(inclPassword:true)}");
        }
    }
}