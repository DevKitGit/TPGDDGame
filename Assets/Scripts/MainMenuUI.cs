using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    private Button _startCampaignButton, _quitButton, _creditsButton, _optionsButton,_buttonOptionsToMain;
    private VisualElement _root, _main, _startCampaign, _setUpControls, _optionsMenu;
    private SliderInt _volumeMaster, _volumeMusic;
    private Toggle _toggleVibrations,_toggleSubtitles;
    
    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        
        //main menu window
        _main = _root.Q<VisualElement>("menu-main");

        _setUpControls = _root.Q<Button>();
        _startCampaignButton = _root.Q<Button>("start-campaign-button");
        _startCampaignButton.clicked += MainToCampaignButtonClicked;
        
        _startCampaignButton = _root.Q<Button>("start-campaign-button");
        _startCampaignButton.clicked += MainToCampaignButtonClicked;
        
        _quitButton = _root.Q<Button>("quit-button");
        _quitButton.clicked += ExitGame;

        _creditsButton = _root.Q<Button>("credits-button");
        _creditsButton.clicked += MainToCreditsButton;
        
        _optionsButton = _root.Q<Button>("options-button");
        _optionsButton.clicked += MainToOptionsButton;

        //options window
        _optionsMenu = _root.Q<VisualElement>("menu-options");
        
        _volumeMaster = _root.Q<SliderInt>("volume-master");
        _volumeMaster.RegisterValueChangedCallback(MasterVolumeChangedCallback);

        _volumeMusic = _root.Q<SliderInt>("volume-music");
        _volumeMusic.RegisterValueChangedCallback(MusicVolumeChangedCallback);

        _toggleVibrations = _root.Q<Toggle>("toggle-vibrations");
        _toggleVibrations.RegisterValueChangedCallback(ToggleVibrationsCallback);
        
        _toggleSubtitles = _root.Q<Toggle>("toggle-subtitles");
        _toggleSubtitles.RegisterValueChangedCallback(ToggleSubtitlesCallback);

        _buttonOptionsToMain = _root.Q<Button>("button-options-to-main");
        _buttonOptionsToMain.clicked += OptionsToMainButton;
        
        //Start campaign menu
        _startCampaign = _root.Q<VisualElement>("menu-campaign");
    }

    private void MainToCreditsButton()
    {
        throw new NotImplementedException();
    }
    private void ToggleSubtitlesCallback(ChangeEvent<bool> evt)
    {
        throw new NotImplementedException();
    }
    private void ToggleVibrationsCallback(ChangeEvent<bool> evt)
    {
        throw new NotImplementedException();
    }
    private void MasterVolumeChangedCallback(ChangeEvent<int> evt)
    {
        throw new NotImplementedException();
    }
    private void MusicVolumeChangedCallback(ChangeEvent<int> evt)
    {
        throw new NotImplementedException();
    }

    private void DisableEverything()
    {
        _main.style.display = DisplayStyle.None;
        _startCampaign.style.display = DisplayStyle.None;
        _optionsMenu.style.display = DisplayStyle.None;
    }
    private void MainToOptionsButton()
    {
        DisableEverything();
        _optionsMenu.style.display = DisplayStyle.Flex;
        _volumeMaster.Focus();
    }


    private void OptionsToMainButton()
    {
        DisableEverything();
        _main.style.display = DisplayStyle.Flex;
        _optionsButton.Focus();
    }

    private void MainToCampaignButtonClicked()
    {
        _main.SetEnabled(false);
        _startCampaign.SetEnabled(true);
    }
    private static void ExitGame()
    {
        #if UNITY_EDITOR
          EditorApplication.ExitPlaymode();
          return;
        #endif
        Application.Quit(0);
    }
}
