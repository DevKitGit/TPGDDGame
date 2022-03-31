using System;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.NavigationMoveEvent;

public class OptionsMenu : IMenuHandler
{
	public UI UI { get; set; }
	public VisualElement Element => UI.Root.Q("options-menu");

	public void BindControls()
	{
		// Element.Q<Toggle>("sfx").BindDirection(UI.NavigationButton, Direction.Up);
		// Element.Q<Toggle>("music").BindDirection(UI.NavigationButton, Direction.Down);
	}

	public void OnEnter()
	{
		UI.Navigation.SetNavbarText("These options are broken");
	}

	public void OnExit()
	{
	}
}