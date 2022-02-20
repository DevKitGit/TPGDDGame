using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : Unit
{
    public override Intent Intent { get; set; } = null;
    public override TaskCompletionSource<bool> Tcs { get; set; }
    private PlayerInput _playerInput;

    
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        
    }


    public void EndTurn()
    {
        _playerInput.DeactivateInput();
        Tcs.TrySetResult(true);
    }

    public override Task<bool> DoTurn()
    {
        _playerInput.ActivateInput();
        Tcs = new TaskCompletionSource<bool>();
        return Tcs.Task;
    }
    
}