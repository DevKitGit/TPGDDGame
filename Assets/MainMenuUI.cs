using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualElement _root;
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        var button = _root.Q<Button>("start-campaign-button");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
