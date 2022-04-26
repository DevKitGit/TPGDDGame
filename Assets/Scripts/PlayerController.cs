using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;

public class PlayerController : MonoBehaviour
{
    public PlayerAsset PlayerAsset { get;  set; }
    
    public PlayerInput PlayerInput { get; private set; }

    public ReadOnlyArray<InputDevice> Devices => PlayerInput.devices;
    
    private PlayerControls _controls; 
    
    private Dictionary<Guid, Action<InputAction.CallbackContext>> _lookup = new();
    
    public Action<InputAction.CallbackContext> CombatGridMove 
    {
        get => _lookup.ContainsKey(_controls.Combat.CombatGridMove.id) ? _lookup[_controls.Combat.CombatGridMove.id] : null;
        set => _lookup[_controls.Combat.CombatGridMove.id] = value; 
    }
    public Action<InputAction.CallbackContext> CombatGridMoveReset 
    {
        get => _lookup.ContainsKey(_controls.Combat.CombatGridMoveReset.id) ? _lookup[_controls.Combat.CombatGridMoveReset.id] : null;
        set => _lookup[_controls.Combat.CombatGridMoveReset.id] = value; 
    }
    public Action<InputAction.CallbackContext> CombatInteract 
    {
        get => _lookup.ContainsKey(_controls.Combat.CombatInteract.id) ? _lookup[_controls.Combat.CombatInteract.id] : null;
        set => _lookup[_controls.Combat.CombatInteract.id] = value; 
    }
    public Action<InputAction.CallbackContext> CombatCancel 
    {
        get => _lookup.ContainsKey(_controls.Combat.CombatCancel.id) ? _lookup[_controls.Combat.CombatCancel.id] : null;
        set => _lookup[_controls.Combat.CombatCancel.id] = value; 
    }
    public Action<InputAction.CallbackContext> CombatUINavigate 
    {
        get => _lookup.ContainsKey(_controls.Combat.CombatUINavigate.id) ? _lookup[_controls.Combat.CombatUINavigate.id] : null;
        set => _lookup[_controls.Combat.CombatUINavigate.id] = value; 
    }
    public Action<InputAction.CallbackContext> CombatUIEnter 
    {
        get => _lookup.ContainsKey(_controls.Combat.CombatUIEnter.id) ? _lookup[_controls.Combat.CombatUIEnter.id] : null;
        set => _lookup[_controls.Combat.CombatUIEnter.id] = value; 
    }

    
    public Action<InputAction.CallbackContext> WorldNavigate 
    {
        get => _lookup.ContainsKey(_controls.World.WorldNavigate.id) ? _lookup[_controls.World.WorldNavigate.id] : null;
        set => _lookup[_controls.World.WorldNavigate.id] = value; 
    }
    public Action<InputAction.CallbackContext> WorldInteract 
    {
        get => _lookup.ContainsKey(_controls.World.WorldInteract.id) ? _lookup[_controls.World.WorldInteract.id] : null;
        set => _lookup[_controls.World.WorldInteract.id] = value; 
    }
    public Action<InputAction.CallbackContext> WorldCancel 
    {
        get => _lookup.ContainsKey(_controls.World.WorldCancel.id) ? _lookup[_controls.World.WorldCancel.id] : null;
        set => _lookup[_controls.World.WorldCancel.id] = value; 
    }
    public Action<InputAction.CallbackContext> WorldUIEnter 
    {
        get => _lookup.ContainsKey(_controls.World.WorldUIEnter.id) ? _lookup[_controls.World.WorldUIEnter.id] : null;
        set => _lookup[_controls.World.WorldUIEnter.id] = value; 
    }
    
    public Action<InputAction.CallbackContext> WorldUINavigate 
    {
        get => _lookup.ContainsKey(_controls.WorldUI.WorldUINavigate.id) ? _lookup[_controls.WorldUI.WorldUINavigate.id] : null;
        set => _lookup[_controls.WorldUI.WorldUINavigate.id] = value; 
    }
    public Action<InputAction.CallbackContext> WorldUIInteract 
    {
        get => _lookup.ContainsKey(_controls.WorldUI.WorldUIInteract.id) ? _lookup[_controls.WorldUI.WorldUIInteract.id] : null;
        set => _lookup[_controls.WorldUI.WorldUIInteract.id] = value; 
    }

    public Action<InputAction.CallbackContext> UINavigate 
    {
        get => _lookup.ContainsKey(_controls.UI.Navigate.id) ? _lookup[_controls.UI.Navigate.id] : null;
        set => _lookup[_controls.UI.Navigate.id] = value; 
    }
    public Action<InputAction.CallbackContext> UISubmit 
    {
        get => _lookup.ContainsKey(_controls.UI.Submit.id) ? _lookup[_controls.UI.Submit.id] : null;
        set => _lookup[_controls.UI.Submit.id] = value; 
    }
    public Action<InputAction.CallbackContext> UICancel 
    {
        get => _lookup.ContainsKey(_controls.UI.Cancel.id) ? _lookup[_controls.UI.Cancel.id] : null;
        set => _lookup[_controls.UI.Cancel.id] = value; 

    }
    public Action<InputAction.CallbackContext> UIPoint = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIClick = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIScrollWheel = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIMiddleClick = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIRightClick = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UITrackedDevicePosition = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UITrackedDeviceOrientation = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIJoin = delegate(InputAction.CallbackContext context) {  };
    private bool uienabled;


