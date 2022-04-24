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
        var buttonIndexReached = 0;
        for (var i = 0; i < worldEvent.choices.Count; i++)
        {
            _buttons[i].GetComponentInChildren<TextMeshProUGUI>().SetText("");
            _buttons[i].gameObject.SetActive(false);

            if (worldEvent.choices[i].choiceOption == Choice.ChoiceOptions.None) continue;
            _buttons[buttonIndexReached].GetComponentInChildren<TextMeshProUGUI>().SetText(worldEvent.choices[worldEvent.choices.Count-1-i].choiceDescription);
            _buttons[buttonIndexReached].gameObject.SetActive(true);
            buttonIndexReached++;
        }
    }
    public void ButtonPressed(Button button)
    {
        var text = button.GetComponentInChildren<TextMeshProUGUI>().text;
        var choice = _worldEvent.choices.Find(c => c.choiceDescription == text);
        eventChoicePicked.Invoke(choice);
    }
}
