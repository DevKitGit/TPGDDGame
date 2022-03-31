using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
	public VisualElement Root { get; private set; }
	public Navigation Navigation { get; private set; }
	
	private readonly List<IMenuHandler> _menuHandlers = new();

	private void Start()
	{
		Root = GetComponent<UIDocument>().rootVisualElement;
		InputManager.Instance.EventSystem.SetSelectedGameObject(gameObject);
		SetupHandlers();
		SetupAudio();
		Navigation.NavigateTo<MainMenuHandler>();
	}

	private void SetupAudio()
	{
		void Move(NavigationMoveEvent evt)
		{
			string clipName = evt.direction switch
			{
				NavigationMoveEvent.Direction.Up => "squish-1",
				NavigationMoveEvent.Direction.Down => "squish-2",
				_ => null,
			};
			if (clipName != null)
			{
				AudioManager.Play(clipName);
			}
		}

		Root.RegisterCallback<NavigationMoveEvent>(Move);
	}

	private void SetupHandlers()
	{
		foreach (Type type in GetType().Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(IMenuHandler).IsAssignableFrom(t)))
		{
			_menuHandlers.Add(Activator.CreateInstance(type) as IMenuHandler);
		}

		foreach (IMenuHandler menuHandler in _menuHandlers)
		{
			menuHandler.UI = this;
			menuHandler.Element?.Display(false);
		}

		Navigation = new Navigation(this, _menuHandlers);
		
		foreach (IMenuHandler menuHandler in _menuHandlers)
		{
			menuHandler.BindControls();
		}
	}
	

}