using System.Collections.Generic;
using UnityEngine;
public class CombatTemplate : ScriptableObject
{
    public string combatName;
    public string combatDescription;
    public List<Vector3Int> playerPositions;
    public List<Vector3Int> enemyPositions;
    public List<Enemy> enemies;
}
