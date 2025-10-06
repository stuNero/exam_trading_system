using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading.Channels;
using App;

// declaring tradesystem object that stores all users, trades and items and reads/writes to file
TradeSystem ts = new TradeSystem();

Menu currentMenu = Menu.None;
// Using a reference User object to save the active user
User? active_user = null;
while (true)
{
    Console.Clear();
    switch (currentMenu)
    {
        // Default menu
        case Menu.None:
            Utility.GenerateMenu(title: "     ---Welcome to Gadgetzan Trading---",
                                 choices: new[] { "Login", "Register Account", "Quit" });
            int.TryParse(Console.ReadLine(), out int choice);
            switch (choice)
            {
                case 1: // Login menu
                    currentMenu = Menu.Login;
                    break;
                case 2: // Register menu
                    currentMenu = Menu.RegisterAccount;
                    break;
                case 3: // Quits program
                    Environment.Exit(0);
                    break;
                default:
                    Utility.Error("Invalid choice, please try again.");
                    break;
            }
            break;
        case Menu.RegisterAccount:
            Console.WriteLine("     --Register Account--");
            // uses .Prompt user input with IsnullorWhitespace for canceling 
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
            // uses .Prompt user input with IsnullorWhitespace for canceling 
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
                    // prints all users with a number based on placement on list
                    for (int i = 0; i < ts.users.Count; i++)
                    {
                        if (ts.users[i] != active_user)
                        {
                            Console.WriteLine($"\n[{i}] \n{ts.users[i].Info()}\n________________________");
                        }
                    }
                    string inputString = Utility.Prompt("Input user's list number: ",clear:false);
                    if (string.IsNullOrWhiteSpace(inputString))
                    {
                        break;
                    }
                    int.TryParse(inputString, out choice); // Converts users list index string to int

                    User? tradePartner = ts.users[choice]; // saves users choice as an object reference based on list index

                    List<Item>? tradeItems = new List<Item>();
                    
                    while (true)    // Starts loop for user to add multiple items to trade
                    {
                        Console.WriteLine(tradePartner.Name + "'s items:");
                        foreach (Item? item in ts.items)
                        {   // only checks if the item isn't already in tradelist and it's the tradepartners items
                            if (item.Owner == tradePartner && !tradeItems.Contains(item))
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
                                if (item.Owner == active_user && !tradeItems.Contains(item))
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
                                    tradeItems.Add(item);
                                }
                            }
                        }
                    }
                    // only make a trade if items are selected
                    if (tradeItems.Count() != 0)
                    {
                        Trade trade = new Trade(active_user, tradePartner, tradeItems);

                        ts.trades.Add(trade);
                        ts.FileWriteTrades();
                        Utility.Success("'Request sent.'");
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
                            Console.Clear();
                            Utility.GenerateMenu(title:"     ---Sent Requests---");
                            check = false;
                            // Only prints trades that are pending and trade callers
                            for (int i = 0; i < ts.trades.Count; i++)
                            {
                                if (ts.trades[i].Caller == active_user && ts.trades[i].TradeState == TradeStatus.Pending)
                                {
                                    check = true;
                                    Console.WriteLine($"[{i}] Request to {ts.trades[i].Responder.Name}\n" + ts.trades[i].Info() + "\n_________________");
                                }
                            }
                            if (!check)
                            {
                                Utility.Error("No requests in inbox");
                                break;
                            }
                            Utility.Success("");
                            break;
                        case 2: // Recieved
                            Console.Clear();
                            Utility.GenerateMenu("     ---Recieved Requests---");
                            check = false;

                            // only prints trades that are pending and is the trade responders
                            for (int i = 0; i < ts.trades.Count; i++)
                            {
                                if (ts.trades[i].Responder == active_user && ts.trades[i].TradeState == TradeStatus.Pending)
                                {
                                    check = true;
                                    Console.WriteLine($"[{i}] Request from {ts.trades[i].Caller.Name}\n" + ts.trades[i].Info() + "\n_________________");
                                }
                            }
                            if (!check)
                            {
                                Utility.Error("No requests in inbox");
                                break;
                            }

                            string answer = Utility.Prompt("Approve or deny requests:\n'<Request Number> ['Approve'/'Deny']'\nEXAMPLE: '1 Approve'\n",clear:false);
                            if (string.IsNullOrWhiteSpace(answer))
                            { break;}

                            // splits the string to 2 elements where the first should be a number and the second a string
                            string[] answerArray = answer.Split(" ");
                            if (answerArray.Length != 2)
                            {
                                Utility.Error("Input seems incorrect, did you follow the pattern: \n'<Request Number> ['Approve'/'Deny']'");
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
                                // If approved, items switches owners
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
                                Utility.Success("Request Approved\n'Time is money, friend!'");
                            }
                            else if (answerArray[1].ToLower() == "deny")
                            {
                                ts.trades[choice].TradeState = TradeStatus.Denied;
                                Utility.Success("Request Denied\n'Don't waste my time!'");
                            }
                            ts.FileWriteTrades();
                            ts.FileWriteItems();
                            break;
                        case 3: // Completed Requests
                            Console.Clear();
                            Utility.GenerateMenu("     ---Completed Requests---");
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
                            Console.ResetColor();
                            if (!check)
                            {
                                Utility.Error("No Completed Requests..");
                                break;
                            }
                            Utility.Success("");
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
                    Utility.Success("");
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
            Utility.GenerateMenu(title: "     ---Main Menu---\nLogged in as: " + active_user.Name,
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
                    // uses .Prompt user input with IsnullorWhitespace for canceling 
                    string itemName = Utility.Prompt("Item name: ");
                    if (string.IsNullOrWhiteSpace(itemName)) { currentMenu = Menu.Main; break; }

                    string itemDesc = Utility.Prompt("Item description: ");
                    if (string.IsNullOrWhiteSpace(itemDesc)) { currentMenu = Menu.Main; break; }

                    Item newItem = new Item(itemName, itemDesc, active_user);
                    ts.items.Add(newItem);
                    // overwrites file with new items added to items list
                    ts.FileWriteItems();
                    Utility.Success($"Item Added: \n{newItem.Info()}");
                    currentMenu = Menu.Main;
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("     ---Your Items---");
                    int ctr = 0;
                    foreach (Item item in ts.items)
                    {
                        // cycles colors for better visibility in print
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
                    Utility.Success("");
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