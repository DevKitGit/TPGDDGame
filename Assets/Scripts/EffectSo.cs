using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Objects/Effect", order = 1)]
public class EffectSo : ScriptableObject
{
    public string Name;
    //red = physical, blue = magic, purple CC
    public Sprite EffectTextSprite;
    public int EffectDurationInTurns;
    public int MinimumAmount;
    public int MaximumAmount;
    public Effect.EffectType effectType;
    public bool ApplyImmediately;
    public bool isEffectConditional;
    public List<EffectSo> EffectConditions;
}