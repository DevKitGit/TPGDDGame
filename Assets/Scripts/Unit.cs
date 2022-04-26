using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Random = System.Random;

public abstract class Unit : MonoBehaviour, ITurnResponder
{
    protected static readonly int AliveString = Animator.StringToHash("Alive");
    protected static readonly int WalkingString = Animator.StringToHash("Walking");
    protected static readonly int HitString = Animator.StringToHash("Hit");
    protected static readonly int AttackString = Animator.StringToHash("Attack");
    
    #region  TurnResponderFields

    public bool TurnDone
    {
        get => _turnDone;
        set => _turnDone = value;
    }
    
    public bool Alive
    {
        get => _Alive;
        set => _Alive = value;
    }
    
    public Faction AllyFaction
    {
        get => _AllyFaction;
        set => _AllyFaction = value;
    }
    public abstract Intent Intent { get; set; }
    public int TurnPriority { get; set; }
    public abstract TaskCompletionSource<bool> Tcs { get; set; }
    public abstract Task<bool> DoTurn();

    #endregion

    [SerializeField] public string UnitName;
    [SerializeField] public String description;
    [SerializeField] public Sprite icon;
    [SerializeField] public Vector3Int tilePosition;
    protected SpriteRenderer SpriteRenderer;
    protected Animator animator;
    protected HealthBarController _healthBarController;
    [Header("Stats")]
    public float LifeForce;
    public float MaxLifeForce;

    public float CombatMoves;
    public float MaxCombatMoves;

    [SerializeField] public List<Ability> abilities;
    [SerializeField] protected List<AbilitySo> AbilitySoList;
    [Header("TurnResponder fields")]
    [SerializeField] protected bool _turnDone;
    [SerializeField] protected bool _Alive;
    [SerializeField] protected Faction _AllyFaction;
    [SerializeField] protected Faction _EnemyFaction;
    [SerializeField] protected bool SelectPhaseActive;
    [SerializeField] protected bool MovePhaseThisTurn, MovePhaseActive, MovePhaseDone;
    [SerializeField] protected bool ActionPhaseThisTurn, ActionPhaseActive, ActionPhaseDone;
    [SerializeField] protected Audio onHit, onDeath, onMove, onStunned;
    protected AudioSource _MoveSource;
    protected List<Effect> appliedEffects = new();
    protected Stack<Tile> currentPath;
    public CombatBoardManager _combatBoardManager;

    [SerializeField, Range(0.1f, 10f)] protected float movementSpeed;
    [SerializeField, Range(0f, 1f)] protected float attackTimeNormalized;
    protected Tile _targetTile;

    protected void Awake()
    {
        _combatBoardManager = FindObjectOfType<CombatBoardManager>();
        animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        abilities = AbilitySoList.Select(so => new Ability(so)).ToList();
        _healthBarController = GetComponentInChildren<HealthBarController>();
        _healthBarController?.SetupSlider(this);
    }

    public enum Faction
    {
        Players,
        AI,
        Neutral,
        None
    }

   

    public void ReceiveEffect(List<Effect> effects)
    {
        AudioManager.Play(onHit, targetParent: gameObject);
        animator.SetTrigger(HitString);
        foreach (var effect in effects)
        {
            if (effect.ApplyImmediately)
            {
                
                if (!ApplySingleEffect(effect))
                {
                    //Unit died while applying effects, so stop applying more(prevents possible weird behavior)
                    Alive = false;
                    AudioManager.Play(onDeath, targetParent: gameObject);
                    animator.SetBool(AliveString,false);
                    break;
                }
            }
            appliedEffects.Add(effect);
        }
    }
    public void ApplyEffects()
    {

        if (appliedEffects.Count(effect => effect.EffectDurationInTurns >= 1) > 0)
        {
            AudioManager.Play(onHit, targetParent: gameObject);
        }
        foreach (var effect in appliedEffects.Where(effect => effect.EffectDurationInTurns >= 1))
        {
            effect.EffectDurationInTurns--;
            if (!ApplySingleEffect(effect))
            {
                //Unit died while applying effects, so stop applying more(prevents possible weird behavior)
                Alive = false;
                animator.SetBool(AliveString,false);
                break;
            }
        }
        appliedEffects.RemoveAll(effect => effect.EffectDurationInTurns <= 0);
    }

    public bool ApplySingleEffect(Effect effect)
    {
        var alive = true;
        switch (effect.effectType)
        {
            case Effect.EffectType.None:
                break;
            case Effect.EffectType.PhysicalDmg:
                alive = ApplyHealth(-UnityEngine.Random.Range(effect.MinimumAmount, effect.MaximumAmount + 1));
                break;
            case Effect.EffectType.MagicDmg:
                alive = ApplyHealth(-UnityEngine.Random.Range(effect.MinimumAmount, effect.MaximumAmount + 1));
                break;
            case Effect.EffectType.Stun:
                MovePhaseThisTurn = false;
                ActionPhaseThisTurn = false;
                break;
            case Effect.EffectType.Silence:
                ActionPhaseThisTurn = false;
                break;
            case Effect.EffectType.Slow:
                MovePhaseThisTurn = ApplyMoves(-UnityEngine.Random.Range(effect.MinimumAmount, effect.MaximumAmount + 1));
                break;
            case Effect.EffectType.Heal:
                ApplyHealth(UnityEngine.Random.Range(effect.MinimumAmount, effect.MaximumAmount + 1));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return alive;
    }

    private void VisualizeEffect(Effect effect)
    {
        switch (effect.effectType)
        {
            case Effect.EffectType.None:
                break;
            case Effect.EffectType.PhysicalDmg:
                //Damage take animation
                break;
            case Effect.EffectType.MagicDmg:
                //Damage take animation
                break;
            case Effect.EffectType.Stun:
                //Show a stun icon
                break;
            case Effect.EffectType.Silence:
                //Show a silence icon
                break;
            case Effect.EffectType.Slow:
                //Show a slow icon
                break;
            case Effect.EffectType.Heal:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool ApplyHealth(int change)
    {
        LifeForce = Math.Clamp(LifeForce + change, 0, MaxLifeForce);
        _healthBarController?.UpdateSlider(this);
        return LifeForce != 0;
    }
    
    public bool ApplyMoves(float change)
    {
        CombatMoves = Math.Clamp(CombatMoves + change, 1, MaxCombatMoves);
        return CombatMoves > 0f;
    }

    public void SetDirection(Tile destination, Tile origin = null)
    {
        if (destination == null)
        {
            return;
        }
        if (origin == null)
        {
            SetDirection(_combatBoardManager.GetTile(tilePosition).Position_world, destination.Position_world);
            return;
        }
        SetDirection(origin.Position_world, destination.Position_world);
    }
    public void SetDirection(Vector3 origin, Vector3 destination)
    {
        if (Vector3.Distance(origin, destination) <= Vector3.kEpsilon)
        {
            //dont change directions if you're standing in the same place
            return;
        }
        SpriteRenderer.flipX = Math.Sign(origin.x - destination.x) > 0;
    }
}
