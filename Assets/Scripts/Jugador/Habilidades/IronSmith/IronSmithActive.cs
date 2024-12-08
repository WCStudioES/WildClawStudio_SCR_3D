using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class IronSmithActiva : DashAbility
{
    [SerializeField] private int UpgradedCdReduction;
    [SerializeField] private int UpgradedDmgIncreased;
    [SerializeField] private List<Transform> extraFires;

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

    public override void InitializeVFX()
    {
        foreach (Transform spawn in extraFires)
        {
            VFXPrefab newVFX = VFXManager.Instance.SpawnVFX(VFXManager.VFXType.shipFire, spawn.position, spawn.rotation, spawn);
            visualEffects.Add(newVFX);
        }
        ToggleVFX(false);
    }

}
