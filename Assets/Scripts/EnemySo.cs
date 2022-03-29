using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Enemy", order = 1)]
public class EnemySo : ScriptableObject
{
    public string name;
    public Sprite icon;
    public int STR, DEX, CON, INT, LCK;
    public float lifeForce, moveSpeed, armor;
    public Unit.Faction AllyFaction = Unit.Faction.AI;
    public Unit.Faction targetFaction = Unit.Faction.Players;
    public List<Ability> abilities;
}