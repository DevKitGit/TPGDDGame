using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class NodeEvent : MonoBehaviour
{
    [SerializeField] private GameObject eventPrefab;
    [SerializeField] private WorldEvent worldEvent;
    [SerializeField] private bool eventLocked = false;
    private Node node;
    private SplineFollower party;
    private bool eventExpended = false;
    private GameObject _instance;
    private PlayerControls controls;

    // Start is called before the first frame update
    void Start()
    {
        controls = new PlayerControls();
        controls.Disable();
        node = GetComponent<Node>();
        party = FindObjectOfType<SplineFollower>();
        party.onNode += HasReachedNode;
    }

    private void HasReachedNode(List<SplineTracer.NodeConnection> passed)
    {
        if (passed[0].node != node) return;
        if (party.GetPercent() == 1.0 || party.GetPercent() == 0.0)
        {
            if (eventLocked && !eventExpended)
            {
                DisplayLockEvent();
                //do stuff when event is locked
                return;
            }
            if (eventExpended) return;
            DisplayEvent();
            eventExpended = true;
            //do stuff when event is unlocked and not expended
        }
        
    }

    private void DisplayLockEvent()
    {
        Debug.Log("lock event");
    }

    private void DisplayEvent()
    {
        _instance = Instantiate(eventPrefab);
        _instance.GetComponent<EventUI>().SetupEventUI(worldEvent);
        _instance.GetComponent<EventUI>().eventChoicePicked += ReceiveChoice;
        SetPlayerControlsToUI(PlayerInput.all.ToList());
    }

    private void SetPlayerControlsToUI(List<PlayerInput> playerInput)
    {
        playerInput.ForEach(p => p.SwitchCurrentActionMap("UI"));
    }



    private void SetPlayerControlsToCombatUI(PlayerInput playerinput)
    {
        var inputSystem = (InputSystemUIInputModule) EventSystem.current.currentInputModule;
        inputSystem.move.Set(playerinput.actions.FindAction(controls.Combat.CombatUINavigate.id));
        inputSystem.leftClick.Set(playerinput.actions.FindAction(controls.Combat.CombatInteract.id));
        inputSystem.cancel.Set(playerinput.actions.FindAction(controls.Combat.CombatCancel.id));
    }
    private void ReceiveChoice(Choice choice)
    {
        _instance.GetComponent<EventUI>().eventChoicePicked -= ReceiveChoice;
        foreach (var playerInput in PlayerInput.all)
        {
            playerInput.SwitchCurrentActionMap("World");
        }
        Destroy(_instance);
        DoEvent(choice);
    }

    private void DoEvent(Choice choice)
    {
        switch (choice.choiceOption)
        {
            case Choice.ChoiceOptions.Fight:
                GameObject.FindWithTag("WorldScene").SetActive(false);
                var asyncOperation = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
                asyncOperation.completed += OnSceneChanged;
                //Initiate combat
                break;
            case Choice.ChoiceOptions.Loot:
                //Display loot screen
                break;
            case Choice.ChoiceOptions.RunAway:
                //do nothing essentially, maybe sometimes incur damage?
                break;
            case Choice.ChoiceOptions.NextChoice:
                worldEvent = worldEvent.nextEvent;
                DisplayEvent();
                break;
            case Choice.ChoiceOptions.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnSceneChanged(AsyncOperation asyncOperation)
    {
        FindObjectOfType<CombatManager>().InitializeCombat(worldEvent.CombatTemplate);
    }
}
