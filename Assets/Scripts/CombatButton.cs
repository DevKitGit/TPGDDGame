using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatButton : Button
{
    [SerializeField] public UnityEvent<BaseEventData> m_OnSelect = new();
    [SerializeField] public UnityEvent<BaseEventData> m_OnDeselect = new();
    [SerializeField] public UnityEvent<BaseEventData> m_OnSubmit = new();
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        m_OnDeselect.Invoke(eventData);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        AudioManager.Play(FindObjectOfType<CombatManager>().onButtonSelect, targetParent: FindObjectOfType<CombatManager>().gameObject);

        m_OnSelect.Invoke(eventData);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        m_OnSubmit.Invoke(eventData);
    }

}