using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementAbility : ActiveAbility
{
    private void Awake()
    {
        type = ActiveType.MovementBuff;
    }

}
