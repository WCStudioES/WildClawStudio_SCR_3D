using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShieldAbility : ActiveAbility
{
    [SerializeField] protected GameObject shield;

    private void Awake()
    {
        type = ActiveType.Shield;
    }
}
