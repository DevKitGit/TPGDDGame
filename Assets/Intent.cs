using System.Collections.Generic;

public class Intent
{
    //Which unit has the intent
    public Unit origin;

    //Which units are being targeted
    public List<Unit> targets;

    public List<Ability> abilities;
}