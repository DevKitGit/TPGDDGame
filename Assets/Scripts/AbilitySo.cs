using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability", order = 1)]

public class AbilitySo : ScriptableObject
{
    public string Name;
    public Sprite sprite;
    public int ActionCost;
    public int ChargesCurrent;
    public int ChargesMax;
    public List<EffectSo> Effects;
    public bool Ranged;
    public int Radius;
    public GameObject ProjectilePrefab;
    public GameObject TargetVisualEffect;
    public float ProjectileSpeed;
    [Range(0f, 1f)] public float AttackTimeNormalized;
}