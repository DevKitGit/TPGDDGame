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
using UnityEngine.InputSystem.UI;
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
    public Image _BackgroundImage;

    public Audio onButtonSelect, onButtonPress;
    [SerializeField] private Audio OnTurnBegin, OnCombatWin, onCombatLose;
    private AudioSource _audioSource;
    private CombatTemplate _combatTemplate;
    private BannerAnimator _bannerAnimator;
    public void InitializeCombat(CombatTemplate template)
    {
        _combatTemplate = template;
        _audioSource = AudioManager.Play(template.audioClip, true, targetParent: gameObject);
        _BackgroundImage.sprite = template.Background;
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
                _tilemap.CellToWorld(_combatTemplate.playerTilePositions[index]) + _tilemap.tileAnchor,quaternion.identity, transform).GetComponent<Player>();
            player.gameObject.name = player.UnitName;
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
            var enemy = Instantiate(_combatTemplate.enemies[i], _tilemap.CellToWorld(_combatTemplate.enemyTilePositions[i]) + _tilemap.tileAnchor, quaternion.identity, transform).GetComponent<Enemy>();
            enemy.gameObject.name = enemy.UnitName;
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
                    if (TurnResponders[currentTurnIndex].AllyFaction != Unit.Faction.AI)
                    {
                        await _bannerAnimator.StartBanner(((Unit) TurnResponders[currentTurnIndex]).UnitName+attachment, 2f);
                    }
                    TurnResponders[currentTurnIndex].TurnDone = await TurnResponders[currentTurnIndex].DoTurn();
                }
                currentTurnIndex++;
                var factionWon = CheckGameOutcome();
                if (factionWon != Unit.Faction.None)
                {
                    EndGame(factionWon);
                    return;
                }
                UpdateIntents();
            }
            AudioManager.Play(OnTurnBegin, targetParent: gameObject);
            await _bannerAnimator.StartBanner($"Turn {TurnNumber+1} begins", OnTurnBegin.Clip.length);
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
    
    private async void EndGame(Unit.Faction faction)
    {
        if (faction == Unit.Faction.Players)
        {
            foreach (var playerInput in PlayerInput.all)
            {
                playerInput.SwitchCurrentActionMap("World");
                playerInput.ActivateInput();
            }
            InputManager.Instance.gameObject.GetComponent<InputSystemUIInputModule>().UnassignActions();
            InputManager.Instance.gameObject.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            InputManager.Instance.gameObject.GetComponent<InputSystemUIInputModule>().ActivateModule();
            Destroy(_audioSource.gameObject);
            AudioManager.Play(OnCombatWin, targetParent: gameObject);
            
            await _bannerAnimator.StartBanner("You are victorious!", OnCombatWin.Clip.length);
            var task = SceneManager.UnloadSceneAsync("combat");
            task.completed += TaskOncompleted;
        }
        else
        {
            Destroy(_audioSource.gameObject);
            AudioManager.Play(onCombatLose, targetParent: gameObject);
            foreach (var playerInput in PlayerInput.all)
            {
                playerInput.SwitchCurrentActionMap("World");
                playerInput.ActivateInput();
            }
            foreach (var playerInput in PlayerInput.all)
            {
                playerInput.GetComponent<PlayerController>().WorldInteract += QuitApplication;
                playerInput.GetComponent<PlayerController>().WorldCancel += QuitApplication;
            }
            await _bannerAnimator.StartBanner("You died.. Press  to exit", onCombatLose.Clip.length);
            Application.Quit();
        }
        
    }

    private void QuitApplication(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Application.Quit();
        }
    }

    private void TaskOncompleted(AsyncOperation obj)
    {
        FindObjectsOfType<GameObject>(true).FirstOrDefault(e => e.name == "WorldScene")?.SetActive(true);
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
