using UnityEngine;

[CreateAssetMenu(fileName = "WorldEvent",menuName = "Scriptable Objects/World Event")]
public class WorldEvent : ScriptableObject
{
    public Sprite sprite;
    public enum ChoiceOptions
    {
        Fight,
        Loot,
        RunAway,
        NextChoice,
        None
    }
    public ChoiceOptions choice1 = ChoiceOptions.None;
    public string choice1text = "";
    
    public ChoiceOptions choice2 = ChoiceOptions.None;
    public string choice2text = "";
    
    public ChoiceOptions choice3 = ChoiceOptions.None;
    public string choice3text = "";

    public CombatTemplate CombatTemplate;

    public WorldEvent nextChoice;
    
}
    
