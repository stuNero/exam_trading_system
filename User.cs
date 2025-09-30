using System.Dynamic;

namespace App;

class User
{
    public string? Name;
    public string? Email;
    public string? _password;

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
        return !string.IsNullOrWhiteSpace(_password);
    }
    public void SetPassword(string password = "", bool fromFile = false)
    {
        string TryPassword(string password)
        {
            password = Utility.Prompt("Input password: ");
            if (string.IsNullOrWhiteSpace(password)) { return password; }

            string confirm = Utility.Prompt("Confirm password: ");
            if (string.IsNullOrWhiteSpace(confirm)) { return confirm; }

            if (password != confirm)
            {
                Utility.Error("Passwords not matching\nTry again..");
                return TryPassword(password);
            }
            return password;
        }
        if (fromFile)
        { _password = password;}
        else
        {_password = TryPassword(password);}
    }
    public bool TryLogin(string? email, string? password)
    {
        return Email == email && _password == password;
    }
}