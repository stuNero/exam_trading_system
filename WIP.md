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