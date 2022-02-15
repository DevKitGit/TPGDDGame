using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CombatBoardManager : MonoBehaviour, PlayerControls.ICombatActions
{
    public PlayerInputManager _playerInputManager;
    private PlayerInput _activePlayer;
    private Tilemap _tileMap;
    private PlayerControls PlayerControls;
    public List<CombatCell> board;

    private void Start()
    {
        CreateBoard();
        _playerInputManager.onPlayerJoined += SetActivePlayer;
        PlayerControls.Combat.SetCallbacks(this);
    }
    
    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += SetActivePlayer;
        PlayerControls = new PlayerControls();

        PlayerControls.Enable();
    }

    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= SetActivePlayer;
        PlayerControls.Disable();
    }
    
    private void SetActivePlayer(PlayerInput playerInput)
    {
        if (!_activePlayer.inputIsActive)
        {
            _activePlayer = playerInput;
        }
    }

    private void MovePointer(InputAction.CallbackContext context)
    {
        //Move in pointed hexagon style. 
        var value = context.ReadValue<Vector2>();
        Debug.Log(value);
    }
    private void CreateBoard()
    {
        _tileMap = GetComponentInChildren<Tilemap>();
        board = new List<CombatCell>(_tileMap.size.x*_tileMap.size.y);
        for (int i = 0; i < _tileMap.size.x; i++)
        {
            for (int j = 0; j < _tileMap.size.y; j++)
            {
                board.Add(new CombatCell(true,1f));
            }
        }
    }
    public enum Pattern
    {
        Star = 0,
        Box = 1,
        Cone = 2
    }
    
    public CombatCell GetCell(int row, int col)
    {
        if (row >= _tileMap.size.y || col >= _tileMap.size.x || row < 0 || col < 0)
        {
            return null;
        }
        return board[col + _tileMap.size.y*row];
    }

    public CombatCell GetCell(Vector3Int cellPos)
    {
        cellPos = CellIndex(cellPos);
        if (cellPos.y >= _tileMap.size.y || cellPos.x >= _tileMap.size.x || cellPos.y < 0 || cellPos.x < 0)
        {
            return null;
        }
        return board[cellPos.y + _tileMap.size.y*cellPos.x];
    }

    
    public List<CombatCell> GetNeighbours(int row, int col, Pattern pattern = Pattern.Box, int radius = 1)
    {
        var neighbours = new List<CombatCell>();
        for (int i = -1; i < 1; i++)
        {
            for (int j = -1; j < 1; j++)
            {
                if (pattern == Pattern.Box)
                {
                    
                }
            }
        }
        return neighbours;
    }
    
    private Vector3Int CellIndex(Vector3Int cellPos)
    {
        return cellPos - _tileMap.origin;
    }

    public void OnGridMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var Direction = context.ReadValue<Vector2>();
            //Debug.Log(Direction.ToString());
        }
    }

    public void OnAbilityWest(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Ability West");
        }
    }

    public void OnAbilityEast(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Ability East");
        }
    }

    public void OnAbilitySouth(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Ability South");
        }
    }

    public void OnAbilityNorth(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Ability North");
        }

    }

    public void OnConsumableLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Consumable left");
        }
    }

    public void OnConsumableRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Consumable right");
        }
    }

    public void OnConsumableDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Consumable down");
        }
    }

    public void OnConsumableUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Consumable up");
        }
    }
}