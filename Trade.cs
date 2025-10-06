using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace App;

/// <summary>
/// Represents an instance of a particular trade with the caller and responder of the trade,
/// a list of the items to trade and what the current trade status is (Pending, Approved or Denied)
/// </summary>
/// <example>
/// <code>
/// Trade trade = new Trade(user1, user2, itemsToTrade)
/// </code>
/// </example>
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
    /// <summary>
    /// Returns formatted string of the trade's different variables. 
    /// </summary>
    /// <returns>Formatted string text</returns>
    public string Info()
    {
        string callerItems = $"{Caller.Name}'s items: ";
        string responderItems = $"{Responder.Name}'s items: ";
        // Structures output as a list of items for each owner
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
               responderItems
               + $"\nStatus: [{TradeState}]";
    }
}