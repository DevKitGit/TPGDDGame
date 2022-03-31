using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class InputManager : MonoBehaviour
{
	[field:SerializeField]
	public InputSystemUIInputModule InputSystem {get; private set; }

	[field:SerializeField]
	public PlayerInputManager PlayerInputManager {get; private set; }

	[field:SerializeField]
	public EventSystem EventSystem {get; private set; }
	
	public static InputManager Instance { get; private set; }
    
	private readonly List<PlayerController> _controllers = new();

	public static event Action<PlayerController> OnPlayerJoin; 
	
	public static List<PlayerController> PlayerControllers => Instance._controllers;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		Instance = this;
		OnPlayerJoin = null;
	}

	public static void AddPlayer(PlayerController controller)
	{
		PlayerControllers.Add(controller);
		DontDestroyOnLoad(controller);
		OnPlayerJoin?.Invoke(controller);
	}

	public static void RemovePlayer(PlayerController controller)
	{
		PlayerControllers.Remove(controller);
		Destroy(controller.gameObject);
	}

	private static void RemoveAllPlayers()
	{
		foreach (var playerController in PlayerControllers)
		{
			Destroy(playerController.gameObject);
		}
		PlayerControllers.Clear();
	}
	
	public static void EnableJoining() => Instance.PlayerInputManager.EnableJoining();

	public static void DisableJoining(bool deletePlayers)
	{
		Instance.PlayerInputManager.DisableJoining();
		if (deletePlayers)
		{
			RemoveAllPlayers();
		}
	}

	private void OnDestroy()
	{
		foreach (PlayerController playerController in _controllers)
		{
			if (playerController == null)
			{
				continue;
			}
			Destroy(playerController.gameObject);
		}
	}
}