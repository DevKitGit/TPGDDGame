using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Unit : MonoBehaviour
{
    public string Name;
    public Intent Intent;
    public Sprite Sprite;
    public SpriteRenderer SpriteRenderer;
    public int STR, DEX, CON, INT, LCK;
    public float combatMoveSpeed;
    public List<Vector2Int> position;
    public List<Ability> abilities;
    public String description;

    public enum Faction
    {
        Players,
        AI,
        Neutral,
        None
    } 

    public void OnEnable()
    {
        CombatManager.OnTurnStart += OnTurnStart;
        CombatManager.OnTurnEnd += OnTurnEnd;
    }

    public abstract void OnTurnStart();
    public abstract void OnTurnEnd();
    
    public void OnChangeDirection()
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
    }
    
    /*
     * level-up dump stats, increase charge of ability
     * a way to get player class, stats, weapon, abilities
     * player stats
     */
    //player calculations
    private int _classHPBase, _lifeForce, _playerLevel, _armor, _playerWpnDmg, _playerSpellDmg, 
        _pPhysDmg, _pRngDmg, _pMgcDmg, _movementSpeed;
    
    //paladin(gem boi) ability calc, mace, aura is based on heal/attack, block near allies
    private int _trgtAllyHeal/*Targeted ally heal, starts with 1 charge, max 2/3 replenish with auto-attack*/,
        _holyBurden/*shield bashes enemy, gives ally shield, 3 charges?*/,
        _teamImm/*Team immunity for a round, doesn't affect aura*/;
    
    //hunter ability calc, bow, mark target shot, deal less dmg at distance
    private int _huntersInsight/*avoid 1 projectile, and increases movement points, infinite charges*/,
        _ccSlow/*ability that slows x turns, 3 charges?*/,
        _AoE/*damage an area, proc 3 marks in a row*/;
    
    /*
     * barbarian ability calc, 2h axe, barb passive is when hitting enemy, that enemy deals reduced dmg to barb,
     * when attacking gain stack then start turn heal based on stack(heals max at specific cap), at end turn stack gets decreased by 1.
     * gain turns when killing enemies
     */
    private int _intimidatingShout/*Cone 3 tiles in front of barb and goes out to 5 tiles, 
    when enemy hit they walk in random direction for their next turn*/,
        _cleave/*melee splash, hits c-shape, gives stacks towards lifesteal*/,
        _rageMode/*gains charges based on # of attacks, dmg goes up based on stacks, stacks expires when used and damages*/;
    /*
     * sorcerer ability calc, 2h staff, shoots out arcane bolts to multiple enemies(turns into bouncing bolts),
     * sorc passive all attacks apply marks, and other attacks expend marks,
     */
    private int _tp/*swap 2 targets, self included*/,
        _marchOfFire/*self explanatory, AoE(turns into fire vines, roots and damages over time)*/,
        _/**/,
        _chaosZone/*if enemies hit = they can only basic attack, if self hit = buffs kit, lasts 3 turns*/;
 
    //a way to get enemy type, stats, weapone, abilities
    //enemy stats
    private int _ememyStr,_enemyDex, _enemyCon, _enemyInt, _enemyLluck;
    //enemy calculations
    private int _enemyHPBase, _enemyLifeForce, _enemyArmor, _enemyWpnDmg, _enemySpellDmg, _enemyLevel, 
        _ePhysDmg, _eRngDmg, _eMgcDmg, _enemyMovementSpeed;
    
    
    private void _statCalc()
    {
        //lifeforce calculation
        _lifeForce = (_classHPBase + (CON / 2)) + _playerLevel;
        _enemyLifeForce = (_enemyHPBase + (_enemyCon / 2) + _enemyLevel);
        
    }

    private void _playerDmgHandler()
    {
        //player damage calculation
        _pPhysDmg = ((STR / _enemyCon) * _playerWpnDmg) - _enemyArmor;
        _pRngDmg = ((DEX / _enemyCon) * _playerWpnDmg) - _enemyArmor;
        _pMgcDmg = ((INT / _enemyCon) * _playerSpellDmg) + (_playerLevel / 2);
    }

    private void _enemyDmgHandler()
    {
        //enemy damage calculation
        _ePhysDmg = ((_ememyStr / CON) * _enemyWpnDmg) + _enemyLevel;
        _eRngDmg = ((_ememyStr / CON) * _enemyWpnDmg) + _enemyLevel;
        _eMgcDmg = ((_enemyInt / CON) * _enemySpellDmg) + (_enemyLevel / 2);
    }
}
