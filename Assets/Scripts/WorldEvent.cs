using UnityEngine;

[CreateAssetMenu(fileName = "WorldEvent",menuName = "Scriptable Objects/World Event")]
public class WorldEvent : ScriptableObject
{
    public Sprite sprite;
    public string headline;
    public string text;

    public enum ChoiceOptions
    {
        Fight,
        Loot,
        RunAway,
        None
    }
    public ChoiceOptions choice1 = ChoiceOptions.None;
    public ChoiceOptions choice2 = ChoiceOptions.None;
    public ChoiceOptions choice3 = ChoiceOptions.None;
    public CombatTemplate CombatTemplate;
}
    
