using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootProjectileAbility : ActiveAbility
{
    private void Awake()
    {
        type = ActiveType.ShootProjectile;
    }
}
