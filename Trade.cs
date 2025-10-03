using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace App;

class Trade
{
    public User Caller;
    public User Responder;
    public List<Item> Items;
    public TradeStatus TradeState;
    public Trade(User caller, User responder, List<Item> items)
    {
        Caller = caller;
        Responder = responder;
        Items = items;
        TradeState = TradeStatus.Pending;
    }
    public string Info()
    {
        string callerItems      = $"{Caller.Name}'s items: ";
        string responderItems   = $"{Responder.Name}'s items: ";
        foreach (Item item in Items)
        {
            if (item.Owner == Caller)
            {
                callerItems += $"\n - {item.Name}\n";
            }
            else if (item.Owner == Responder)
            {
                responderItems += $"\n - {item.Name}\n";
            }
        }
        return callerItems + "\n" +
               responderItems + $"\nStatus: [{TradeState}]";
    }
}