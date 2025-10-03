using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading.Channels;
using App;

TradeSystem ts = new TradeSystem();

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

            ts.RegisterUser(name, email);
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
            foreach (User user in ts.users)
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
                                choices: new[] { "Propose Trade", "Browse trade Requests", "Show Market Items", "Back to Main Menu" });
            int.TryParse(Console.ReadLine(), out int input);
            switch (input)
            {
                case 1: // Propose trade
                    Console.Clear();
                    Console.WriteLine("     ---Propose a Trade---");

                    Console.WriteLine("Who do you want to trade with?");
                    Console.WriteLine("________________________");
                    for (int i = 0; i < ts.users.Count; i++)
                    {
                        if (ts.users[i] != active_user)
                        {
                            Console.WriteLine($"\n[{i}] \n{ts.users[i].Info()}\n________________________");
                        }
                    }
                    Console.Write("Input user's list number: ");
                    int.TryParse(Console.ReadLine(), out choice);

                    User? tradePartner = ts.users[choice];
                    List<Item>? tradeItems = new List<Item>();
                    while (true)
                    {
                        Console.WriteLine(tradePartner.Name + "'s items:");
                        foreach (Item? item in ts.items)
                        {
                            if (item.Owner == tradePartner && !tradeItems.Contains(item) && item.Available)
                            {
                                Console.WriteLine("\n-" + item.Name);
                            }

                        }
                        string? itemName = Utility.Prompt("Name of item(s) you want: ", clear: false);
                        if (string.IsNullOrWhiteSpace(itemName))
                        { currentMenu = Menu.Trade; break; }

                        foreach (Item? item in ts.items)   // adding items to be traded
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
                            foreach (Item item in ts.items)
                            {
                                if (item.Owner == active_user && item.Available)
                                {
                                    Console.WriteLine("\n-" + item.Name);
                                }
                            }
                            string itemName = Utility.Prompt("Item name: ", clear: false);
                            if (string.IsNullOrWhiteSpace(itemName))
                            {
                                currentMenu = Menu.Trade;
                                giveItem = false;
                                break;
                            }
                            foreach (Item item in ts.items)
                            {
                                if (item.Name == itemName)
                                {
                                    item.Available = false;
                                    tradeItems.Add(item);
                                }
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
                        ts.FileWrite(ts.tradesPath, toExport: exportTradeArray);
                        ts.trades.Add(trade);
                        foreach (Item item in ts.items)
                        {
                            if (tradeItems.Contains(item))
                            { item.Available = false; }
                        }
                        Utility.Success("'Request sent.\nTime is money, friend!'");
                    }
                    else
                    { Utility.Error("No items selected to be traded"); }
                    currentMenu = Menu.Trade;
                    break;
                case 2: // Browse requests
                    Console.Clear();
                    Utility.GenerateMenu(title: "     ---Trade Requests---",
                                        choices: new[] {"Sent","Recieved","Completed Requests","Back to menu"});
                    int.TryParse(Console.ReadLine(), out input);
                    switch (input)
                    {
                        case 1: // Sent
                            check = false;
                            for (int i = 0; i < ts.trades.Count; i++)
                            {
                                if (ts.trades[i].Caller == active_user)
                                {
                                    check = true;
                                    Console.WriteLine($"[{i}]\n" + ts.trades[i].Info());
                                }
                            }
                            if (!check)
                            {
                                Utility.Error("No requests in inbox");
                                break;
                            }
                            Console.ReadLine();
                            break;
                        case 2: // Recieved
                            check = false;
                            for (int i = 0; i < ts.trades.Count; i++)
                            {
                                if (ts.trades[i].Responder == active_user && ts.trades[i].TradeState == TradeStatus.Pending)
                                {
                                    check = true;
                                    Console.WriteLine($"\n[{i}]\n" + ts.trades[i].Info());
                                }
                            }
                            if (!check)
                            {
                                Utility.Error("No requests in inbox");
                                break;
                            }
                            string answer = Utility.Prompt("Approve or deny requests:\n'<Request Number> [Approve/Deny]'",clear:false);
                            if (string.IsNullOrWhiteSpace(answer))
                            { break;}
                            string[] answerArray = answer.Split(" ");
                            if (answerArray.Length < 2)
                            {
                                Utility.Error("Input seems incorrect, did you follow the pattern: \n'<Request Number> [Approve/Deny]'");
                                break;
                            }
                            int.TryParse(answerArray[0], out choice);
                            if (ts.trades[choice].TradeState != TradeStatus.Pending)
                            {
                                Utility.Error("Chosen request is already resolved");
                                break;
                            }
                            if (answerArray[1].ToLower() == "approve")
                                {
                                    ts.trades[choice].TradeState = TradeStatus.Approved;
                                    foreach (Item item in ts.items)
                                    {
                                        if (ts.trades[choice].Items.Contains(item))
                                        {
                                            if (item.Owner == ts.trades[choice].Caller)
                                            {
                                                item.Owner = ts.trades[choice].Responder;
                                            }
                                            else if (item.Owner == ts.trades[choice].Responder)
                                            {
                                                item.Owner = ts.trades[choice].Caller;
                                            }
                                        }
                                    }
                                }
                                else if (answerArray[0].ToLower() == "deny")
                                {
                                    ts.trades[choice].TradeState = TradeStatus.Denied;
                                    foreach (Item item in ts.trades[choice].Items)
                                    {
                                        item.Available = true;
                                    }
                                }
                            Console.ReadLine();
                            break;
                        case 3: // Completed Requests
                            check = false;
                            int ctr1 = 0;
                            foreach (Trade trade in ts.trades)
                            {
                                if (trade.TradeState != TradeStatus.Pending)
                                {
                                    if (trade.Caller == active_user || trade.Responder == active_user)
                                    {
                                        if (ctr1 % 2 == 0)
                                        { Console.ForegroundColor = ConsoleColor.Cyan; }
                                        else
                                        { Console.ForegroundColor = ConsoleColor.Magenta; }
                                        check = true;
                                        Console.WriteLine(trade.Info());
                                        ctr1++;
                                    }
                                }
                            }
                            if (!check)
                            {
                                Utility.Error("No Completed Requests..");
                                break;
                            }
                            Console.ReadLine();
                            break;
                        case 4: // Quit
                        default:
                            break;
                    }
                    break;
                case 3: // Browse items
                    Console.Clear();
                    Console.WriteLine("     ---Available Items---");
                    int ctr = 0;
                    foreach (Item item in ts.items)
                    {
                        if (item.Owner != active_user && item.Available)
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
                    break;
                case 4:
                    currentMenu = Menu.Main;
                    break;
                default:
                    Utility.Error("An unexpected error occurred. Returning to menu.");
                    break;
            }
            break;
        case Menu.Main:
            Utility.GenerateMenu(title: "     ---Main Menu---\nCurrent logged in user: " + active_user.Name,
                                    choices: new[] { "Trade", "Add Item to Market", "View Your Items", "Log out" });
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
                    ts.FileWrite(ts.itemsPath, toExport: itemToExport);

                    ts.items.Add(newItem);
                    Utility.Success($"Item Added: \n{newItem.Info()}");
                    currentMenu = Menu.Main;
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("     ---Your Items---");
                    int ctr = 0;
                    foreach (Item item in ts.items)
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