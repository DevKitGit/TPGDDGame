using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect
{
    public string Name;
    public Sprite EffectTextSprite;
    public int EffectDurationInTurns;
    public int MinimumAmount;
    public int MaximumAmount;
    public EffectType effectType;
    public bool ApplyImmediately;
    public bool isEffectConditional;
    public List<Effect> EffectConditions;
    public enum EffectType
    {
        None = 0,
        PhysicalDmg = 1,
        MagicDmg = 2,
        Stun = 3,
        Silence = 4,
        Slow = 5,
        Heal = 6,
        Move = 7,
        Status = 8
    }
    
    public Effect(EffectSo effectSo)
    {
        Name = effectSo.Name;
        EffectTextSprite = effectSo.EffectTextSprite;
        EffectDurationInTurns = effectSo.EffectDurationInTurns;
        MinimumAmount = effectSo.MinimumAmount;
        MaximumAmount = effectSo.MaximumAmount;
        effectType = effectSo.effectType;
        ApplyImmediately = effectSo.ApplyImmediately;
        isEffectConditional = effectSo.isEffectConditional;
        EffectConditions = effectSo.EffectConditions.Select(e => new Effect(e)).ToList();
    }
    

}

