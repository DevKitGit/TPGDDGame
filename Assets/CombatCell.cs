using UnityEngine;

public class CombatCell
{
    public bool Walkable;
    public float MovementCost;
    public Unit Unit;

    public CombatCell(bool Walkable, float MovementCost)
    {
        this.Walkable = Walkable;
        this.MovementCost = MovementCost;
    }
    
    public void onFocusEnter()
    {
        
    }

    public void onFocusExit()
    {
        
    }

    public void onSelected()
    {
        
    }

}