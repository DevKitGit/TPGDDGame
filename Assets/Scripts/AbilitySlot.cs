using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class AbilitySlot : MonoBehaviour
{
    [SerializeField] public SpriteRenderer _spriteRenderer;
    [SerializeField] public TextMeshProUGUI _abilityCost;
    [SerializeField] public TextMeshProUGUI _abilityCharges;
    
    public void SetAbility(Ability ability)
    {
        _spriteRenderer.sprite = ability.sprite;
        _abilityCost.text = ability.ActionCost.ToString();
        _abilityCharges.text = $"{ability.ChargesCurrent.ToString()}/{ability.ChargesMax.ToString()}";
    }
}
