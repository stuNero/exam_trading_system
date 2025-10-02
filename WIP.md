string ParseEmail(string email)
{
   if (!email.Contains("@"))
    {
        Utility.Error($"{email} is not a valid email format");
        return "";
    }
        return email;
}


            // Console.Clear();
            // Console.Write("Enter email:");
            // string? tryEmail = Console.ReadLine();
            // Console.Write("Enter password:");
            // string? password = Console.ReadLine();
            // bool check = false;
            // foreach (User user in users)
            // {
            //     if (user.Email == tryEmail)
            //     {
            //         if (user.TryLogin(tryEmail, password))
            //         {
            //             Utility.Success("Login success\nWelcome " + user.Name);
            //             currentMenu = Menu.Main;
            //             break;
            //         }
            //     }
            // }
            // Utility.Error("Incorrect Email or Password");
            // currentMenu = Menu.Main;
            
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
                    Console.WriteLine("Which item would you like to buy?");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("Item Name: ");
                    Console.ResetColor();
                    string? itemToTrade = Console.ReadLine();

                    check = false;
                    foreach (Item item in items)
                    {
                        if (item.Owner != active_user && item.Name.ToLower() == itemToTrade.ToLower() && item.Available)
                        {
                            trades.Add(new Trade(item.Owner, active_user, new List<Item> { item }));

                            Utility.Success($"Trade request sent to {item.Owner}");
                            check = true;
                        }
                    }
                    if (!check)
                    { Utility.Error($"Item: {itemToTrade} does not exist.."); break;}