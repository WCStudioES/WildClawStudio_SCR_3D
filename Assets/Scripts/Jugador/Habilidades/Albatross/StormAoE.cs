using System;
using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormAoE : AreaDmg
{
    public override void OnHit(IDamageable target, NetworkedPlayer dmgDealer)
    {
        Debug.Log("ALGUIEN RECIBE POR TORMENTA");
        target.GetDamage(dmg, dmgDealer);
    }
}
