using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatButton : Button
{
    [SerializeField] public UnityEvent m_OnSelect = new();
    [SerializeField] public UnityEvent m_OnDeselect = new();

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        m_OnDeselect.Invoke();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        m_OnSelect.Invoke();
    }
}