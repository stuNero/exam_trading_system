using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;
using App;

List<User> users = new List<User>();

Menu currentMenu = Menu.None;

User active_user = null;

while (true)
{
    Console.Clear();
    switch (currentMenu)
    {
        case Menu.None:
            Utility.GenerateMenu(title:"Welcome to Stratholme Trading",choices: new[] { "Login", "Register Account", "Quit" });
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
            Console.WriteLine("--Register Account--");
            Console.Write("Enter name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter email: ");
            string? email = Console.ReadLine();

            RegisterUser(name, email);

            currentMenu = Menu.None;
            break;
        case Menu.Login:
            Console.WriteLine("--Login-- ");
            Console.Write("Email:");
            string? loginEmail = Console.ReadLine();
            Console.Write("Password:");
            string? loginPassword = Console.ReadLine();
            bool check = false;
            foreach (User user in users)
            {
                if (user.TryLogin(loginEmail, loginPassword))
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
                Utility.GenerateMenu(title: "--Main Menu--", choices: new[] { "Trade", "View Your Items", "Log out" });
                int.TryParse(Console.ReadLine(), out int input);
                switch (input)
                {
                    case 1:

                        break;
                    case 2:

                        break;
                    case 3:
                        Utility.Success($"User {active_user.Name} logged out.");
                        active_user = null;
                        currentMenu = Menu.None;
                        break;
                    case 4:
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
        User newUser = new User(name, email);
        newUser.SetPassword();
        users.Add(newUser);
        Utility.Success($"Account registered!\nAccount details:\n{newUser.Info(inclPassword:true)}");
    }
}