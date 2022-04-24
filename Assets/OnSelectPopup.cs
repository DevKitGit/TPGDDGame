using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OnSelectPopup : MonoBehaviour
{
    // Start is called before the first frame update
    private CombatButton _combatButton;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshProUGUI header, description, leftBottom,leftDescriptor, rightBottom, rightDescriptor;
    
    public void SetupPopup()
    {
        _combatButton = transform.parent.GetComponent<CombatButton>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _combatButton.m_OnSelect.AddListener(ShowPopup);
        _combatButton.m_OnDeselect.AddListener(HidePopup);
        HidePopup();
    }
    private void OnDestroy()
    {
        _combatButton.m_OnSelect.RemoveListener(ShowPopup);
        _combatButton.m_OnDeselect.RemoveListener(HidePopup);
    }

    private void ShowPopup()
    {
        _spriteRenderer.color = Color.white;
        header.alpha = 1;
        header.ForceMeshUpdate();
        
        description.alpha = 1;
        description.ForceMeshUpdate();

        leftBottom.alpha = 1;
        leftBottom.ForceMeshUpdate();

        leftDescriptor.alpha = 1;
        leftDescriptor.ForceMeshUpdate();

        rightBottom.alpha = 1;
        rightBottom.ForceMeshUpdate();

        rightDescriptor.alpha = 1;
        rightDescriptor.ForceMeshUpdate();

    }

    private void HidePopup()
    {
        _spriteRenderer.color = Color.clear;
        header.alpha = 0;
        header.ForceMeshUpdate();
        
        description.alpha = 0;
        description.ForceMeshUpdate();

        leftBottom.alpha = 0;
        leftBottom.ForceMeshUpdate();

        leftDescriptor.alpha = 0;
        leftDescriptor.ForceMeshUpdate();

        rightBottom.alpha = 0;
        rightBottom.ForceMeshUpdate();

        rightDescriptor.alpha = 0;
        rightDescriptor.ForceMeshUpdate();
    }
    
    public void PopulateAbilityPopup(Ability ability)
    {
        if (ability.Effects.Count > 0)
        {
            var min = ability.Effects[0].MinimumAmount;
            var max = ability.Effects[0].MaximumAmount;
            leftBottom.text = min == max ? max.ToString() : $"{min.ToString()} - {max.ToString()}";
        }

        header.text = ability.Name;
        description.text = ability.Name;
        rightBottom.text = ability.ActionCost == 0 ? "∞" : $"{ability.ChargesCurrent} / {ability.ChargesMax}";
        header.ForceMeshUpdate();
        description.ForceMeshUpdate();
        leftBottom.ForceMeshUpdate();
        rightBottom.ForceMeshUpdate();
    }

    public void PopulateIntentPopup(Intent intent)
    {
        header.text = intent.origin.name;
        if (intent.origin is Player playerIntent)
        {
            description.text = playerIntent.description;
        }
        else
        {
            description.text = $"{intent.origin.name} intends to use {intent.abilities[0].Name} on {intent.targets[0]}";     
        }

        leftBottom.text = "";
        rightBottom.text = "";
        leftDescriptor.text = "";
        rightDescriptor.text = "";
        
        header.ForceMeshUpdate();
        description.ForceMeshUpdate();
        leftBottom.ForceMeshUpdate();
        rightBottom.ForceMeshUpdate();
        leftDescriptor.ForceMeshUpdate();
        rightDescriptor.ForceMeshUpdate();
    }
}
