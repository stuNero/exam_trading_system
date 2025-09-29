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
            txt += "\nPassword: " + _password;
        }
        return txt;
    }
    public bool HasPassword()
    {
        return !string.IsNullOrEmpty(_password);
    }
    public void SetPassword()
    {
        string TryPassword()
        {
            string attempt1 = Utility.Prompt("Input password: ");
            if (string.IsNullOrWhiteSpace(attempt1)) { return attempt1; }

            string attempt2 = Utility.Prompt("Confirm password: ");
            if (string.IsNullOrWhiteSpace(attempt2)) { return attempt2; }

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