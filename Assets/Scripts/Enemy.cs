
using System.Threading.Tasks;

public class Enemy : Unit
{
    public override Intent Intent { get; set; }
    public override TaskCompletionSource<bool> Tcs { get; set; }

    public void EndTurn()
    {
        Tcs.TrySetResult(true);
    }
    public override Task<bool> DoTurn()
    {
        Tcs = new TaskCompletionSource<bool>();
        return Tcs.Task;
    }
}
