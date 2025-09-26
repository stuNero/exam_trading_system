using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;
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
            Console.Write("Enter name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter email: ");
            string? email = Console.ReadLine();

            RegisterUser(name, email);

            Console.Clear();
            currentMenu = Menu.Main;
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
        Utility.Success($"Account registered!\nAccount details:\n {newUser.Info(inclPassword:true)}");
    }
}