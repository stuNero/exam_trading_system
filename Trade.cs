using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace App;

class Trade
{
    public User? Sender;
    public User? Reciever;
    public List<Item>? Items;
    public TradeStatus? TradeState;
    public Trade(User sender, User reciever, List<Item> items)
    {
        Sender = sender;
        Reciever = reciever;
        Items = items;
        TradeState = TradeStatus.Pending;
    }
    public string Info()
    {
        string senderItems = "";
        string recieverItems = "";
        senderItems += Sender.Name + "'s items: ";
        foreach (Item item in Items)
        {
            if (item.Owner == Sender)
            {
                senderItems += $"\n{item.Name}";
            }
            else if (item.Owner == Reciever)
            {
                recieverItems += $"\n{item.Name}";
            }
        }

        return senderItems + "\n" + recieverItems + $"\nStatus: [{TradeState}]";
    }
}