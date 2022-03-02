using System.Collections.Generic;
using UnityEngine;
public class CombatTemplate : ScriptableObject
{
    public string combatName;
    public string combatDescription;
    public List<Vector3Int> playerTilePositions;
    public List<Vector3Int> enemyTilePositions;
    public List<Enemy> enemies;
}
