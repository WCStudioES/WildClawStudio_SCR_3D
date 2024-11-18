using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoQueenActive : ShieldAbility
{
    public override void AbilityExecution()
    {
        shield.GetComponent<Shield>().Initialize(networkedPlayer);
    }

}
