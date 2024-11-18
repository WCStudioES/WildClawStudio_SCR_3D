using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandoraPassive : StatBuffPassive
{
    private bool hasASecondPassed = true;
    public override void AbilityExecution()
    {
        networkedPlayer.GetHeal((int)buffQuantity, networkedPlayer);
    }

    public override bool CheckAvailability()
    {
        if(hasASecondPassed && networkedPlayer.actualHealth.Value < networkedPlayer.maxHealth.Value)
        {
            hasASecondPassed = false;
            Invoke("resetSecond", 1f);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void resetSecond()
    {
        hasASecondPassed = true;
    }
}