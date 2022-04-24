using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


public class CharacterSelectMenu : IMenuHandler
{
	public UI UI { get; set; }
	public VisualElement Element => UI.Root.Q("character-select-menu");
	
	private readonly List<VisualPlayer> _players = new ();

	private int LockedInCount => _players.Count(p => p.LockedIn);
	private int ValidCount => _players.Count(p => p.IsValid);

	private IEnumerator _countDown;

	public void BindControls()
	{
		
	}

	private void ResetChildren()
	{
		List<VisualElement> list = Element.Q("characters").Children().ToList();
		foreach (VisualElement element in list)
		{
			_players.Add(new VisualPlayer {Element = element});
			element.SetEnabled(false);
			element.style.backgroundImage = null;
		}
	}
	
	public void OnEnter()
	{
		_players.Clear();
		UI.Navigation.IsBackButtonEnabled = false;

		ResetChildren();
		InputManager.EnableJoining();
		InputManager.OnPlayerJoin += OnPlayerJoin;
		UI.Navigation.SetNavbarText("Press a button to join");
	}

	public void OnExit()
	{
		UI.Navigation.IsBackButtonEnabled = true;
		InputManager.OnPlayerJoin -= OnPlayerJoin;
		InputManager.DisableJoining(true);
		ResetChildren();
	}

	private void OnPlayerJoin(PlayerController controller)
	{
		var visualPlayer = _players.FirstOrDefault(e => e.Controller == null);
		if (visualPlayer != null)
		{
			AudioManager.Play("squish-2");
			visualPlayer.Controller = controller;
			controller.InitControlLookup();
			visualPlayer.Element.SetEnabled(true);
			ProcessNavigation(visualPlayer, null);
			visualPlayer.Controller.UICancel += (e => ProcessBack(visualPlayer, e));
			visualPlayer.Controller.UISubmit += (e => ProcessLockIn(visualPlayer, e));
			visualPlayer.Controller.UINavigate += (e => ProcessNavigation(visualPlayer, e));
		}
	}

	
	private void UnassignPlayerEvents()
	{
		foreach (var visualPlayer in _players)
		{
			if (visualPlayer.Controller == null) continue;
			visualPlayer.Controller.UICancel = delegate(InputAction.CallbackContext context) {  };
			visualPlayer.Controller.UISubmit  = delegate(InputAction.CallbackContext context) {  };
			visualPlayer.Controller.UINavigate  = delegate(InputAction.CallbackContext context) {  };
		}
	}
	
	void ProcessBack(VisualPlayer visualPlayer, InputAction.CallbackContext context)
	{
		if (!context.performed || visualPlayer.Controller == null)
		{
			return;
		}

		AudioManager.Play("back-1");
		if (visualPlayer.LockedIn)
		{
			visualPlayer.LockedIn = false;
			GameManager.Stop(_countDown);
			UI.Navigation.SetNavbarText("Press a button to join");
			_countDown = null;
		}
		else
		{
			visualPlayer.Reset();
		}
	}

	void ProcessLockIn(VisualPlayer visualPlayer, InputAction.CallbackContext context)
	{
		if (visualPlayer.LockedIn || !context.performed || visualPlayer.Controller == null || visualPlayer.Controller.PlayerAsset == null)
		{
			return;
		}

		visualPlayer.LockedIn = true;
		AudioManager.Play("squish-3");
		
		if (_countDown == null && LockedInCount == ValidCount && ValidCount > 0)
		{

			_countDown = GameManager.Start(DoCountDown());
		}
	}

	IEnumerator DoCountDown()
	{
		var seconds = 3;
		var failed = false;
		
		for (int i = 0; i < seconds; i++)
		{
			string num = i switch
			{
				0 => "THREE",
				1 => "TWO",
				2 => "ONE",
				_ => throw new ArgumentOutOfRangeException(),
			};
			
			UI.Navigation.SetNavbarText($"All players ready! {num}!");
			yield return new WaitForSeconds(1);
			if (LockedInCount != ValidCount || ValidCount <= 0)
			{
				failed = true;
				break;
			}
		}

		if (!failed)
		{
			UnassignPlayerEvents();
			GameManager.LoadMainScene();
		}

		_countDown = null;
	}

	void ProcessNavigation(VisualPlayer visualPlayer, InputAction.CallbackContext? context)
	{
		if (visualPlayer.LockedIn)
		{
			return;
		}
		
		var usedSausages = _players.Where(p => p.Controller != null).Select(p => p.Controller.PlayerAsset).ToList();
		var sausages = GameManager.Assets.PlayerAssets;
		
		if (context == null)
		{
			var availableSausages = sausages.Except(usedSausages).ToList();
			SetSausage(visualPlayer, availableSausages.FirstOrDefault());
			return;
		}

		if (!context.Value.performed)
		{
			return;
		}
		
		AudioManager.Play("squish-2");
		int dir = context.Value.ReadValue<Vector2>().x > 0 ? 1 : -1;

		int currentIndex = sausages.IndexOf(visualPlayer.Controller.PlayerAsset);
		for (int i = 0; i < sausages.Count; i++)
		{
			currentIndex = currentIndex + dir;
			currentIndex = currentIndex >= sausages.Count ? 0 : currentIndex;
			currentIndex = currentIndex < 0 ? sausages.Count - 1 : currentIndex;

			var currentSausage = sausages[currentIndex];
			if (!usedSausages.Contains(currentSausage))
			{
				SetSausage(visualPlayer, currentSausage);
				break;
			}
		}
	}

	private void SetSausage(VisualPlayer visualPlayer, PlayerAsset playerAsset)
	{
		visualPlayer.Controller.PlayerAsset = playerAsset;
		visualPlayer.Element.style.backgroundImage = playerAsset.Texture;
		playerAsset.Color = visualPlayer.Element.style.borderBottomColor.value;
	}
	
	private class VisualPlayer
	{
		private const string LockedInString = "player__locked-in";

		public void Reset()
		{
			Element.SetEnabled(false);
			Element.style.backgroundImage = null;
			if (Controller == null)
			{
				return;
			}
			InputManager.RemovePlayer(Controller);
			InputManager.Instance.InputSystem.enabled = false;
			InputManager.Instance.InputSystem.enabled = true;
		}
		
		public bool IsValid => Controller != null;
		public PlayerController Controller { get; set; }
		public VisualElement Element { get; set; }
		public bool LockedIn
		{
			get => Element?.ClassListContains(LockedInString) ?? false;
			set
			{
				switch (value)
				{
					case true: Element.AddToClassList(LockedInString);break;
					default: Element.RemoveFromClassList(LockedInString);break;
				}
			}
		}
	}
}