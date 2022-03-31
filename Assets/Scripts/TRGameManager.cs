
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class TRGameManager : MonoBehaviour
{
    private CombatManager _combatManager;
    [SerializeField] private Player _playerHolder;
    [SerializeField] private List<PlayerSo> _playerScriptableObjects;
    [SerializeField] private List<Player> _players;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        _combatManager = FindObjectOfType<CombatManager>();
        foreach (var player in _playerScriptableObjects)
        {
            var newPlayer = Instantiate(_playerHolder.gameObject, Vector3.zero, quaternion.identity).GetComponent<Player>();
            newPlayer.PopulateData(player);
            newPlayer.gameObject.name = player.displayName;
            _players.Add(newPlayer);
        }
    }
}