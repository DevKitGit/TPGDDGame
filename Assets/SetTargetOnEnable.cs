using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetTargetOnEnable : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
