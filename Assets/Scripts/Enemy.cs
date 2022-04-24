
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Unit
{
    public override Intent Intent { get; set; }
    public override TaskCompletionSource<bool> Tcs { get; set; }

    public bool turnActive = false;
    private bool HasAttacked = false;

    protected void Start()
    {
        SetupAttackEvent();
        
    }

    public void UpdateIntent()
    {
        if (Intent == null)
        {
            Intent = new Intent(this, false, FindTarget(), abilities.GetRange(0,1));
        }
        else
        {
            Intent.targets = FindTarget();
        }
    }
    
    public List<Tile> FindTarget()
    {
        var targets = FindObjectsOfType<Unit>().ToList();
        targets.RemoveAll(t => t.AllyFaction == AllyFaction);
        Stack<Tile> closestTargetPath = new Stack<Tile>();
        foreach (var target in targets)
        {

            var targetPath = _combatBoardManager.Pathfinder.FindPathDFS(tilePosition, target.tilePosition);
            if (targetPath == null || targetPath.Count == 0)
            {
                //no path found
                continue;
            }
            
            if (closestTargetPath != null || closestTargetPath.Count == 0 || targetPath.ToList()[Index.End].Cost <= closestTargetPath.ToList()[Index.End].Cost)
            {
                
                closestTargetPath = targetPath;
            }
        }

        var targetTile = closestTargetPath.LastOrDefault(tile => tile.Cost <= CombatMoves + 1);
        if (targetTile == null)
        {
            return null;
        }
        if (targetTile.UnitOnTile != null)
        {
            targetTile = targetTile.Parent;
        }
        return new List<Tile> {targetTile};
    }

    public override Task<bool> DoTurn()
    {
        Tcs = new TaskCompletionSource<bool>();
        turnActive = true;
        HasAttacked = false;
        BeginMovePhase();
        return Tcs.Task;
    }
    
    public void BeginMovePhase()
    {
        if (Intent.targets == null)
        {
            //no target exists so dont target anything.
            MovePhaseActive = false;
            MovePhaseThisTurn = false;
            MovePhaseDone = true;
            return;
        }
        currentPath = _combatBoardManager.Pathfinder.FindPathDFS(tilePosition, Intent.targets[0].Position_grid);
        if (currentPath != null && currentPath.Count > 0)
        {
            if (!currentPath.TryPeek(out _targetTile))
            {
                MovePhaseActive = false;
                MovePhaseThisTurn = false;
                MovePhaseDone = true;
                return;
            }
            MovePhaseActive = true;
            animator.SetBool(WalkingString,currentPath.Peek().UnitOnTile == null);
            SetDirection(gameObject.transform.position, _targetTile.Position_world);
        }
        else
        {
            MovePhaseActive = false;
            MovePhaseThisTurn = false;
            MovePhaseDone = true;
        }
    }

    private void Update()
    {
        if (!turnActive || !Alive) return;
        if (MovePhaseActive)
        {
            Move();
        }
        else if (ActionPhaseActive)
        {
            Attack(); 
            
        }
    }

    public void Move()
    {
        if (!currentPath.TryPeek(out _targetTile) || _targetTile.Position_grid == tilePosition)
        {
            MovePhaseDone = true;
            MovePhaseActive = false;
            animator.SetBool(WalkingString,false);
            currentPath.Clear();
            ActionPhaseActive = true;
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
        transform.position = _combatBoardManager._tileMap.CellToWorld(tilePosition);
        currentPath.TryPop(out _);
        if (currentPath.TryPeek(out _targetTile))
        {
            SetDirection(transform.position, _combatBoardManager._tileMap.CellToWorld(_targetTile.Position_grid));
        }
    }

    public void Attack()
    {
        ActionPhaseActive = false;
        var unitsInRange = _combatBoardManager.Pathfinder.Neighbors(_combatBoardManager.GetTile(tilePosition));
        var tileWithUnit = unitsInRange.FirstOrDefault(tile => tile.UnitOnTile != null && tile.UnitOnTile.Alive && tile.UnitOnTile.AllyFaction == _EnemyFaction);
        if (tileWithUnit == null)
        {
            EndTurn();
            return;
        }
        //print($"{gameObject.name} tried to attack target: {tileWithUnit.UnitOnTile.gameObject.name}");

        //a unit is in melee range, so set attack
        Intent.targets[0] = tileWithUnit;
        SetDirection(transform.position, Intent.targets[0].Position_world);
        animator.SetTrigger(AttackString);

    }

    private void SetupAttackEvent()
    {
        var clip = animator.runtimeAnimatorController.animationClips.FirstOrDefault(aclip =>
            aclip.name.ToLower().Contains("attack"));
        if (clip == null) return;
        var animationEvent = new AnimationEvent
        {
            time = clip.length * attackTimeNormalized, 
            functionName = nameof(OnAttackHit)
        };
        clip.AddEvent(animationEvent);
    }
    
    public void OnAttackHit()
    {
        if (HasAttacked)
        {
            return;
        }

        HasAttacked = true;
        Intent.targets[0].UnitOnTile.ReceiveEffect(Intent.abilities[0].Effects);
        EndTurn();
    }
    public void EndTurn()
    {
        turnActive = false;
        MovePhaseActive = false;
        ActionPhaseActive = false;
        Tcs.TrySetResult(true);
    }
    
 
}
