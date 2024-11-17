using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnCollisionPassive : PassiveAbility
{
    public CollisionWith[] collidesWith;
    public GameObject collidedObject;
    public enum CollisionWith
    {
        Wall,
        Ship,
        Meteorite,
        Proyectile
    }
}
