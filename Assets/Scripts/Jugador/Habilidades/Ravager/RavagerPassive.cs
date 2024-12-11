using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavagerPassive : OnCollisionPassive
{
    public int dmg = 5;
    public override void AbilityExecution()
    {
        Debug.Log("Ravager tankea el golpe porque es dios");
        if (networkedPlayer.IsServer)
        {
            if(target != null)
            {
                Debug.Log("Ravager destruye embistiendo");
                target.GetDamage(dmg + (int)( dmg * (float)networkedPlayer.dmgBalance.Value/100), networkedPlayer);
            }
        }
    }

    public override bool CheckAvailability()
    {
        if(actualResQuantity > neededResQuantity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
