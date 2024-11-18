using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public abstract class StatBuffPassive : PassiveAbility
{
    public BuffedStat buffedStat;
    public float buffQuantity;

    public enum BuffedStat
    {
        Hp,
        Energy,
        Armor,
        Damage,
        Speed
    }
}
