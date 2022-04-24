using System.Collections;
using System.Collections.Generic;
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
        abilitySlots.ForEach(abilitySlot => abilitySlot.SetAvailable(available));
        characterArtSlot.color = available ? Color.white : Color.gray;
        
        //characterName.color = available ? Color.white : Color.gray;
        //characterName.ForceMeshUpdate();
        //characterDescription.color = available ? Color.white : Color.gray;
        //characterDescription.ForceMeshUpdate();
    }

    public void ReceiveAbilityPress(int index)
    {
        _currentlyActivePlayer?.SetSelectedAbility(index);
    }
}
