namespace App;

class TradeSystem
{
    public string usersPath = "users.csv";
    public string itemsPath = "items.csv";
    public string tradesPath = "trades.csv";
    public List<User> users = new List<User>();
    public List<Item> items = new List<Item>();
    public List<Trade> trades = new List<Trade>();

    public TradeSystem()
    {
        List<string[]> userFileLines = FormatFileRead(usersPath);
        UpdateUsers(userFileLines);

        List<string[]> itemFileLines = FormatFileRead(itemsPath);
        UpdateItems(itemFileLines);

        List<string[]> tradeFileLines = FormatFileRead(tradesPath);
        UpdateTrades(tradeFileLines);
    }

    public List<string[]> FormatFileRead(string path)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }
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
    public void UpdateUsers(List<string[]> formattedLines)
    {
        foreach (string[] line in formattedLines)
        {
            string email = line[0];
            string password = line[1];
            string name = line[2];
            User newUser = new User(name, email);
            newUser.SetPassword(password, fromFile: true);
            users.Add(newUser);
        }
    }
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
    public void UpdateTrades(List<string[]> formattedLines)
    {
        foreach (string[] line in formattedLines)
        {
            List<Item> tradeItems = new List<Item>();

            User[] traders = new User[2];

            string senderEmail = line[0];
            string recieverEmail = line[1];
            string tradeState = line[2];
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
            newTrade.TradeState = Enum.Parse<TradeStatus>(tradeState);
            trades.Add(newTrade);
        }
    }
    public void FileWrite(string path, params string[] toExport)
    {
        List<string[]> formattedLines = FormatFileRead(path);
        bool check = false;
        foreach (string[] line in formattedLines)
        {
            if (line.Contains(toExport[0]))
            { check = true; }
        }
        if (!check)
        {
            using (StreamWriter writer = File.AppendText(path))
            {
                string txt = "";
                for (int i = 0; i < toExport.Length; i++)
                {
                    txt += toExport[i] + ",";
                }
                txt = txt.Substring(0, txt.Length - 1);
                writer.WriteLine(txt);
            }
        }
    }
    public void RegisterUser(string? name, string? email)
    {
        bool check = false;
        foreach (User user in users)
        {
            if (user.Email == email)
            { check = true; }
        }
        if (!check)
        {
            User? newUser = new User(name, email);
            newUser.SetPassword();
            if (string.IsNullOrWhiteSpace(newUser._password))
            { Utility.Error("Password cannot be empty"); }
            else
            {
                users.Add(newUser);

                string[] userToExport = new string[3];
                userToExport[0] = newUser.Email;
                userToExport[1] = newUser._password;
                userToExport[2] = newUser.Name;

                FileWrite(usersPath, toExport: userToExport);
                Utility.Success($"Account registered!\nAccount details:\n{newUser.Info(inclPassword: true)}");
            }
        }
        else
        { Utility.Error($"Email: '{email}' is already taken"); }
    }
}