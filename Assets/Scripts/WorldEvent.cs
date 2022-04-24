using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldEvent",menuName = "Scriptable Objects/World Event")]
public class WorldEvent : ScriptableObject
{
    public Sprite sprite;


    public List<Choice> choices;

    public CombatTemplate CombatTemplate;

    public WorldEvent nextEvent;
    
}

[Serializable]
public class Choice
{
    public enum ChoiceOptions
    {
        Fight,
        Loot,
        RunAway,
        NextChoice,
        None
    }
    public ChoiceOptions choiceOption;
    public string choiceDescription;

    public Choice(ChoiceOptions choiceOption, string choiceDescription)
    {
        this.choiceOption = choiceOption;
        this.choiceDescription = choiceDescription;
    }
}
