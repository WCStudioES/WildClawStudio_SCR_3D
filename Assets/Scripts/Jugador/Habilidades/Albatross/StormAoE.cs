using System;
using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormAoE : AreaDmg
{
    public override void OnHit(IDamageable target, NetworkedPlayer dmgDealer)
    {
        Debug.Log("Creando tormenta");
        target.GetDamage(dmg, dmgDealer);
    }

    public override void ExtraBehaviour()
    {
        transform.localScale.Scale(new Vector3(1.1f, 1, 1.1f));
    }
}
