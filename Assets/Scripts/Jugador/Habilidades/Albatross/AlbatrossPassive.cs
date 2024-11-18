using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbatrossPassive : OnHitPassive
{
    [SerializeField] private int hpPercentThreshold = 30;
    [SerializeField] private int extraDmg = 10;
    private GameObject target;
    private NetworkedPlayer enemyPlayer;
    public override void AbilityExecution()
    {
        if (networkedPlayer.IsServer)
        {
            Debug.Log("Albatross ejecuta");
            enemyPlayer.GetDamage(extraDmg, networkedPlayer);
        }
    }

    public override bool CheckAvailability()
    {
        if(enemyPlayer == null) return false;

        if(enemyPlayer.actualHealth.Value <= enemyPlayer.maxHealth.Value * hpPercentThreshold/100)
        {
            Debug.Log("Albatross puede ejecutar");
            return true;
        }
        return false;
    }

    public override void OnHit(GameObject pTarget)
    {
        target = pTarget;
        enemyPlayer = target.GetComponentInParent<NetworkedPlayer>();
        Debug.Log("Albatross va a intentar ejecutar");
        Execute();
    }
}
