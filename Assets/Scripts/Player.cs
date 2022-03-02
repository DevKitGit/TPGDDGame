using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : Unit, PlayerControls.ICombatActions
{
    
    public CombatBoardManager _cbm;
    public PlayerControls playerControls;
    public PlayerInputManager playerInputManager;
    public bool pointerMode = false;
    private float _timeSinceLastMove;
    private Vector2 moveValue = Vector2.zero;
    private bool moveActive = false;
    
    [SerializeField, Range(0,1)] private float _horitonzalBorder = 0.15f;
    [SerializeField, Range(0,1)] private float _verticalBorder =  0.25f;
    [SerializeField, Range(1,30)] private float _inputPollRate = 10;

    public Vector3Int _indicatorTilePosition;
    public override Intent Intent { get; set; } = null;
    public override TaskCompletionSource<bool> Tcs { get; set; }
    private PlayerInput _playerInput;
    public Vector3Int currentlyHoveredTile;
    public Stack<Tile> _currentPath = new Stack<Tile>();
    public List<Tile> tilesWithinReach = new List<Tile>();

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        playerInputManager = PlayerInputManager.instance;
        _timeSinceLastMove = Time.time;
        playerControls = new PlayerControls(); 
        playerControls.Combat.SetCallbacks(this);
        playerControls.Enable();
        _cbm = FindObjectOfType<CombatBoardManager>();
        SetupPlayerForCombat(tilePosition);
    }


    public void EndTurn()
    {
        if (_playerInput.inputIsActive)
        {
            _playerInput.DeactivateInput();
            MakeTilesWithinReach(false);
        }
    }

    public void MakeTilesWithinReach(bool withinReach)
    {
        tilesWithinReach.ForEach(t => t.WithinReach = withinReach);
        tilesWithinReach.ForEach(
            tile => tile.IndicatorTile.SetIndicator(withinReach ? IndicatorTile.Indicator.WithinReach : IndicatorTile.Indicator.Default));

    }
    public override Task<bool> DoTurn()
    { 
        BeginTurn();
        ApplyEffects();
        if (MovePhaseThisTurn || ActionPhaseThisTurn)
        {
            _playerInput.ActivateInput();
        }
        Tcs = new TaskCompletionSource<bool>();
        return Tcs.Task;
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

    public void OnMoveReset(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _indicatorTilePosition = tilePosition;
            DisplayMoveAccepted(tilePosition);
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
                return isRowOdd ? CombatBoardManager.DEG60_ODD : CombatBoardManager.DEG60_EVEN;
            } 
            if (horizontal+vertical==-2)
            {
                return isRowOdd ? CombatBoardManager.DEG240_ODD : CombatBoardManager.DEG240_EVEN;
            }if (horizontal < 0)
            {
                return isRowOdd ? CombatBoardManager.DEG120_ODD : CombatBoardManager.DEG120_EVEN;
            }
            return isRowOdd ? CombatBoardManager.DEG300_ODD : CombatBoardManager.DEG300_EVEN;
        }
        return new(horizontal,vertical,0);
        
    }
    private void OnEnable()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }
    private void OnDisable()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerControls.Disable();
    }
    
    private void FixedUpdate()
    {
        MoveGridIndicator();
    }
    private void MoveGridIndicator()
    {
        if (moveActive && Time.time - _timeSinceLastMove > 1f/_inputPollRate)
        {
            var direction = InterpolateGridDirection(_indicatorTilePosition, moveValue);
            var resultDirection = _indicatorTilePosition + direction;
            if (!_cbm.InBounds(resultDirection) || !_cbm.board[_cbm.Index(resultDirection)].Walkable || !_cbm.board[_cbm.Index(resultDirection)].WithinReach)
            {
                displayMoveDenied();
            }
            else
            {
                DisplayMoveAccepted(resultDirection);
            }
        }
    }

    private void DisplayMoveAccepted(Vector3Int resultDirection)
    {
        if (_currentPath?.Count > 0 )
        {
            _currentPath.ToList().ForEach(tile => tile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default));
            _currentPath.Clear();
                    
        }
        tilesWithinReach = _cbm.Pathfinder.FindWalkableTiles(tilePosition, combatMoveSpeed);
        MakeTilesWithinReach(true);
        _indicatorTilePosition = resultDirection;
        _currentPath = _cbm.Pathfinder.FindPath(tilePosition, _indicatorTilePosition);
        _currentPath?.ToList().ForEach(tile => tile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.ChosenPath));
        _timeSinceLastMove = Time.time;
    }

    private void displayMoveDenied()
    {
        
    }
    
    private void SetupPlayerForCombat(Vector3Int cellPosition)
    {
        gameObject.transform.position = _cbm._tileMap.CellToWorld(cellPosition);
        _indicatorTilePosition = cellPosition;
        tilesWithinReach = _cbm.Pathfinder.FindWalkableTiles(tilePosition, combatMoveSpeed);
        MakeTilesWithinReach(true);
    }
}