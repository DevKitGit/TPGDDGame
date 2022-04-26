using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnSelectPopup : MonoBehaviour
{
    // Start is called before the first frame update
    private CombatButton _combatButton;
    private SpriteRenderer _spriteRenderer, dividerSpriteRenderer;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI header, description, leftBottom,leftDescriptor, rightBottom, rightDescriptor;
    
    public void SetupPopup()
    {
        _combatButton = transform.parent.GetComponent<CombatButton>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        dividerSpriteRenderer = GetComponentsInChildren<SpriteRenderer>().FirstOrDefault(e => e.name == "Square");
        _combatButton.m_OnSelect.AddListener(ShowPopup);
        _combatButton.m_OnDeselect.AddListener(HidePopup);
        HidePopup(null);
    }
    private void OnDestroy()
    {
        _combatButton.m_OnSelect.RemoveListener(ShowPopup);
        _combatButton.m_OnDeselect.RemoveListener(HidePopup);
    }

    public void ShowPopup(BaseEventData arg0)
    {
        if (image != null && image.sprite.name.Equals("SquareInsides"))
        {
            return;
        }
        _spriteRenderer.color = Color.white;
        dividerSpriteRenderer.color = Color.white;
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

    public void HidePopup(BaseEventData arg0)
    {
        _spriteRenderer.color = Color.clear;
        dividerSpriteRenderer.color = Color.clear;

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
        description.text = ability.Description;
        rightBottom.text = ability.ChargeCost == 0 ? "∞" : $"{ability.ChargesCurrent} / {ability.ChargesMax}";
        header.ForceMeshUpdate();
        description.ForceMeshUpdate();
        leftBottom.ForceMeshUpdate();
        rightBottom.ForceMeshUpdate();
    }

    public void PopulateIntentPopup(Intent intent)
    {
        if (!intent.origin.Alive) return;
        
        header.text = intent.origin.name;
        if (intent.origin is Player playerIntent)
        {
            description.text = playerIntent.description;
        }
        else
        {
            if (intent.targets[0].UnitOnTile == null)
            {
                description.text = $"{intent.origin.name} is trying to walk towards you!"; 
            }
            else
            {
                description.text = $"{intent.origin.name} intends to use {intent.abilities[0].Name} on the {intent.targets[0].UnitOnTile.UnitName}"; 
            }
                
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
