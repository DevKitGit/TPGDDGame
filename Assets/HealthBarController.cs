using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private Slider Slider;
    // Start is called before the first frame update
    
    public void SetupSlider(Unit unit)
    {
        Slider = GetComponentInChildren<Slider>();
        UpdateSlider(unit);
    }

    public void UpdateSlider(Unit unit)
    {
        Slider.value = unit.LifeForce / unit.MaxLifeForce;
        if (unit.LifeForce == 0f)
        {
            Invoke(nameof(UnitDied), 0.3f);
        }
    }

    public void UnitDied()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
