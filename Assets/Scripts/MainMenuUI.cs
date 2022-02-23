using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    private Button _startCampaignButton, _quitButton, _creditsButton, _optionsButton;
    
    private VisualElement _root, _mainScroll, _mainMenu, _startCampaignMenu, _optionsMenu;
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _startCampaignButton = _root.Q<Button>("start-campaign-button");
        _quitButton = _root.Q<Button>("quit-button");
        _creditsButton = _root.Q<Button>("credits-button");
        _optionsButton = _root.Q<Button>("options-button");
        
        _mainMenu = _root.Q<VisualElement>("menu-main");
        _startCampaignMenu = _root.Q<VisualElement>("menu-campaign");
        _optionsMenu = _root.Q<VisualElement>("menu-options");
        
        _startCampaignButton.clicked += StartCampaignButtonClicked;
        _optionsButton.clicked += OptionsButtonClicked;
        _quitButton.clicked += ExitGame;
        _startCampaignButton.Focus();
    }

    private void OptionsButtonClicked()
    {
        Debug.Log("options");
        _root.AddToClassList("main-scroll-left");
        _root.RemoveFromClassList("main-scroll-center");
    }


    private void StartCampaignButtonClicked()
    {
        _root.AddToClassList("main-scroll-right");
        _root.RemoveFromClassList("main-scroll-center");
    }
    private void ExitGame()
    {
        EditorApplication.ExitPlaymode();
        //Application.Quit(0);
    }

}
