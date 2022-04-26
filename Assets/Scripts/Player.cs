using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class Player : Unit
{
    public PlayerControls playerControls;
    public bool pointerMode = false;
    private float _timeSinceLastMove;
    private Vector2 moveValue = Vector2.zero;
    private bool moveActive = false;
    
    [SerializeField, Range(0,1)] private float _horitonzalBorder = 0.15f;
    [SerializeField, Range(0,1)] private float _verticalBorder =  0.25f;
    [SerializeField, Range(1,30)] private float _inputPollRate = 10;

    public override Intent Intent { get; set; } = null;
    public override TaskCompletionSource<bool> Tcs { get; set; }
    public Tile currentlyHoveredTile;
    public Stack<Tile> _currentPath = new Stack<Tile>();
    public List<Tile> tilesWithinReach = new List<Tile>();
    private List<Tile> _actionTargetTiles = new List<Tile>();
    private Dictionary<Guid, Action<InputAction.CallbackContext>> _lookup = new();
    private PlayerControls.ICombatActions _combatActionsImplementation;
    private PlayerController playerController;
    public bool turnActive = false;
    private Ability SelectedAbility = null;
    private bool SelectedAbilityChangedThisFrame;
    private bool ActionSelectPhaseActive;
    private bool uienabled = false;
    private GameObject SavedSelectedGameObject;
    private bool ActionMovePhase = false;
    private AnimationClip moveClip;

    private void Start()
    {
        _combatBoardManager = FindObjectOfType<CombatBoardManager>();
        _timeSinceLastMove = Time.time;
        _targetTile = null;
        Intent = new Intent(this, false, new List<Tile>(), new List<Ability>());
        SetupAttackEvent();
    }

    public void SetSelectedAbility(int index)
    {
        if (turnActive)
        {
            var previousIndex = abilities.IndexOf(SelectedAbility);
            if (previousIndex != -1)
            {
                FindObjectOfType<CharacterUIHandler>().UnhidePopup(previousIndex);
            }
            
            SelectedAbilityChangedThisFrame = SelectedAbility == abilities[index];
            
            SelectedAbility = abilities[index];
            
            
        }
    }
    
    private void SetupAttackEvent()
    {
        
        for (int i = 0; i < abilities.Count; i++)
        {
            var animationClipName = "Attack" + i;
            var clip = animator.runtimeAnimatorController.animationClips.FirstOrDefault(aclip =>
                aclip.name == animationClipName);
            if (clip == null || clip.events.ToList().Count != 0) continue;
            var animationEvent = new AnimationEvent
            {
                time = clip.length * abilities[i].AttackTimeNormalized, 
                functionName = nameof(AttackSequenceDone)
            };
            clip.AddEvent(animationEvent);
        }
    }
    
    public void AssignController(PlayerController pController)
    {
        playerController = pController;
        AssignActions();
    }
    
    private void AssignActions()
    {
        playerController.CombatGridMove += OnMove;
        playerController.CombatInteract += OnInteract;
        playerController.CombatGridMoveReset += OnMoveReset;
        playerController.CombatUIEnter += OnUIEnter;
        playerController.CombatCancel += OnCancel;
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed && turnActive && !uienabled && SelectedAbility != null)
        {
            var target = FindObjectsOfType<AbilitySlot>().FirstOrDefault(a => a._slottedAbilityIndex == abilities.IndexOf(SelectedAbility))?.gameObject;
            EnableUIInput(true,SavedSelectedGameObject);
            playerController.PlayerInput.ActivateInput();
            FindObjectOfType<CharacterUIHandler>().UnhidePopup(abilities.IndexOf(SelectedAbility));
            _actionTargetTiles?.ForEach(e => e.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default));
        }
    }

    private void OnDestroy()
    {
        UnassignActions();
    }

    public void UnassignActions()
    {
        playerController.CombatGridMove -= OnMove;
        playerController.CombatInteract -= OnInteract;
        playerController.CombatGridMoveReset -= OnMoveReset;
        playerController.CombatUIEnter -= OnUIEnter;
        playerController.CombatCancel -= OnCancel;

    }

    private void OnUIEnter(InputAction.CallbackContext context)
    {
        /*if (context.performed && turnActive && MovePhaseDone)
        {
            EnableUIInput(!uienabled);
        }*/
    }
    
    public void EndTurn()
    {
        turnActive = false;
        MovePhaseActive = false;
        ActionPhaseActive = false;
        MovePhaseDone = true;
        ActionPhaseDone = true;
        
        SelectedAbility = null;
        _actionTargetTiles = null;
        _targetTile = null;
        FindObjectOfType<CharacterUIHandler>().SetAvailable(false);
        playerController.PlayerInput.DeactivateInput();
        Tcs.TrySetResult(true);
    }

    public void BeginTurn()
    {
        ApplyMoves(MaxCombatMoves);     
        tilesWithinReach = _combatBoardManager.Pathfinder.FindWalkableBFS(tilePosition, CombatMoves).ToList();

        currentlyHoveredTile = _combatBoardManager.GetTile(tilePosition);

        SelectPhaseActive = true;
        
        MovePhaseThisTurn = true;
        ActionPhaseThisTurn = true;
        
        MovePhaseDone = false;
        ActionPhaseDone = false;
        
        MovePhaseActive = false;
        ActionPhaseActive = false;
        
        ActionSelectPhaseActive = false;
    }
   
    public override Task<bool> DoTurn()
    { 
        BeginTurn();
        ApplyEffects();
        Tcs = new TaskCompletionSource<bool>();

        if (!MovePhaseThisTurn && !ActionPhaseThisTurn)
        {
            Tcs.TrySetResult(true);
        }
        else
        {
            playerController.PlayerInput.ActivateInput();
            FindObjectOfType<CharacterUIHandler>().UpdatePlayerUI(this);
            
            turnActive = true;
            tilesWithinReach = _combatBoardManager.Pathfinder.FindWalkableBFS(tilePosition, CombatMoves).ToList();
            tilesWithinReach.ToList().ForEach(tile => tile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.WithinReach));
            //BeginMovePhase();
            //Invoke(nameof(EndTurn),1);
        }
        return Tcs.Task;
    }

    private void EnableUIInput(bool enabled,GameObject target = null)
    {
        var inputModule = InputManager.Instance.gameObject.GetComponent<InputSystemUIInputModule>();
        uienabled = enabled;
        if (enabled)
        {
            
            inputModule.UnassignActions();
            inputModule.AssignDefaultActions();
            inputModule.actionsAsset = playerController.PlayerInput.actions;
            InputManager.Instance.EventSystem.SetSelectedGameObject(target);
            inputModule.ActivateModule();
            InputManager.Instance.EventSystem.currentInputModule.UpdateModule();
        }
        else
        {
            SavedSelectedGameObject = InputManager.Instance.EventSystem.currentSelectedGameObject;
            InputManager.Instance.EventSystem.SetSelectedGameObject(null);
            inputModule.UnassignActions();
        }
    }
    private void BeginMovePhase()
    {
        
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed && turnActive)
        {
            
            moveValue = context.ReadValue<Vector2>();;
            moveActive = moveValue.magnitude > 0;
        }
    }

    public void OnMoveReset(InputAction.CallbackContext context)
    {
        if (context.performed || !turnActive) return;
        if (SelectPhaseActive && !MovePhaseActive && !MovePhaseDone)
        {
            MoveCurrentDisplayTarget(_combatBoardManager.GetTile(tilePosition));
        } else if (turnActive && ActionSelectPhaseActive)
        {
            
        }
    }

    public void OnPointerMove(InputAction.CallbackContext context)
    {
        if (context.performed  && turnActive)
        {
            var Direction = context.ReadValue<Vector2>();
        }
    }

    public void OnPointerToggle(InputAction.CallbackContext context)
    {
        if (context.performed  && turnActive)
        {
            pointerMode = !pointerMode;
            Debug.Log(pointerMode ? "Entering pointer mode" : "Exiting pointer mode");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || !turnActive) return;

        if (SelectPhaseActive && MovePhaseThisTurn && !MovePhaseActive && !MovePhaseDone)
        {            
            AudioManager.Play(FindObjectOfType<CombatManager>().onButtonPress, targetParent: FindObjectOfType<CombatManager>().gameObject);

            //if select mode is enabled, and there is a movephase this turn that isn't done, then begin move phase
            _targetTile = currentlyHoveredTile;
            ApplyMoves(-_targetTile.Cost);

            foreach (var tile in _currentPath)
            {
                tile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default);
            }
            tilesWithinReach.ForEach(t => t.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default));
            animator.SetBool(WalkingString,true);
            SelectPhaseActive = false;
            MovePhaseActive = true;
            if (_MoveSource != null)
            {
                Destroy(_MoveSource.gameObject);
            }
            _MoveSource = AudioManager.Play(onMove, true, targetParent: gameObject);
        }else if (ActionSelectPhaseActive && MovePhaseDone && SelectedAbility != null && uienabled)
        {
            AudioManager.Play(FindObjectOfType<CombatManager>().onButtonPress, targetParent: FindObjectOfType<CombatManager>().gameObject);

            EnableUIInput(false);
            uienabled = false;
            FindActionTargetTiles();
            foreach (var tile in _actionTargetTiles)
            {
                tile.IndicatorTile.SetIndicator(SelectedAbility.targetType switch
                {
                    Ability.TargetType.self => IndicatorTile.Indicator.Ally,
                    Ability.TargetType.ally => IndicatorTile.Indicator.Ally,
                    Ability.TargetType.ground => IndicatorTile.Indicator.WithinReach,
                    Ability.TargetType.enemy => IndicatorTile.Indicator.Enemy,
                    Ability.TargetType.pet => IndicatorTile.Indicator.Ally,
                    _ => throw new ArgumentOutOfRangeException(),
                });

            }
            _actionTargetTiles.FirstOrDefault(e => e.UnitOnTile != null)?.IndicatorTile.SetIndicator(SelectedAbility.targetType switch
            {
                
                Ability.TargetType.self => IndicatorTile.Indicator.SelectedAlly,
                Ability.TargetType.ally => IndicatorTile.Indicator.SelectedAlly,
                Ability.TargetType.ground => IndicatorTile.Indicator.ChosenPath,
                Ability.TargetType.enemy => IndicatorTile.Indicator.SelectedEnemy,
                Ability.TargetType.pet => IndicatorTile.Indicator.SelectedAlly,
                _ => throw new ArgumentOutOfRangeException(),
            });
            var target = _actionTargetTiles.FirstOrDefault(e => e.UnitOnTile != null);
            SetDirection(target == null ? _actionTargetTiles.FirstOrDefault() : target);
        }
        else if (ActionSelectPhaseActive && MovePhaseDone && SelectedAbility != null && _actionTargetTiles != null && _actionTargetTiles.Count > 0)
        {
            AudioManager.Play(FindObjectOfType<CombatManager>().onButtonPress, targetParent: FindObjectOfType<CombatManager>().gameObject);

            animator.SetTrigger("Attack"+abilities.IndexOf(SelectedAbility));
            var moveeffect = SelectedAbility.Effects.FirstOrDefault(e => e.effectType == Effect.EffectType.Move);
            if (moveeffect != null)
            {
                ActionMovePhase = true;
                moveClip = animator.runtimeAnimatorController.animationClips.FirstOrDefault(e =>
                    e.name == "Attack" + abilities.IndexOf(SelectedAbility));
            }
            else
            {
                SetDirection(_targetTile);
            }
            ActionSelectPhaseActive = false;
            turnActive = false;
            //EnableUIInput(false,FindObjectsOfType<AbilitySlot>().First(a => a._slottedAbility == abilities[0]).gameObject);
        }
    }

    public void AttackSequenceDone()
    {
        if (SelectedAbility.Ranged)
        {
            SpawnRangedAttack();
            return;
        }

        AudioManager.Play(SelectedAbility.TargetAudio,targetParent:gameObject);
        if (SelectedAbility.Radius > 0)
        {
            var list = _combatBoardManager.Pathfinder.FindMeleeNeighbours(_combatBoardManager.GetTile(tilePosition), _targetTile,SelectedAbility.Radius);
            list.ForEach(e =>
            {
                print(e.Position_grid);
                if (e.UnitOnTile != null && e.UnitOnTile.AllyFaction == Faction.AI && e.UnitOnTile.Alive)
                {
                    
                    _targetTile.UnitOnTile.ReceiveEffect(SelectedAbility.Effects);
                }
                
            });
        }
        else
        {
            _targetTile.UnitOnTile.ReceiveEffect(SelectedAbility.Effects);
        }
        _actionTargetTiles.ForEach(t => t.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default));
        EndTurn();
    }
    
    public async void SpawnRangedAttack()
    {
        await SelectedAbility.SpawnProjectile(_combatBoardManager.GetTile(tilePosition), _targetTile);
        if (SelectedAbility.Radius > 0)
        {
            var list = _combatBoardManager.Pathfinder.FindRangedNeighbours(_targetTile, SelectedAbility.Radius);
            list.ForEach(e =>
            {
                if (e.UnitOnTile != null && e.UnitOnTile.AllyFaction == Faction.AI && e.UnitOnTile.Alive)
                {
                    e.UnitOnTile.ReceiveEffect(SelectedAbility.Effects);
                }
                
            });
        }
        else
        {
            _targetTile.UnitOnTile.ReceiveEffect(SelectedAbility.Effects);
        }
        _actionTargetTiles.ForEach(t => t.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default));
        EndTurn();
    }
    
    public Vector3Int InterpolateGridDirection(Vector3Int origin, Vector3 inputDirection)
    {
        inputDirection.Normalize();

        var horizontal = math.abs(inputDirection.x) > _horitonzalBorder ? Math.Sign(inputDirection.x) : 0;
        var vertical = math.abs(inputDirection.y) > _verticalBorder ? Math.Sign(inputDirection.y) : 0;
        var isRowOdd = origin.y % 2 == 0;
        
        if (!(Mathf.Abs(horizontal) + math.abs(vertical) > 1f)) return new(horizontal, vertical, 0);
        switch (horizontal+vertical)
        {
            case 2:
                return isRowOdd ? CombatBoardManager.DEG60_ODD : CombatBoardManager.DEG60_EVEN;
            case -2:
                return isRowOdd ? CombatBoardManager.DEG240_ODD : CombatBoardManager.DEG240_EVEN;
        }
        if (horizontal < 0)
        {
            return isRowOdd ? CombatBoardManager.DEG120_ODD : CombatBoardManager.DEG120_EVEN;
        }
        return isRowOdd ? CombatBoardManager.DEG300_ODD : CombatBoardManager.DEG300_EVEN;

    }
    private void Update()
    {
        if (!turnActive || !Alive)
        {
            if (ActionMovePhase)
            {
                ActionMove();
            }
            else
            {
                return;
            }
        }
        if (SelectPhaseActive && !MovePhaseActive && !ActionPhaseActive)
        {
            MoveGridIndicator();
        }
        if (MovePhaseActive)
        {
            Move();
        }
        else if (ActionSelectPhaseActive)
        {
             ActionSelect();
        }
    }

    private void ActionMove()
    {
        if (Vector3.Distance(transform.position, _targetTile.Position_world) < 0.1f)
        {
            ActionMovePhase = false;
            transform.position = _targetTile.Position_world;
            _combatBoardManager.GetTile(tilePosition).UnitOnTile = null;
            tilePosition = _targetTile.Position_grid;
            _combatBoardManager.GetTile(tilePosition).UnitOnTile = this;
            _actionTargetTiles.ForEach(e =>e.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default));
            EndTurn();
        }

        if (_targetTile != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetTile.Position_world, Time.deltaTime*3);
        }
    }
    private void ActionSelect()
    {
        if (SelectedAbility == null || SelectedAbilityChangedThisFrame )
        {
            if (_actionTargetTiles == null) return;
            foreach (var tile in _actionTargetTiles)
            {
                tile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default);
            }

            if (_targetTile != null)
            {
                _targetTile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default);
            }
            SelectedAbilityChangedThisFrame = false;
        }
        else if (SelectedAbility.Ranged && moveActive && !uienabled && Time.time - _timeSinceLastMove > 1f/_inputPollRate)
        {
            _timeSinceLastMove = Time.time;
            foreach (var tile in _actionTargetTiles)
            {
                tile.IndicatorTile.SetIndicator(SelectedAbility.targetType switch
                {
                    Ability.TargetType.self => IndicatorTile.Indicator.Enemy,
                    Ability.TargetType.ally => IndicatorTile.Indicator.Ally,
                    Ability.TargetType.ground => IndicatorTile.Indicator.WithinReach,
                    Ability.TargetType.enemy => IndicatorTile.Indicator.Enemy,
                    Ability.TargetType.pet => IndicatorTile.Indicator.Ally,
                    _ => throw new ArgumentOutOfRangeException(),
                });
            }

            var nearestAngleTile =
                _combatBoardManager.Pathfinder.FindNearestAngleTile(_targetTile, moveValue, _actionTargetTiles.Where(e => e != _targetTile).ToList(), 25f,SelectedAbility.targetType != Ability.TargetType.ground);
            _targetTile = nearestAngleTile != null ? nearestAngleTile : _targetTile;
            if (_targetTile != null)
            {
                SetDirection(_targetTile);
                _targetTile.IndicatorTile.SetIndicator(SelectedAbility.targetType switch
                {
                    Ability.TargetType.self => IndicatorTile.Indicator.SelectedAlly,
                    Ability.TargetType.ally => IndicatorTile.Indicator.SelectedAlly,
                    Ability.TargetType.ground => IndicatorTile.Indicator.ChosenPath,
                    Ability.TargetType.enemy => IndicatorTile.Indicator.SelectedEnemy,
                    Ability.TargetType.pet => IndicatorTile.Indicator.SelectedAlly,
                    _ => throw new ArgumentOutOfRangeException(),
                });
            }
        }
        else if (moveActive && !uienabled && Time.time - _timeSinceLastMove > 1f/_inputPollRate)
        {
            _timeSinceLastMove = Time.time;
            var nearestAngleTile =
                _combatBoardManager.Pathfinder.FindNearestAngleTile(_combatBoardManager.GetTile(tilePosition), moveValue, _actionTargetTiles.Where(e => e != _targetTile).ToList(), 25f, SelectedAbility.targetType != Ability.TargetType.ground);
            foreach (var tile in _actionTargetTiles)
            {
                if (tile == nearestAngleTile)
                {
                    SetDirection(_targetTile);
                    tile.IndicatorTile.SetIndicator(SelectedAbility.targetType switch
                    {
                        Ability.TargetType.self => IndicatorTile.Indicator.SelectedAlly,
                        Ability.TargetType.ally => IndicatorTile.Indicator.SelectedAlly,
                        Ability.TargetType.ground => IndicatorTile.Indicator.ChosenPath,
                        Ability.TargetType.enemy => IndicatorTile.Indicator.SelectedEnemy,
                        Ability.TargetType.pet => IndicatorTile.Indicator.SelectedAlly,
                        _ => throw new ArgumentOutOfRangeException(),
                    });
                    continue;
                }
                tile.IndicatorTile.SetIndicator(SelectedAbility.targetType switch
                {
                    Ability.TargetType.self => IndicatorTile.Indicator.Enemy,
                    Ability.TargetType.ally => IndicatorTile.Indicator.Ally,
                    Ability.TargetType.ground => IndicatorTile.Indicator.WithinReach,
                    Ability.TargetType.enemy => IndicatorTile.Indicator.Enemy,
                    Ability.TargetType.pet => IndicatorTile.Indicator.Ally,
                    _ => throw new ArgumentOutOfRangeException(),
                });
            }
        }
    }

    private void FindActionTargetTiles()
    {
        if (SelectedAbility.Ranged)
        {
            var moveeffect = SelectedAbility.Effects.FirstOrDefault(e => e.effectType == Effect.EffectType.Move);
            if (moveeffect != null)
            {
                _actionTargetTiles = _combatBoardManager.Pathfinder.FindRangedNeighbours(_combatBoardManager.GetTile(tilePosition), moveeffect.MinimumAmount);
                _actionTargetTiles.RemoveAll(e => e.UnitOnTile != null);
                _targetTile = _actionTargetTiles.FirstOrDefault();
            }
            else
            {
                var targets = FindObjectsOfType<Unit>().ToList().Where(e => e.AllyFaction == _EnemyFaction && e.Alive).ToList();
                _actionTargetTiles = targets.Select(e => _combatBoardManager.GetTile(e.tilePosition)).ToList();
                _targetTile = _actionTargetTiles.FirstOrDefault();
            }
            return;
        }
        _actionTargetTiles = _combatBoardManager.Pathfinder.Neighbors(_combatBoardManager.GetTile(tilePosition)).Where(e => e.UnitOnTile != null && e.UnitOnTile.AllyFaction == Faction.AI && e.UnitOnTile.Alive).ToList();
        _targetTile = _actionTargetTiles.FirstOrDefault();
    }
    private void MoveGridIndicator()
    {
        if (moveActive && Time.time - _timeSinceLastMove > 1f/_inputPollRate)
        {
            var direction = InterpolateGridDirection(currentlyHoveredTile.Position_grid, moveValue);
            var resultTile = _combatBoardManager.GetTile(currentlyHoveredTile.Position_grid + direction);
            if (resultTile == currentlyHoveredTile)
            {
                //if the user didn't move, return.
                return;
            }
            
            if (resultTile == null || !resultTile.Walkable || resultTile.UnitOnTile != null)
            {
                MoveDenied();
            }
            else
            {
                var path = _combatBoardManager.Pathfinder.FindPathDFS(tilePosition, resultTile.Position_grid);
                if (path.ToList()[^1].Cost > CombatMoves)
                {
                    //dont allow this move if the path is out of bounds
                    
                    MoveDenied();
                    return;
                }
                MoveCurrentDisplayTarget(resultTile);
            }
        }
    }
    private void MoveCurrentDisplayTarget(Tile displayTargetTile)
    {
        if (_currentPath?.Count > 0 )
        {
            _combatBoardManager.GetTile(tilePosition).IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default);
            _currentPath.ToList().ForEach(tile => tile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default));
        }

        if (tilesWithinReach?.Count > 0)
        {
            tilesWithinReach.ForEach(tile => tile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.Default));
        }
        
        tilesWithinReach = _combatBoardManager.Pathfinder.FindWalkableBFS(tilePosition, CombatMoves).ToList();
        tilesWithinReach.ToList().ForEach(tile => tile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.WithinReach));
        
        
        _currentPath = _combatBoardManager.Pathfinder.FindPathDFS(tilePosition, displayTargetTile.Position_grid);
        currentlyHoveredTile = displayTargetTile;
        _combatBoardManager.GetTile(tilePosition).IndicatorTile.SetIndicator(IndicatorTile.Indicator.ChosenPath);
        _currentPath?.ToList().ForEach(tile => tile.IndicatorTile.SetIndicator(IndicatorTile.Indicator.ChosenPath));
        _timeSinceLastMove = Time.time;
    }
    private void MoveDenied()
    {
        
    }
    public void Move()
    {
        if (!_currentPath.TryPeek(out _targetTile) || _targetTile.Position_grid == tilePosition)
        {
            MovePhaseDone = true;
            MovePhaseActive = false;
            animator.SetBool(WalkingString,false);
            _currentPath.Clear();
            ActionSelectPhaseActive = true;
            Destroy(_MoveSource.gameObject);

            EnableUIInput(true,FindObjectsOfType<AbilitySlot>().First(a => a._slottedAbility == abilities[0]).gameObject);
            return;
        }

        //another tile exists, move towards that
        transform.position = Vector3.MoveTowards(
            transform.position, 
            _targetTile.Position_world,
            Time.deltaTime * movementSpeed);

        if (!(Vector3.Distance(transform.position, _targetTile.Position_world) < 0.01f))
        {
            return;
        }
        //if a tile has been reached, pop it and set direction towards new one.
        _combatBoardManager.GetTile(tilePosition).UnitOnTile = null;
        _targetTile.UnitOnTile = this;
        tilePosition = _targetTile.Position_grid;
        transform.position = _targetTile.Position_world;
        _currentPath.TryPop(out _);
        if (_currentPath.TryPeek(out _targetTile))
        {
            SetDirection(transform.position, _targetTile.Position_world);
        }
    }
}