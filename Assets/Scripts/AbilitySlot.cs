using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class AbilitySlot : MonoBehaviour
{
    [SerializeField] private Ability _slottedAbility;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshProUGUI _abilityCost;
    [SerializeField] private TextMeshProUGUI _abilityCharges;
    
    public void SetAbility(Ability ability)
    {
        _slottedAbility = ability;
        _spriteRenderer.sprite = ability.sprite;
        _abilityCost.text = ability.ActionCost.ToString();
        _abilityCharges.text = $"{ability.ChargesCurrent.ToString()}/{ability.ChargesMax.ToString()}";
    }
}
