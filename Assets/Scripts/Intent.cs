using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UI;

public class Intent
{
    public Intent(Unit origin, bool hidden, List<Tile> targets, List<Ability> abilities)
    {
        this.origin = origin;
        this.hidden = hidden;
        this.targets = targets;
        this.abilities = abilities;
    }
    
    //Which unit has the intent
    public Unit origin;

    public bool hidden;
    
    //Which Tiles are being targeted
    public List<Tile> targets;

    public List<Ability> abilities;
}