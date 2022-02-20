using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Player", order = 1)]
public class PlayerSo : ScriptableObject
{
    public string name;
    public Sprite icon;
    public int STR, DEX, CON, INT, LCK;
    public float lifeForce, moveSpeed, armor;
    public Unit.Faction allyFaction;
    public Unit.Faction enemyFaction;
    public List<Ability> abilities;
}

