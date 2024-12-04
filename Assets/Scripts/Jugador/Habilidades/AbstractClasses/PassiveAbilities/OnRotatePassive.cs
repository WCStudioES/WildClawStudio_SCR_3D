using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnRotatePassive : PassiveAbility
{
    private void Awake()
    {
        type = PassiveType.OnRotation;
    }

}
