namespace App;

class User
{
    string Name;
    string Email;
    string _password;

    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }
    public bool TryLogin(string email, string password)
    {
        return Email == email && _password == password;
    }
}