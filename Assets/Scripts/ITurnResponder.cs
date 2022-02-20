using System.Threading.Tasks;


public interface ITurnResponder
{
    bool TurnDone { get; set; }
    Intent Intent { get; set; }

    //Typically turn priority is decided by agility.
    int TurnPriority { get; set; }
    public TaskCompletionSource<bool> Tcs { get; set; }
    
    public Task<bool> DoTurn();
    
}