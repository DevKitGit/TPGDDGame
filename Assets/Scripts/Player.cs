using UnityEngine;
using UnityEngine.InputSystem;
public class Player : Unit
{
    private PlayerInput _playerInput;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }
    public override void OnTurnEnd()
    {
    }
    public override void OnTurnStart()
    {
    }
}