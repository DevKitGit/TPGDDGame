using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    public Sprite IntentPlaceholder;
    public List<Image> IntentDisplayList;
    public List<GameObject> enemies;
    public CombatBoardManager combatBoardManager;
    private CombatTemplate _combatTemplate;
    private BannerAnimator _bannerAnimator;
    
    public void InitializeCombat(CombatTemplate template)
    {
        _combatTemplate = template;
        _bannerAnimator = FindObjectOfType<BannerAnimator>();
        _tilemap = FindObjectOfType<Tilemap>();
        combatBoardManager = FindObjectOfType<CombatBoardManager>();
        combatBoardManager.CreateBoard();
        TurnResponders = new List<ITurnResponder>();
        SpawnPlayers();
        SpawnEnemies();
        foreach (var onSelectPopup in FindObjectsOfType<OnSelectPopup>(true))
        {
            onSelectPopup.SetupPopup();
        }
        Invoke(nameof(TurnMainLoop),0.5f);
    }

    private void SpawnPlayers()
    {
        var playerInputs = PlayerInput.all;
        for (var index = 0; index < playerInputs.Count; index++)
        {
            var playerInput = playerInputs[index];
            playerInput.SwitchCurrentActionMap("Combat");
            var player = Instantiate(playerInput.gameObject.GetComponent<PlayerController>().PlayerAsset.Prefab,
                _tilemap.CellToWorld(_combatTemplate.playerTilePositions[index]),quaternion.identity).GetComponent<Player>();
            player._combatBoardManager = combatBoardManager;
            player.tilePosition = _combatTemplate.playerTilePositions[index];
            combatBoardManager.GetTile(player.tilePosition).SetUnit(player);
            player.AssignController(playerInput.gameObject.GetComponent<PlayerController>());
            TurnResponders.Add(player.gameObject.GetComponent<ITurnResponder>());
            player.SetDirection(combatBoardManager.GetCenterTile());
            playerInput.SwitchCurrentActionMap("Combat");
            playerInput.DeactivateInput();
        }
    }
    
    private void SpawnEnemies()
    {
        for (var i = 0; i < _combatTemplate.enemies.Count; i++)
        {
            var enemy = Instantiate(_combatTemplate.enemies[i], _tilemap.CellToWorld(_combatTemplate.enemyTilePositions[i]), quaternion.identity).GetComponent<Enemy>();
            enemy.gameObject.name = enemy.name;
            enemy._combatBoardManager = combatBoardManager;
            enemy.tilePosition = _combatTemplate.enemyTilePositions[i];
            combatBoardManager.GetTile(enemy.tilePosition).SetUnit(enemy);

            enemy.SetDirection(combatBoardManager.GetCenterTile());
            TurnResponders.Add(enemy.gameObject.GetComponent<ITurnResponder>());
        }
    }

    private async void TurnMainLoop()
    {
        while (true)
        {
            BeginTurn();
            

            UpdateIntents();
            var currentTurnIndex = 0;
            while (TurnResponders.Count(t => t.Alive && !t.TurnDone) > 0)
            {
                if (TurnResponders[currentTurnIndex].Alive)
                {
                    await Task.Delay(1000);
                    var attachment = "'s turn"; 
                    await _bannerAnimator.StartBanner(((Unit) TurnResponders[currentTurnIndex]).UnitName+attachment, 2f);
                    TurnResponders[currentTurnIndex].TurnDone = await TurnResponders[currentTurnIndex].DoTurn();
                }
                currentTurnIndex++;
                var factionWon = CheckGameOutcome();
                if (factionWon != Unit.Faction.None)
                {
                    Debug.Log($"Faction winner: {factionWon}");
                    EndGame();
                    return;
                }
                UpdateIntents();
            }
            await _bannerAnimator.StartBanner($"Turn {TurnNumber+1} begins", 2f);
            EndTurn();
        }
    }

    private void BeginTurn()
    {
        IncrementTurnText();
        TurnResponders.FindAll(t => t.Alive).ForEach(t=>
        {
            t.TurnDone = false;
        });
    }
    
    private void UpdateIntents()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy.Alive && !enemy.TurnDone)
            {
                enemy.UpdateIntent();
            }
        }
        
        TurnResponders = TurnResponders.OrderBy(t => t.TurnPriority * (t.TurnDone ? 0 : 1) * (t.Alive ? 1 : 0)).ToList();
        IntentDisplayList.ForEach(e => e.sprite = IntentPlaceholder);
        var temp = TurnResponders.Where(t => t.Alive && !t.TurnDone).ToList();

        for (int i = 0; i < temp.Count; i++)
        {
            IntentDisplayList[i].sprite = ((Unit) temp[i]).icon;
            IntentDisplayList[i].gameObject.GetComponentInChildren<OnSelectPopup>().PopulateIntentPopup(temp[i].Intent);
        }
    }

   
    private void EndTurn()
    {
        
    }
    
    private void EndGame()
    {
        //SceneManager.UnloadSceneAsync("combat");
        
    }
    
    private void IncrementTurnText()
    {
        TurnNumber += 1;
        TurnNumberHolder.text = TurnNumber.ToString();
        TurnNumberHolder.ForceMeshUpdate();
    }
    private Unit.Faction CheckGameOutcome()
    {
        var playersAlive = TurnResponders.Count(t => t.Alive && t.AllyFaction == Unit.Faction.Players);
        var AIAlive =  TurnResponders.Count(t => t.Alive && t.AllyFaction == Unit.Faction.AI);
        return playersAlive == 0 ? Unit.Faction.AI : AIAlive == 0 ? Unit.Faction.Players : Unit.Faction.None;
    }
}
