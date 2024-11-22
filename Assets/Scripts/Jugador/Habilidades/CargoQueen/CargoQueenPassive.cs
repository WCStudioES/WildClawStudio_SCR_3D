using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoQueenPassive : OnHitPassive
{
    private Debris debris;
    public override void AbilityExecution()
    {
        if(networkedPlayer.IsServer)
        {
            Debug.Log("Cargo Queen se cura más del Derbis");
            networkedPlayer.GetHeal(debris.resToGive.Value, networkedPlayer);
        }
    }

    public override bool CheckAvailability()
    {
        if(debris != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnHit(GameObject target)
    {
        debris= target.GetComponent<Debris>();
        Execute();
    }
}
