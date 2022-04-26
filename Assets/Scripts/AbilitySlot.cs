using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    [SerializeField] public Ability _slottedAbility;
    [SerializeField] public int _slottedAbilityIndex;
    [SerializeField] private Image _Abilityimage;
    [SerializeField] private TextMeshProUGUI _abilityCost;
    [SerializeField] private TextMeshProUGUI _abilityCharges;
    [SerializeField] private Button _button;
    [SerializeField] private OnSelectPopup popup;

    private void Start()
    {
        popup = GetComponentInChildren<OnSelectPopup>();
    }

    public void SetAbility(Ability ability)
    {
        _slottedAbility = ability;
        _button = GetComponent<Button>();
        _Abilityimage.sprite = ability.sprite;
        _abilityCost.text = ability.ChargeCost == 0 ? "∞" : ability.ChargeCost.ToString();
        
        _abilityCharges.text = $"{ability.ChargesCurrent.ToString()}/{ability.ChargesMax.ToString()}";
        
        popup.PopulateAbilityPopup(ability);
    }

    public void SetAvailable(bool available)
    {
        _button.interactable = available;
        _Abilityimage.color = available ? Color.white : Color.gray;
        _abilityCost.color = available ? Color.white : Color.gray;
        _abilityCharges.color = available ? Color.white : Color.gray;
    }
}
