using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIExtensions
{
	public static void Display(this VisualElement element, bool shouldDisplay)
	{
		if (element == null)
		{
			return;
		}
		
		element.style.display = shouldDisplay ? DisplayStyle.Flex : DisplayStyle.None;
	}

	public static void Replace(this VisualElement root, VisualElement oldElement, VisualElement newElement)
	{
		int index = root.IndexOf(oldElement);
		root.Insert(index, newElement);
		root.Remove(oldElement);
	}


	public static void BindValue(this Button button, Action action) => button.clicked += action;

	public static void Activate(this IMenuHandler instance, bool shouldDisplay)
	{
		if (instance == null)
		{
			return;
		}
		
		instance.Element?.Display(shouldDisplay);
		if (shouldDisplay)
		{
			instance.Element?.Query<BindableElement>().First()?.Focus();
		}
		Action action = shouldDisplay ? instance.OnEnter : instance.OnExit;
		action.Invoke();
	}

	public static void BindDirection(this VisualElement element, VisualElement target, params NavigationMoveEvent.Direction[] dirs)
	{
		if (element == null)
		{
			return;
		}
		element.RegisterCallback<NavigationMoveEvent>(e =>
		{
			if (InputManager.PlayerControllers.Any(c => c.Devices.Contains(InputManager.Instance.InputSystem.move.action.activeControl?.device)))
			{
				e.PreventDefault();
				return;
			}
			if (dirs?.ToList().Contains(e.direction) ?? false)
			{
				e.PreventDefault();
				target?.Focus();
			}
		});
	}

	public static void BorderColor(this VisualElement e, Color color)
	{
		e.style.borderBottomColor = color;
		e.style.borderLeftColor = color;
		e.style.borderRightColor = color;
		e.style.borderTopColor = color;
	}
}