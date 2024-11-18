using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavagerPassive : OnCollisionPassive
{
    public override void AbilityExecution()
    {
        Debug.Log("Ravager tankea el golpe porque es dios");
    }

    public override bool CheckAvailability()
    {
        return true;
    }
}
