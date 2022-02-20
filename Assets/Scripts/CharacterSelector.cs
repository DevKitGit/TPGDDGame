using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager;
    private List<Player> _availableCharacters;
    private List<PlayerInput> _playerInputs;
    private List<int> _beingHovered;

    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += AddPlayer;
        _playerInputManager.onPlayerLeft += RemovePlayer;
    }

    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= AddPlayer;
        _playerInputManager.onPlayerLeft -= RemovePlayer;
    }

    private void RemovePlayer(PlayerInput playerInput)
    {
        _playerInputs.Remove(playerInput);
    }

    private void AddPlayer(PlayerInput playerInput)
    {
        _playerInputs.Add(playerInput);
    }

    
    public void OnCharacterSelectEnter(Player player, PlayerInput playerInput)
    {
        
    }

    public void OnCharacterSelectExit(Player player, PlayerInput playerInput)
    {
        
    }
    
    public void OnCharacterHoverEnter(Player player, PlayerInput playerInput)
    {
        
    }

    public void OnCharacterHoverExit(Player player, PlayerInput playerInput)
    {
        
    }
}
