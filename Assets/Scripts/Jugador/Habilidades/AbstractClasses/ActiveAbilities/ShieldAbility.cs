using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShieldAbility : ActiveAbility
{
    [SerializeField] protected GameObject shield;
    [SerializeField] protected Transform shieldSpawn;

    private void Awake()
    {
        type = ActiveType.Shield;
    }
}
