using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatManager : MonoBehaviour
{
    private Tilemap _tilemap;
    //The current turn number
    public int TurnNumber { get; private set; } = 1;
    public List<ITurnResponder> TurnResponders;
    public List<ITurnResponder> CurrentTurn;
    public List<Intent> Intents;
    //The TMP displaying the current turn number;
    public TextMeshProUGUI TurnNumberHolder;

    
    private void InitializeCombat(List<Player> players, CombatTemplate template)
    {
        _tilemap = FindObjectOfType<Tilemap>();
        TurnNumber = 1;
        TurnResponders = new List<ITurnResponder>();
        players.ForEach(p => TurnResponders.Add(p.GetComponent<ITurnResponder>()));
        template.enemies.ForEach(e => TurnResponders.Add(e.GetComponent<ITurnResponder>()));
        SpawnEnemies(template);
        
    }

    private void SpawnEnemies(CombatTemplate combatTemplate)
    {
        for (var i = 0; i < combatTemplate.enemies.Count; i++)
        {
            combatTemplate.enemies[i].position = combatTemplate.enemyPositions[i];
            Instantiate(combatTemplate.enemies[i], _tilemap.CellToWorld(combatTemplate.enemies[i].position), quaternion.identity);
        }
    }

    private async void StartTurn()
    {
        IncrementTurnText();
        UpdateIntents();
        while (Intents.Count > 0)
        {
            CheckGameOutcome();
            var turnFinished = await TurnResponders[0].DoTurn();
            if (turnFinished)
            {
            
            }
        }
        EndTurn();
    }
    
    private void IncrementTurnText()
    {
        TurnNumber += 1;
        TurnNumberHolder.text = TurnNumber.ToString();
        TurnNumberHolder.ForceMeshUpdate();
    }
    
    private void UpdateIntents()
    {
        
    }


    private void CheckGameOutcome()
    {
        
    }

    private void EndTurn()
    {

    }
    
    private TaskCompletionSource<float> _tcs;

    private Task<float> DoTurn()
    {
        _tcs = new TaskCompletionSource<float>();
        return _tcs.Task;
    }
}
