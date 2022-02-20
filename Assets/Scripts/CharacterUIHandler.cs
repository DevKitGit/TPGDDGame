using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterUIHandler : MonoBehaviour
{
    [SerializeField] private List<AbilitySlot> abilitySlots;
    [SerializeField] private SpriteRenderer characterArtSlot;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterDescription;
    // Start is called before the first frame update

    public void UpdatePlayerUI(Player player)
    {
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            abilitySlots[i].SetAbility(player.abilities[i]);
        }
        characterArtSlot.sprite = player.Sprite;
        characterName.text = player.Name;
        characterName.ForceMeshUpdate();
        characterDescription.text = player.description;
        characterDescription.ForceMeshUpdate();
    }
}
