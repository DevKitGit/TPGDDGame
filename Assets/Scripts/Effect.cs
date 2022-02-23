using UnityEngine;

public abstract class Effect
{
    public string Name;
    //red = physical, blue = magic, purple CC
    public Sprite EffectTextSprite;
    public int TurnsLeft;
    public int MinimumAmount;
    public int MaximumAmount;
    public bool IgnoreArmor;
    public Type type;

    public bool applyImmediately;
    //what was the damage calculated to be
    public int chosenAmount;
    public enum Type
    {
        None = 0,
        Physical = 1,
        Magic = 2,
        Stun = 3,
        Silence = 4,
        Slow = 5
    }
    

}