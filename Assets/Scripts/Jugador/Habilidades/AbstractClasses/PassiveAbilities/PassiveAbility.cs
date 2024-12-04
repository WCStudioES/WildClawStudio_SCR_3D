using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveAbility : Ability
{
    public PassiveType type;
    public enum PassiveType
    {
        StatBuff,
        OnHit,
        OnCollision,
        OnRotation
    }
}
