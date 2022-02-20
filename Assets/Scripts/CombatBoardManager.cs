using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class CombatBoardManager : MonoBehaviour, PlayerControls.ICombatActions
{
    
    [Header("Player Input")]
    public PlayerControls playerControls;
    public PlayerInputManager playerInputManager;
    public List<PlayerInput> playerInputs;
    public PlayerInput activePlayer;
    public bool pointerMode = false;
    private float _timeSinceLastMove;
    private Vector2 moveValue = Vector2.zero;
    private bool moveActive = false;
    
    [SerializeField, Range(0,1)] private float _horitonzalBorder = 0.15f;
    [SerializeField, Range(0,1)] private float _verticalBorder =  0.25f;

    [SerializeField,Range(1,30)] private float _inputPollRate = 10;
    
    [Header("Grid")]
    private Tilemap _tileMap;
    public List<Node> board;
    private Vector2Int _tileMapDimensions;
    private Vector3Int currentlyHoveredTile;
    private List<GameObject> _pathIndicators = new List<GameObject>();
    public GameObject _pathIndicatorPrefab;
    private static Vector3Int
        DEG0 = new(1, 0, 0),
        DEG60_EVEN = new(1, 1, 0),
        DEG60_ODD = new(0, 1, 0),
        DEG120_EVEN = new(0, 1, 0),
        DEG120_ODD = new(-1, 1, 0),
        DEG180 = new(-1, 0, 0),
        DEG240_EVEN = new(0, -1, 0),
        DEG240_ODD = new(-1, -1, 0),
        DEG300_EVEN = new(1, -1, 0),
        DEG300_ODD = new(0, -1, 0);


    private static Vector3Int[] oddDirs = 
        {DEG0, DEG60_ODD, DEG120_ODD, DEG180, DEG240_ODD, DEG300_ODD};
    private static Vector3Int[] evenDirs = 
        {DEG0, DEG60_EVEN, DEG120_EVEN, DEG180, DEG240_EVEN, DEG300_EVEN};

    private Pathfinding _pathfinder;


    private void Start()
    {
        _timeSinceLastMove = Time.time;
        playerControls.Combat.SetCallbacks(this);
        CreateBoard();
        
    }
    
    private void RegisterUser(PlayerInput playerInput)
    {
        activePlayer = playerInput;
        playerInputs.Add(playerInput);
        playerInput.gameObject.name = "Player." + playerInputs.Count;
        playerInput.gameObject.transform.position = _tileMap.GetCellCenterWorld(_tileMap.origin);
        currentlyHoveredTile = _tileMap.WorldToCell(playerInput.gameObject.transform.position);
    }
    private void DeregisterUser(PlayerInput playerInput)
    {
        
    }
    private void OnEnable()
    {
        playerInputManager.playerJoinedEvent.AddListener(RegisterUser);
        playerInputManager.playerLeftEvent.AddListener(DeregisterUser);
        playerControls = new PlayerControls();
        playerControls.Enable();
        
    }
    private void OnDisable()
    {
        playerInputManager.playerJoinedEvent.RemoveListener(RegisterUser);
        playerInputManager.playerLeftEvent.RemoveListener(DeregisterUser);
        playerControls.Disable();
    }
    private void SetActivePlayer(PlayerInput playerInput)
    {
        
        if (activePlayer != null && !activePlayer.inputIsActive)
        {
            activePlayer = playerInput;
        }
    }
    private void CreateBoard()
    {
        _tileMap = GetComponentInChildren<Tilemap>();
        _tileMap.CompressBounds();
        _tileMapDimensions = new Vector2Int(_tileMap.size.x, _tileMap.size.y);
        board = new List<Node>(_tileMapDimensions.x*_tileMapDimensions.y);
        _tileMap.origin.Set(Mathf.CeilToInt(_tileMapDimensions.x/2f)+1,Mathf.CeilToInt(_tileMapDimensions.y/2f)+1,0);
        //Make the pathfinding nodes

        for (int y = 0; y < _tileMapDimensions.y; y++)
        {
            for (int x = 0; x < _tileMapDimensions.x; x++)
            {
                Debug.Log((x,y));
                board.Add(new CombatNode(new Vector3Int(x,y,0),true,1f));
            }
        }
        _pathfinder = new Pathfinding(board, _tileMapDimensions);
        
    }
    public Node GetCell(int row, int col)
    {
        return InBounds(new Vector3Int(col, row)) ? null : board[col + _tileMapDimensions.y*row];
    }

    public Node GetCell(Vector3Int cellPos)
    {
        return InBounds(cellPos) ? null : board[cellPos.y + _tileMapDimensions.y*cellPos.x];
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var value = context.ReadValue<Vector2>();
            moveValue = value;
            moveActive = value.magnitude > 0;
        }
    }

    private void FixedUpdate()
    {
        movePlayer();
    }

    private void movePlayer()
    {
        if (moveActive && Time.time - _timeSinceLastMove > 1f/_inputPollRate)
        {
            var playerThatMoved = FindPlayer(activePlayer.devices[0]);
            var activePlayerCurrentIndex = _tileMap.WorldToCell(activePlayer.gameObject.transform.position);
            /*var direction = InterpolateGridDirection(activePlayerCurrentIndex, moveValue);
            var resultDirection = activePlayerCurrentIndex + direction;*/
            var direction = InterpolateGridDirection(currentlyHoveredTile, moveValue);
            var resultDirection = currentlyHoveredTile + direction;
            if (!InBounds(resultDirection))
            {
                displayMoveDenied();
            }
            else
            {
                if (_pathIndicators.Count > 0)
                {
                    _pathIndicators.ForEach(indicator => Destroy(indicator));

                }
                currentlyHoveredTile = resultDirection;
                
                var path = _pathfinder.FindPath(activePlayerCurrentIndex, currentlyHoveredTile);
                _pathIndicators.Clear();
                var debugString = "";
                foreach (var node in path)
                {
                    _pathIndicators.Add(GameObject.Instantiate(_pathIndicatorPrefab, _tileMap.CellToWorld(node.Position),quaternion.identity));
                    debugString += node.Position;
                }
                //Debug.Log(debugString);
                //var hoveredTile = resultDirection;
                
                //activePlayer.gameObject.transform.position = _tileMap.CellToWorld(activePlayerCurrentIndex + direction);
                /*Debug.Log($"input is: {moveValue}, Moving in: {direction}, from {activePlayerCurrentIndex} to {_tileMap.WorldToCell(activePlayer.gameObject.transform.position)}");
                */
                _timeSinceLastMove = Time.time;
            }
        }
    }
    private void displayMoveDenied()
    {
    }

    public Vector3Int InterpolateGridDirection(Vector3Int origin, Vector3 inputDirection)
    {
        inputDirection.Normalize();

        var horizontal = math.abs(inputDirection.x) > _horitonzalBorder ? Math.Sign(inputDirection.x) : 0;
        var vertical = math.abs(inputDirection.y) > _verticalBorder ? Math.Sign(inputDirection.y) : 0;
        
        var isRowOdd = origin.y % 2 == 0;
        //if a direction is diagonal
        if (Mathf.Abs(horizontal) + math.abs(vertical) > 1f) 
        {
            if (horizontal+vertical==2)
            {
                return isRowOdd ? DEG60_ODD : DEG60_EVEN;
            } 
            if (horizontal+vertical==-2)
            {
                return isRowOdd ? DEG240_ODD : DEG240_EVEN;
            }if (horizontal < 0)
            {
                return isRowOdd ? DEG120_ODD : DEG120_EVEN;
            }
            return isRowOdd ? DEG300_ODD : DEG300_EVEN;
        }
        return new(horizontal,vertical,0);
    }
    private bool InBounds(Vector3Int gridPos)
    {
        if (gridPos.x == _tileMapDimensions.x-1 && gridPos.y % 2 == 1)
        {
            return false;
        }
        return gridPos.x > -1 && gridPos.y > -1 && gridPos.x < _tileMapDimensions.x && gridPos.y < _tileMapDimensions.y;
    }

    public PlayerInput FindPlayer(InputDevice inputDevice)
    {
        return playerInputs.FirstOrDefault(p => p.devices.Contains(inputDevice));
    }
    public void OnMoveReset(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Reset move position");
        }
    }

    public void OnPointerMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var Direction = context.ReadValue<Vector2>();
        }
    }

    public void OnPointerToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            pointerMode = !pointerMode;
            Debug.Log(pointerMode ? "Entering pointer mode" : "Exiting pointer mode");
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