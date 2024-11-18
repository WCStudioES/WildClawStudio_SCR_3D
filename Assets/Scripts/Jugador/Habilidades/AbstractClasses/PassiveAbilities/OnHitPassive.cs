using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHitPassive : PassiveAbility
{
    private void Awake()
    {
        type = PassiveType.OnHit;
    }

    public abstract void OnHit(GameObject target);
}
