using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    //The current turn number
    public int TurnNumber { get; private set; } = 1;

    public GameObject playerBasePrefab;
    public List<Player> players;
    
    public List<Enemy> enemies;
    //The TMP displaying the current turn number;
    public TextMeshProUGUI TurnNumberHolder;
    public List<Vector2Int> playerStartingIndices;
    public static event Action OnTurnStart = delegate {  };
    public static event Action OnTurnEnd = delegate{  };

    private void InitializeCombat(CombatTemplate template)
    {
        
    }

    private void SpawnEnemies(List<Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
        }
    }
    private void UpdateIntents()
    {
        
    }
    private void StartTurn()
    {
        IncrementTurnText();
    }
    
    private void EndTurn()
    {
        
    }
    
    private void IncrementTurnText()
    {
        TurnNumber += 1;
        TurnNumberHolder.text = TurnNumber.ToString();
        TurnNumberHolder.ForceMeshUpdate();
    }
}
