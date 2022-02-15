﻿using System.Collections.Generic;

public class Intent
{
    //Which unit has the intent
    public Unit origin;

    public bool hidden;
    
    //Which units are being targeted
    public List<CombatCell> targets;

    public List<Ability> abilities;
}