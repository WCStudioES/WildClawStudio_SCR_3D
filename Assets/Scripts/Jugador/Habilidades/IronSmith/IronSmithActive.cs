using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSmithActiva : DashAbility
{
    public override void AbilityExecution()
    {
        StartCoroutine(PerformDash());
    }


}
