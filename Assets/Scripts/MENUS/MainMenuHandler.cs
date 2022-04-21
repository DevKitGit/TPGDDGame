using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuHandler : IMenuHandler
{
	public UI UI { get; set; }
	public VisualElement Element => UI.Root.Q("main-menu");

	public void BindControls()
	{
		var optionsButton = Element.Q<Button>("options");
		optionsButton.BindValue(UI.Navigation.NavigateTo<OptionsMenu>);
		var newGameButton = Element.Q<Button>("newgame");
		newGameButton.BindValue(UI.Navigation.NavigateTo<CharacterSelectMenu>);
		var exitButton = Element.Q<Button>("exit");
		exitButton.BindValue(Application.Quit);
#if UNITY_EDITOR
		exitButton.BindValue(EditorApplication.ExitPlaymode);
#endif
	}

	public void OnEnter()
	{
		UI.Navigation.SetNavbarText("Have a nice day~");
	}

	public void OnExit()
	{
	}
}