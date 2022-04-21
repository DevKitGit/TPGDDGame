using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;

public class PlayerController : MonoBehaviour
{
    public PlayerAsset PlayerAsset { get;  set; }
    
    public PlayerInput PlayerInput { get; private set; }

    public ReadOnlyArray<InputDevice> Devices => PlayerInput.devices;
    
    private PlayerControls _controls; 
    
    private Dictionary<Guid, Action<InputAction.CallbackContext>> _lookup = new();
    
    //Combat Actions
    public Action<InputAction.CallbackContext> CombatMove = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> CombatMoveReset = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> CombatUIEnter = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> CombatInteract = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> CombatCancel = delegate(InputAction.CallbackContext context) {  };
    
    //World Actions
    public Action<InputAction.CallbackContext> WorldNavigate = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> WorldInteract = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> WorldCancel = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> WorldUIEnter = delegate(InputAction.CallbackContext context) {  };
    
    //Combat UI Actions
    public Action<InputAction.CallbackContext> CombatUINavigate = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> CombatUIInteract = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> CombatUICancel = delegate(InputAction.CallbackContext context) {  };
    
    //UI Actions
    public Action<InputAction.CallbackContext> UINavigate = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UISubmit = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UICancel = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIPoint = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIClick = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIScrollWheel = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIMiddleClick = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIRightClick = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UITrackedDevicePosition = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UITrackedDeviceOrientation = delegate(InputAction.CallbackContext context) {  };
    public Action<InputAction.CallbackContext> UIJoin = delegate(InputAction.CallbackContext context) {  };


    void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        InputManager.AddPlayer(this);
        PlayerInput.neverAutoSwitchControlSchemes = true;
        InitControlLookup();
    }
    private void InitControlLookup()
    {
        PlayerInput.onActionTriggered += OnActionTriggered;
        _controls = new PlayerControls();
        _controls.Disable();
        AssignActions();
    }

    private void UnassignActions()
    {
        PlayerInput.onActionTriggered -= OnActionTriggered;
        _lookup.Clear();
    }
    
    private void AssignActions()
    {
        //Combat
        SetActionWithID(_controls.Combat.CombatMove,CombatMove);
        SetActionWithID(_controls.Combat.CombatMoveReset,CombatMoveReset);
        SetActionWithID(_controls.Combat.CombatUIEnter,CombatUIEnter);
        SetActionWithID(_controls.Combat.CombatInteract,CombatInteract);
        SetActionWithID(_controls.Combat.CombatCancel, CombatCancel);
        
        SetActionWithID(_controls.World.WorldNavigate,WorldNavigate);
        SetActionWithID(_controls.World.WorldInteract,WorldInteract);
        SetActionWithID(_controls.World.WorldCancel,WorldCancel);
        SetActionWithID(_controls.World.WorldUIEnter,WorldUIEnter);
        
        SetActionWithID(_controls.CombatUI.CombatUINavigate,CombatUINavigate);
        SetActionWithID(_controls.CombatUI.CombatUIInteract,CombatUIInteract);
        SetActionWithID(_controls.CombatUI.CombatUICancel,CombatUICancel);
        
        SetActionWithID(_controls.UI.UINavigate,UINavigate);
        SetActionWithID(_controls.UI.UISubmit,UISubmit);
        SetActionWithID(_controls.UI.UICancel,UICancel);
        SetActionWithID(_controls.UI.UIPoint,UIPoint);
        SetActionWithID(_controls.UI.UIClick,UIClick);
        SetActionWithID(_controls.UI.UIScrollWheel,UIScrollWheel);
        SetActionWithID(_controls.UI.UIMiddleClick,UIMiddleClick);
        SetActionWithID(_controls.UI.UIRightClick,UIRightClick);
        SetActionWithID(_controls.UI.UITrackedDevicePosition,UITrackedDevicePosition);
        SetActionWithID(_controls.UI.UITrackedDeviceOrientation,UITrackedDeviceOrientation);
        SetActionWithID(_controls.UI.UIJoin,UIJoin);
    }
    
    private void SetActionWithID(InputAction inputActionGuidRef,Action<InputAction.CallbackContext> action)
    {
        _lookup[inputActionGuidRef.id] = action;
        
    }
    
    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (_lookup.TryGetValue(obj.action.id, out var action))
        {
            action.Invoke(obj);
        }
    }
}