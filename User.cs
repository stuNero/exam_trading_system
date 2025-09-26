using System.Dynamic;

namespace App;

class User
{
    public string? Name;
    public string? Email;
    string? _password;

    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }
    public void SetPassword()
    {
        string TryPassword()
        {
            Console.Write("Input password: ");
            string? attempt1 = Console.ReadLine();
            Console.Write("Confirm password: ");
            string? attempt2 = Console.ReadLine();
            if (attempt1 != attempt2)
            {
                Utility.Error("Passwords not matching\nTry again..");
                return TryPassword();
            }
            return attempt1;
        }
        TryPassword();
    }
    public bool TryLogin(string email, string password)
    {
        return Email == email && _password == password;
    }
}