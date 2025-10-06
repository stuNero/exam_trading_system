using System.Dynamic;

namespace App;
/// <summary>
/// Represents a user of the program, handles login password registration
/// </summary>
/// <example>
/// <code>
/// User user = new User("John Doe", "John@hotmail.com")
/// </code>
/// </example>
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
    /// <summary>
    /// Returns a formatted string of the user object's information
    /// </summary>
    /// <param name="inclPassword">Boolean to choose if password should be included in return string</param>
    /// <returns>Formatted information string</returns>
    public string Info(bool inclPassword = false)
    {
        string txt = $"Name: {Name}\nEmail: {Email}";
        if (inclPassword)
        {
            txt += "\nPassword: " + _password;
        }
        return txt;
    }
    /// <summary>
    /// Sets the user's password by prompt or directly(fromFile)
    /// </summary>
    /// <remarks>
    /// When fromFile is false this method prompts the users input either until it cancels the 
    /// prompt or both the passwords match
    /// </remarks>
    /// <param name="password">default value for if fromFile == true</param>
    /// <param name="fromFile">if false prompts the console user for password, if true it takes in password parameter</param>
    public void SetPassword(string password = "", bool fromFile = false)
    {
        // Helper method for checking password matching
        string TryPassword(string password)
        {
            // using prompt and isnullorwhitespace for ability to cancel mid
            password = Utility.Prompt("Input password: ");
            if (string.IsNullOrWhiteSpace(password)) { return password; }

            string confirm = Utility.Prompt("Confirm password: ");
            if (string.IsNullOrWhiteSpace(confirm)) { return confirm; }

            // using recursion until passwords match or cancelled
            if (password != confirm)
            {
                Utility.Error("Passwords not matching\nTry again..");
                return TryPassword(password);
            }
            return password;
        }
        // if fromfile it reads the input password string, skips user input
        if (fromFile)
        { _password = password; return; }

        _password = TryPassword(password);
    }
    /// <summary>
    /// Checks if input email is user's email and if input password is user's password
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns>Boolean value, false if credentials don't match, true if they do</returns>
    public bool TryLogin(string? email, string? password)
    {
        return Email == email && _password == password;
    }
}