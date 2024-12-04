using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSmithPassive : OnCollisionPassive
{
    public int maxDamage = 50;
    public override void AbilityExecution()
    {
        if(target != null)
        {
            target.GetDamage(Mathf.RoundToInt(Mathf.Abs(networkedPlayer.nave.rotationSpeed) * maxDamage/networkedPlayer.nave.maxRotationSpeed), networkedPlayer);
        }
    }

    public override bool CheckAvailability()
    {
        if(Mathf.Abs(networkedPlayer.nave.rotationSpeed) > networkedPlayer.nave.maxRotationSpeed * 4/6 && actualResQuantity >= neededResQuantity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