    void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        InputManager.AddPlayer(this);
        var InputmanagerGO = InputManager.Instance.gameObject;
        var module =InputmanagerGO.GetComponent<InputSystemUIInputModule>();
        module.actionsAsset = PlayerInput.actions;
        
        //PlayerInput.uiInputModule = module;
        //PlayerInput.uiInputModule = (modules.Length == 1 && modules[0].actionsAsset.name != "PlayerControls") ? modules[0] : InputmanagerGO.AddComponent<InputSystemUIInputModule>();
        //PlayerInput.uiInputModule.actionsAsset = PlayerInput.actions;
        module.Process();
        //PlayerInput.uiInputModule.UpdateModule();
        //PlayerInput.neverAutoSwitchControlSchemes = true;

        //InitControlLookup();
    }
    public void InitControlLookup()
    {
        PlayerInput.onActionTriggered += OnActionTriggered;
        _controls = new PlayerControls();
        _controls.Disable();
        AssignActions();
        
    }
    
    private void EnableUIInput(bool enabled, GameObject target = null)
    {
        var inputModule = InputManager.Instance.gameObject.GetComponent<InputSystemUIInputModule>();
        uienabled = enabled;
        if (enabled)
        {
            inputModule.UnassignActions();
            inputModule.AssignDefaultActions();
            inputModule.actionsAsset = PlayerInput.actions;
            InputManager.Instance.EventSystem.SetSelectedGameObject(target);
            inputModule.ActivateModule();
        }
        else
        {
            inputModule.UnassignActions();
            InputManager.Instance.EventSystem.SetSelectedGameObject(null);
        }
    }

    private void UnassignActions()
    {
        PlayerInput.onActionTriggered -= OnActionTriggered;
        _lookup.Clear();
    }
    
    private void AssignActions()
    {
        //Combat
        
        SetActionWithID(_controls.Combat.CombatGridMove,CombatGridMove);
        SetActionWithID(_controls.Combat.CombatGridMoveReset,CombatGridMoveReset);
        SetActionWithID(_controls.Combat.CombatInteract,CombatInteract);
        SetActionWithID(_controls.Combat.CombatCancel, CombatCancel);
        SetActionWithID(_controls.Combat.CombatUINavigate, CombatUINavigate);
        SetActionWithID(_controls.Combat.CombatUIEnter, CombatUIEnter);

        SetActionWithID(_controls.World.WorldNavigate,WorldNavigate);
        SetActionWithID(_controls.World.WorldInteract,WorldInteract);
        SetActionWithID(_controls.World.WorldCancel,WorldCancel);
        SetActionWithID(_controls.World.WorldUIEnter,WorldUIEnter);
        
        SetActionWithID(_controls.WorldUI.WorldUIInteract, WorldUIInteract);
        SetActionWithID(_controls.WorldUI.WorldUINavigate, WorldUINavigate);
        
        SetActionWithID(_controls.UI.Navigate,UINavigate);
        SetActionWithID(_controls.UI.Submit,UISubmit);
        SetActionWithID(_controls.UI.Cancel,UICancel);
        SetActionWithID(_controls.UI.Point,UIPoint);
        SetActionWithID(_controls.UI.Click,UIClick);
        SetActionWithID(_controls.UI.ScrollWheel,UIScrollWheel);
        SetActionWithID(_controls.UI.MiddleClick,UIMiddleClick);
        SetActionWithID(_controls.UI.RightClick,UIRightClick);
        SetActionWithID(_controls.UI.TrackedDevicePosition,UITrackedDevicePosition);
        SetActionWithID(_controls.UI.TrackedDeviceOrientation,UITrackedDeviceOrientation);
        SetActionWithID(_controls.UI.Join,UIJoin);
    }
    
    public void SetActionWithID(InputAction inputActionGuidRef,Action<InputAction.CallbackContext> action)
    {
        action += delegate(InputAction.CallbackContext context) {  };
        _lookup[inputActionGuidRef.id] = action;
    }

    
    public void AssignAction(Action<InputAction.CallbackContext>inputAction, Action<InputAction.CallbackContext> action)
    {
        inputAction += action;
    }
    
    public void UnAssignAction(InputAction inputActionGuidRef, Action<InputAction.CallbackContext> action)
    {
        _lookup[inputActionGuidRef.id] -= action;
    }
    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        
        /*
        if (PlayerInput.currentActionMap == null || !PlayerInput.currentActionMap.Contains(obj.action))
        {
            return;
        }
        */
        _lookup[obj.action.id].Invoke(obj);
    }
}