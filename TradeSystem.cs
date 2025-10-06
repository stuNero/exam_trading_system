namespace App;
/// <summary>
/// Represents the main programs objects, User, Item, Trade lists and methods for manipulating these.
/// </summary>
class TradeSystem
{
    // Declaring directories for program information storage
    public string usersPath = "users.csv";
    public string itemsPath = "items.csv";
    public string tradesPath = "trades.csv";

    // Declaring lists to be used for each class
    public List<User> users = new List<User>();
    public List<Item> items = new List<Item>();
    public List<Trade> trades = new List<Trade>();
    
    public TradeSystem()
    {
        // Reads file, creates one if it doesn't exist
        List<string[]> userFileLines = FormatFileRead(usersPath);
        // Updates the list based on file content
        UpdateUsers(userFileLines);

        List<string[]> itemFileLines = FormatFileRead(itemsPath);
        UpdateItems(itemFileLines);

        List<string[]> tradeFileLines = FormatFileRead(tradesPath);
        UpdateTrades(tradeFileLines);
    }
    /// <summary>
    /// Formats .csv files lines of text into string arrays in a list
    /// </summary>
    /// <param name="path">file directory</param>
    /// <returns>List of all lines of text in the file as string arrays</returns>
    public List<string[]> FormatFileRead(string path)
    {
        // if file doesn't exist, create one with path name
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }
        // reads and assigns all lines as string arrays in a list
        string[] lines = File.ReadAllLines(path);
        List<string[]> formattedLines = new List<string[]>();
        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] formatLine = line.Split(",");
                formattedLines.Add(formatLine);
            }
        }
        return formattedLines;
    }
    /// <summary>
    /// Writes all User's from List users to .csv file in .csv format
    /// </summary>
    public void FileWriteUsers()
    {
        string txt = "";
        foreach (User user in users)
        {
            txt += $"{user.Email},{user._password},{user.Name}\n";
        }
        File.WriteAllText(usersPath, txt);
    }
    /// <summary>
    /// Writes all Item's from List items to .csv file in .csv format
    /// </summary>
    public void FileWriteItems()
    {
        string txt = "";
        foreach (Item item in items)
        {
            txt += $"{item.Name},{item.Description},{item.Owner.Email}\n";
        }
        File.WriteAllText(itemsPath, txt);
    }
    /// <summary>
    /// Writes all Trade's from List trades to .csv file in .csv format
    /// </summary>
    public void FileWriteTrades()
    {
        string txt = "";
        foreach (Trade trade in trades)
        {
            txt += $"{trade.Caller.Email},{trade.Responder.Email},{trade.TradeState.ToString()}";
            foreach (Item item in trade.Items)
            {
                txt += $",{item.Name}";
            }
            txt += "\n";
        }
        File.WriteAllText(tradesPath, txt);
    }
    /// <summary>
    /// Takes in List of lines of User variables as string arrays and adds to users list
    /// </summary>
    /// <param name="formattedLines">formatted lines of text from the FileWrite method</param>
    public void UpdateUsers(List<string[]> formattedLines)
    {
        foreach (string[] line in formattedLines)
        {
            // reads each element in the array where the first or 0 is the identifying (unique) string for the object
            // this is the same for every Update method
            string email = line[0];
            string password = line[1];
            string name = line[2];
            User newUser = new User(name, email);
            newUser.SetPassword(password, fromFile: true);
            users.Add(newUser);
        }
    }
    /// <summary>   
    /// Takes in List of lines of Item variables as string arrays and adds to items list
    /// </summary>
    /// <param name="formattedLines">formatted lines of text from the FileWrite method</param>
    public void UpdateItems(List<string[]> formattedLines)
    {
        foreach (string[] line in formattedLines)
        {
            string name = line[0];
            string desc = line[1];
            User? owner = null;
            foreach (User user in users)
            {
                if (line[2] == user.Email)
                {
                    owner = user;
                    break;
                }
            }
            items.Add(new Item(name, desc, owner));
        }
    }
    /// <summary>
    /// Takes in List of lines of Trade variables as string arrays and adds to trades list
    /// </summary>
    /// <param name="formattedLines">formatted lines of text from the FileWrite method</param>
    public void UpdateTrades(List<string[]> formattedLines)
    {
        // For each trade in the read file
        foreach (string[] line in formattedLines)
        {
            List<Item> tradeItems = new List<Item>();

            User[] traders = new User[2];

            string senderEmail = line[0];
            string recieverEmail = line[1];
            string tradeState = line[2];

            // Assigns users based on their identifying email
            foreach (User user in users)
            {
                if (user.Email == senderEmail)
                {
                    traders[0] = user;
                }
                if (user.Email == recieverEmail)
                {
                    traders[1] = user;
                }
            }
            // Assigns items if they correspond to those already in the system
            for (int i = 3; i < line.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (line[i] == item.Name)
                    {
                        tradeItems.Add(item);
                    }
                }
            }
            Trade newTrade = new Trade(traders[0], traders[1], tradeItems);
            // TradeStatus is saved as a string, enum.parse translates from string to enum value
            newTrade.TradeState = Enum.Parse<TradeStatus>(tradeState);
            trades.Add(newTrade);
        }
    }
    /// <summary>
    /// Adds new User to to List users and writes to specific file 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    public void RegisterUser(string name, string email)
    {
        // checks if the email is already registered
        bool check = false;
        foreach (User user in users)
        {
            if (user.Email == email)
            { check = true; }
        }
        // if not, registering continues
        if (!check)
        {
            User? newUser = new User(name, email);
            newUser.SetPassword();
            if (string.IsNullOrWhiteSpace(newUser._password))   
            { Utility.Error("Password cannot be empty"); }      
            else
            {
                users.Add(newUser);
                FileWriteUsers();
                Utility.Success($"Account registered!\nAccount details:\n{newUser.Info(inclPassword: true)}");
            }
        }
        else
        { Utility.Error($"Email: '{email}' is already taken"); }
    }
}