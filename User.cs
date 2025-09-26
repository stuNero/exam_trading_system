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
    public string Info(bool inclPassword = false)
    {
        string txt = $"Name: {Name}\nEmail: {Email}";
        if (inclPassword)
        {
            txt += "Password: " + _password;
        }
        return txt;
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
        _password = TryPassword();
    }
    public bool TryLogin(string? email, string? password)
    {
        return Email == email && _password == password;
    }
}