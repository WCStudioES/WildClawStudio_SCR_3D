using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnCollisionPassive : PassiveAbility
{
    protected IDamageable target;
    public void CollideWith(Collision other)
    {
        Debug.Log("Te has chocado con una pasiva de chocarse");
        target = other.gameObject.GetComponentInParent<IDamageable>();
        Execute();
    }
}
