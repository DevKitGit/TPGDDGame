using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image _image;
    [SerializeField] private List<Button> _buttons;
    private WorldEvent _worldEvent;
    public Action<Choice> eventChoicePicked = delegate {  };

    public void SetupEventUI(WorldEvent worldEvent)
    {
        _buttons.Reverse();
        _worldEvent = worldEvent;
        _image.sprite = worldEvent.sprite;
        var choices =  worldEvent.choices.Where(e => e.choiceOption != Choice.ChoiceOptions.None).ToList();
        foreach (var button in _buttons)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().SetText("");
            button.gameObject.SetActive(false);
        }
        for (var i = 0; i < choices.Count; i++)
        {
            _buttons[i].GetComponentInChildren<TextMeshProUGUI>().SetText(choices[choices.Count-i-1].choiceDescription);
            _buttons[i].gameObject.SetActive(true);
        }

        InputManager.Instance.EventSystem.SetSelectedGameObject(_buttons.LastOrDefault(e => e.gameObject.activeInHierarchy)?.gameObject);
    }
    public void ButtonPressed(Button button)
    {
        var text = button.GetComponentInChildren<TextMeshProUGUI>().text;
        var choice = _worldEvent.choices.Find(c => c.choiceDescription == text);
        eventChoicePicked.Invoke(choice);
    }
}
