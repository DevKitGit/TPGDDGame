using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Player", order = 1)]
public class PlayerSo : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public int STR, DEX, CON, INT, LCK;
    public int lifeForce, armor;
    public float moveSpeed;
    public Unit.Faction allyFaction;
    public Unit.Faction enemyFaction;
    public List<Ability> abilities;
}

