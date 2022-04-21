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
    public int TurnNumber { get; private set; } = 0;
    public List<ITurnResponder> TurnResponders;
    public List<ITurnResponder> CurrentTurn;
    public List<Intent> Intents;
    //The TMP displaying the current turn number;
    public TextMeshProUGUI TurnNumberHolder;
    public List<GameObject> enemies;

    public void InitializeCombat(List<Player> players, CombatTemplate template)
    {
        _tilemap = FindObjectOfType<Tilemap>();
        TurnNumber = 1;
        TurnResponders = new List<ITurnResponder>();
        players.ForEach(p => TurnResponders.Add(p.GetComponent<ITurnResponder>()));
        template.enemies.ForEach(e => TurnResponders.Add(e.GetComponent<ITurnResponder>()));
        //SpawnEnemies(template);
        TurnMainLoop();
    }
    
    /*private void SpawnEnemies(CombatTemplate combatTemplate)
    {
        for (var i = 0; i < combatTemplate.enemies.Count; i++)
        {
            
            combatTemplate.enemies[i].tilePosition = combatTemplate.enemyTilePositions[i];
            Instantiate(combatTemplate.enemies[i], _tilemap.CellToWorld(combatTemplate.enemies[i].tilePosition), quaternion.identity);
        }
    }*/

    private async void TurnMainLoop()
    {
        while (true)
        {
            BeginTurn();
            UpdateIntents();
            var factionWon = Unit.Faction.None;
            while (TurnResponders.FindAll(t => t.Alive && !t.TurnDone).Count > 0)
            {
                TurnResponders[0].TurnDone = await TurnResponders[0].DoTurn();
                factionWon = CheckGameOutcome();
                if (factionWon != Unit.Faction.None)
                {
                    break;
                }

                UpdateIntents();
            }

            if (factionWon != Unit.Faction.None)
            {
                EndGame();
                return;
            }

            EndTurn();
        }
    }

    private void BeginTurn()
    {
        IncrementTurnText();
        TurnResponders.FindAll(t => t.Alive).ForEach(t=> t.TurnDone = false);
    }
    
    private void UpdateIntents()
    {
        
    }

    private void EndTurn()
    {
        
    }
    
    private void EndGame()
    {
        
    }
    
    private void IncrementTurnText()
    {
        TurnNumber += 1;
        TurnNumberHolder.text = TurnNumber.ToString();
        TurnNumberHolder.ForceMeshUpdate();
    }
    private Unit.Faction CheckGameOutcome()
    {
        //did the player lose?
        var playersAlive = 0;
        var AIAlive = 0;
        foreach (var turnResponder in TurnResponders.Where(turnResponder => turnResponder.Alive))
        {
            switch (turnResponder.AllyFaction)
            {
                case Unit.Faction.AI:
                    AIAlive++;
                    break;
                case Unit.Faction.Players:
                    playersAlive++;
                    break;
            }
        }

        if (playersAlive == 0)
        {
            return Unit.Faction.AI;
        }

        if (AIAlive == 0)
        {
            return Unit.Faction.Players;
        }

        return Unit.Faction.None;
    }
}
