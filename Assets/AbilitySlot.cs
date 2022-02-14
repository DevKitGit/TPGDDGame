using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class AbilitySlot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshProUGUI _abilityCost;
    [SerializeField] private TextMeshProUGUI _abilityCharges;
    
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        var ability = new Ability();
        ability.sprite = _spriteRenderer.sprite;
        ability.ActionCost = 1;
        ability.ChargesCurrent = 1;
        ability.ChargesMax = 3;
        
        SetAbility(ability);
    }

    private void SetAbility(Ability ability)
    {
        _spriteRenderer.sprite = ability.sprite;
        _spriteRenderer.color = Color.black;
        _abilityCost.text = ability.ActionCost.ToString();
        _abilityCharges.text = $"{ability.ChargesCurrent.ToString()}/{ability.ChargesMax.ToString()}";
    }
}
