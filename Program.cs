using App;
Console.Clear();

List<User> users = new List<User>();

Menu currentMenu = Menu.Main;

while (true)
{
    switch (currentMenu)
    {
        case Menu.Main:
            Utility.GenerateMenu(title: "Main Menu", "Login", "Register Account");
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
                    currentMenu = Menu.Main;
                    break;
            }
            break;
        case Menu.Login:

            break;
        case Menu.RegisterAccount:
            Console.WriteLine("Register Account:");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            Console.Write("Enter email: ");
            string email = Console.ReadLine();
            users.Add(new User(username, email));
            currentMenu = Menu.Main;
            break;
        default:
            Utility.Error("An unexpected error occurred. Returning to main menu.");
            currentMenu = Menu.Main;
            break;
    }
}