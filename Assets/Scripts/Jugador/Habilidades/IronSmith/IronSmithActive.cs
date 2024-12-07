using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSmithActiva : DashAbility
{
    [SerializeField] private int UpgradedCdReduction;
    [SerializeField] private int UpgradedDmgIncreased;
    public override void AbilityExecution()
    {
        StartCoroutine(PerformDash());
    }

    public override void UpgradeAbility()
    {
        isUpgraded = true;
        networkedPlayer.UpdateAbilityUIClientRpc(Color.magenta);
        neededResQuantity-= UpgradedCdReduction ;
        dashDamage += UpgradedDmgIncreased;
        Debug.Log(dashDamage);
    }
        
    public override void ResetPartida()
    {
        Debug.Log(networkedPlayer.userName + " reseted Ability");
        networkedPlayer.UpdateAbilityUIClientRpc(Color.white);
        if (isUpgraded)
        {
            isUpgraded = false;
            neededResQuantity+= UpgradedCdReduction ;
            dashDamage -= UpgradedDmgIncreased ;
        }
    }

}
