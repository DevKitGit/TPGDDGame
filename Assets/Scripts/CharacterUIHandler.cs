using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIHandler : MonoBehaviour
{
    [SerializeField] private List<AbilitySlot> abilitySlots;
    [SerializeField] private Image characterArtSlot;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterDescription;
    // Start is called before the first frame update
    [SerializeField] private Player _currentlyActivePlayer;
    [SerializeField] private AbilitySlot lastAbilitySlot;
    private void Start()
    {
        var combatButtons = abilitySlots.Select(abilitySlot => abilitySlot.GetComponent<CombatButton>()).ToList();
        combatButtons.ForEach(combatButton => combatButton.m_OnSelect.AddListener(delegate
        {
            ReceiveAbilityPress(combatButton.GetComponent<AbilitySlot>());
        }));
        lastAbilitySlot = abilitySlots[0];
    }

    public void UpdatePlayerUI(Player player)
    {
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            abilitySlots[i].SetAbility(player.abilities[i]);
        }
        characterArtSlot.sprite = player.icon;
        characterName.text = player.UnitName;
        characterName.ForceMeshUpdate();
        characterDescription.text = player.description;
        characterDescription.ForceMeshUpdate();
        
        _currentlyActivePlayer = player;
        SetAvailable(true);
    }

    public void SetAvailable(bool available)
    {
        if (!available)
        {
            abilitySlots.ForEach(abilitySlot => abilitySlot.SetAvailable(available));
        }
        else
        {
            abilitySlots.ForEach(abilitySlot => abilitySlot.SetAvailable(abilitySlot._slottedAbility.Castable()));
        }
        characterArtSlot.color = available ? Color.white : Color.gray;
        
        //characterName.color = available ? Color.white : Color.gray;
        //characterName.ForceMeshUpdate();
        //characterDescription.color = available ? Color.white : Color.gray;
        //characterDescription.ForceMeshUpdate();
    }
    
    public void ReceiveAbilityPress(AbilitySlot abilitySlot)
    {
        if (_currentlyActivePlayer != null)
        {
            if (_currentlyActivePlayer.abilities[abilitySlot._slottedAbilityIndex].Castable())
            {
                _currentlyActivePlayer.SetSelectedAbility(abilitySlot._slottedAbilityIndex);
                lastAbilitySlot.GetComponentInChildren<OnSelectPopup>().HidePopup(null);
            }
            lastAbilitySlot = abilitySlot;
        }
    }

    public void UnhidePopup(int index)
    {
        abilitySlots[index].GetComponentInChildren<OnSelectPopup>().ShowPopup(null);
    }
}
