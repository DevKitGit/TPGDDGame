using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Navigation
{
	public Navigation(UI ui, List<IMenuHandler> menuHandlers)
	{
		_menuHandlers = menuHandlers;
		_infoText = ui.Root.Q<Label>("navBarLabel");
		_backButton = ui.Root.Q<Button>("back");
		var element = ui.Root.Q("navigation");
		
		_backButton.BindValue(Back);
		BindNavigateFromBackButton(element);
		BindNavigateToBackButton(_menuHandlers);
		InputManager.Instance.InputSystem.cancel.action.performed += Back;
	}

	private readonly Label _infoText;
	private readonly Button _backButton;
	private readonly Stack<IMenuHandler> _history = new();
	private readonly List<IMenuHandler> _menuHandlers;

	public bool IsBackButtonEnabled
	{
		get => _backButton.enabledSelf;
		set => _backButton.SetEnabled(value);
	}

	public bool IsBackButtonDisplayed
	{
		get => _backButton.style.display == DisplayStyle.Flex;
		set => _backButton.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
	}


	public void NavigateTo<T>() where T : IMenuHandler
	{
		if (_history.Count > 0)
		{
			AudioManager.Play("squish-3");
		}

		SetNavbarText();
		var menu = GetHandler<T>();
		if (_history.TryPeek(out var last))
		{
			last.Activate(false);
		}

		_history.Push(menu);
		IsBackButtonDisplayed = _history.Count > 1;
		menu.Activate(true);
	}

	private void Back()
	{
		if (_history.Count <= 1)
		{
			return;
		}
		_history.Pop().Activate(false);
		_history.Peek().Activate(true);
		IsBackButtonDisplayed = _history.Count > 1;
		AudioManager.Play("sizzle-1");
	}

	private void Back(InputAction.CallbackContext context)
	{
		foreach (PlayerController playerController in InputManager.PlayerControllers)
		{
			if (playerController.PlayerInput.devices.Contains(context.control.device))
			{
				return;
			}
		}

		Back();
	}

	private IMenuHandler GetHandler<T>() where T : IMenuHandler => _menuHandlers.OfType<T>().FirstOrDefault();
	
	private void BindNavigateToBackButton(List<IMenuHandler> menus)
	{
		foreach (IMenuHandler menuHandler in menus)
		{
			var query = menuHandler.Element.Query<BindableElement>().Where(b => b.ClassListContains("button"));
			var first = query.First();
			first?.RegisterCallback<NavigationMoveEvent>(e => Callback(e, NavigationMoveEvent.Direction.Up));
		
			var last = query.Last();
			if (last != first)
			{
				last?.RegisterCallback<NavigationMoveEvent>(e => Callback(e, NavigationMoveEvent.Direction.Down));
			}
		}

		void Callback(NavigationMoveEvent e, NavigationMoveEvent.Direction dir)
		{
			if (!IsBackButtonDisplayed)
			{
				return;
			}
			
			if (InputManager.PlayerControllers.Any(c => c.Devices.Contains(InputManager.Instance.InputSystem.move.action.activeControl?.device)))
			{
				e.PreventDefault();
				return;
			}

			if (e.direction == dir)
			{
				e.PreventDefault();
				_backButton?.Focus();
			}
		}
	}

	private void BindNavigateFromBackButton(VisualElement navBar)
	{
		navBar.RegisterCallback<NavigationMoveEvent>(e =>
		{
			var target = e.direction switch
			{
				NavigationMoveEvent.Direction.Up =>  _history.Peek().Element.Query<BindableElement>().Last(), 
				NavigationMoveEvent.Direction.Down => _history.Peek().Element.Query<BindableElement>().First(), 
				_ => null,
			};
			
			target?.Focus();
			e.PreventDefault();
		});
	}

	public void SetNavbarText(string text = "")
	{
		if (_infoText != null)
		{
			_infoText.text = text.ToUpper();
		}
	}
}
