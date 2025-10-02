using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading.Channels;
using App;

string usersPath = "users.csv";
string itemsPath = "items.csv";
string tradesPath = "trades.csv";

List<User>  users   = new List<User>();
List<Item>  items   = new List<Item>();
List<Trade> trades  = new List<Trade>();

List<string[]> userFileLines = FormatFileRead(usersPath);
UpdateUsers(userFileLines);

List<string[]> itemFileLines = FormatFileRead(itemsPath);
UpdateItems(itemFileLines);

List<string[]> tradeFileLines = FormatFileRead(tradesPath);
UpdateTrades(tradeFileLines);

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
        case Menu.Trade:
            Utility.GenerateMenu(title: "     ---Market---",
                                    choices: new[] { "Propose Trade","Browse trade Requests","Show Market Items","Back to Main Menu" });
            int.TryParse(Console.ReadLine(), out int input);
            switch (input)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("     ---Propose a Trade---");

                    Console.WriteLine("Who do you want to trade with?");
                    Console.WriteLine("________________________");
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i] != active_user)
                        {
                            Console.WriteLine($"\n[{i}] \n{users[i].Info()}\n________________________");
                        }
                    }
                    Console.Write("Input user's list number: ");
                    int.TryParse(Console.ReadLine(), out choice);
                    
                    User? tradePartner = users[choice];
                    List<Item>? tradeItems = new List<Item>();
                    while (true)
                    {
                        Console.WriteLine(tradePartner.Name + "'s items:");
                        foreach (Item? item in items)
                        {
                            if (item.Owner == tradePartner && !tradeItems.Contains(item) && item.Available)
                            {
                                Console.WriteLine("\n-" + item.Name);
                            }
                            
                        }
                        string? itemName = Utility.Prompt("Name of item(s) you want: ", clear: false);
                        if (string.IsNullOrWhiteSpace(itemName))
                        { currentMenu = Menu.Trade; break; }

                        foreach (Item? item in items)
                        {
                            if (item.Name.ToLower() == itemName.ToLower() && item.Owner == tradePartner)
                            {
                                tradeItems.Add(item);
                                break;
                            }
                        }
                    }
                    Console.Write("Do you want to give items in this trade? (yes/no)");
                    string? choice1 = Console.ReadLine();
                    if (choice1.ToLower() == "yes")
                    {
                        bool giveItem = true;
                        Console.WriteLine("Which item?\n");
                        while (giveItem)
                        {
                            foreach (Item item in items)
                            {
                                if (item.Owner == active_user)
                                {
                                    Console.WriteLine("\n-" + item.Name);
                                }
                                string itemName = Utility.Prompt("Item name: ");
                                if (string.IsNullOrWhiteSpace(itemName))
                                {
                                    currentMenu = Menu.Trade;
                                    giveItem = false;
                                    break;
                                }
                                tradeItems.Add(item);
                            }
                        }
                    }
                    if (tradeItems.Count() != 0)
                    {
                        Trade trade = new Trade(active_user, tradePartner, tradeItems);
                        List<string> exportTrade = new List<string>();
                        exportTrade.Add(active_user.Email);
                        exportTrade.Add(tradePartner.Email);
                        exportTrade.Add(trade.TradeState.ToString());

                        foreach (Item item in tradeItems)
                        {
                            exportTrade.Add(item.Name);
                        }
                        string[] exportTradeArray = exportTrade.ToArray();
                        FileWrite(tradesPath, toExport: exportTradeArray);
                        foreach (Item item in items)
                        {
                            if (tradeItems.Contains(item))
                            {
                                item.Available = false;
                            }
                        }
                        Utility.Success("Trade request sent!");
                    }
                    else
                    {
                        Utility.Error("No items selected to be traded");
                    }
                    currentMenu = Menu.Trade;
                    break;
                case 2:
                    Console.Clear();
                    foreach (Trade trade in trades)
                    {
                        if (trade.Reciever == active_user || trade.Sender == active_user)
                        {
                            Console.WriteLine(trade.Info());
                        }
                    }
                    Console.ReadLine();
                    currentMenu = Menu.Trade;
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("     ---Available Items---");
                    int ctr = 0;
                    foreach (Item item in items)
                    {
                        if (item.Owner != active_user)
                        {
                            if (ctr % 2 == 0)
                                { Console.ForegroundColor = ConsoleColor.Cyan; }
                                else
                                { Console.ForegroundColor = ConsoleColor.Magenta; }

                            Console.WriteLine($"\n{item.Info()}\n____");
                            ctr += 1;
                        }
                    }
                    Console.ResetColor();
                    Console.ReadLine();
                    currentMenu = Menu.Trade;
                    break;
                case 4:
                    currentMenu = Menu.Main;
                    break;
                default:
                    Utility.Error("An unexpected error occurred. Returning to menu.");
                    currentMenu = Menu.Trade;
                    break;
            }       
            break;
        case Menu.Main:
            Utility.GenerateMenu(title: "     ---Main Menu---",
                                    choices: new[] { "Trade", "Add Item to Market", "View Your Items","Log out" });
            int.TryParse(Console.ReadLine(), out input);
            switch (input)
            {
                case 1:
                    currentMenu = Menu.Trade;
                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine("     ---Add an item---");

                    string itemName = Utility.Prompt("Item name: ");
                    if (string.IsNullOrWhiteSpace(itemName)) { currentMenu = Menu.Main; break; }

                    string itemDesc = Utility.Prompt("Item description: ");
                    if (string.IsNullOrWhiteSpace(itemDesc)) { currentMenu = Menu.Main; break; }

                    Item newItem = new Item(itemName, itemDesc, active_user);

                    string[] itemToExport = new string[3];
                    itemToExport[0] = itemName;
                    itemToExport[1] = itemDesc;
                    itemToExport[2] = active_user.Email;
                    FileWrite(itemsPath, toExport: itemToExport);

                    items.Add(newItem);
                    Utility.Success($"Item Added: \n{newItem.Info()}");
                    currentMenu = Menu.Main;
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
                            Console.WriteLine($"\n{item.Info()}\n____");
                        }
                        ctr += 1;
                    }
                    Console.ResetColor();
                    Console.ReadLine();
                    currentMenu = Menu.Main;
                    break;
                case 4:
                    Console.Clear();
                    Utility.Success($"User {active_user?.Name} logged out.");
                    active_user = null;
                    currentMenu = Menu.None;
                    break;
            }
                break;
        default:
            Utility.Error("An unexpected error occurred. Returning to main menu.");
            currentMenu = Menu.Main;
        break;
    }
}
List<string[]> FormatFileRead(string path)
{
    if (!File.Exists(path))
    {
        File.WriteAllText(path, "");
    }
    string[] lines = File.ReadAllLines(path);
    List<string[]> formattedLines = new List<string[]>();

    foreach (string line in lines)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            string[] formatLine = line.Split(",");
            formattedLines.Add(formatLine);
        }
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
void UpdateItems(List<string[]> formattedLines)
{
    foreach (string[] line in formattedLines)
    {
        string name = line[0];
        string desc = line[1];
        User? owner = null;
        foreach (User user in users)
        {
            if (line[2] == user.Email)
            {
                owner = user;
                break;
            }
        }
        items.Add(new Item(name, desc, owner));
    }
}
void UpdateTrades(List<string[]> formattedLines)
{
    foreach (string[] line in formattedLines)
    {
        List<Item> tradeItems = new List<Item>();

        User[] traders = new User[2];

        string senderEmail = line[0];
        string recieverEmail = line[1];
        string tradeState = line[2];
        foreach (User user in users)
        {
            if (user.Email == senderEmail)
            {
                traders[0] = user;
            }
            if (user.Email == recieverEmail)
            {
                traders[1] = user;
            }
        }
        for (int i = 3; i < line.Length; i++)
        {
            foreach (Item item in items)
                {
                    if (line[i] == item.Name)
                    {
                        tradeItems.Add(item);
                    }
                }
        }
        Trade newTrade = new Trade(traders[0], traders[1], tradeItems);
        newTrade.TradeState = (TradeStatus)Enum.Parse(typeof(TradeStatus), tradeState);
        trades.Add(newTrade);
    }
}
void FileWrite(string path, params string[] toExport)
{
    List<string[]> formattedLines = FormatFileRead(path);
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
        { check = true;}
    }
    if (!check)
    {
        User? newUser = new User(name, email);
        newUser.SetPassword();
        if (string.IsNullOrWhiteSpace(newUser._password))
        { Utility.Error("Password cannot be empty"); }
        else
        {
            users.Add(newUser);

            string[] userToExport = new string[3];
            userToExport[0] = newUser.Email;
            userToExport[1] = newUser._password;
            userToExport[2] = newUser.Name;

            FileWrite(usersPath, toExport: userToExport);
            Utility.Success($"Account registered!\nAccount details:\n{newUser.Info(inclPassword: true)}");
        }
    }
    else
    { Utility.Error($"Email: '{email}' is already taken");}
}