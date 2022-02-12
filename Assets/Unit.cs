using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit
{
    public string Name;
    public Intent Intent;
    public Sprite Sprite;
    public SpriteRenderer SpriteRenderer;
    
    
    
    // Implement what happens when turn begins/ends for this unit
    
    // Implement what happens when damage is taken/dealt for this unit
    public abstract void OnDamageTake();
    public abstract void OnDamageDeal();

    public void OnChangeDirection()
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
        
    }
    // Implement what happens 
    
}