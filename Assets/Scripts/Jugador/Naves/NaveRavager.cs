using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NaveRavager : PlayerShip
{
    public override void InitializeStats()
    {
        shipName = "Ravager";
        description = "Está guapa";

        initialHealth = 100;
        initialArmor = 10;

        healthIncrement = 20;
        armorIncrement = 3;

        //skins;
        //chromas;
    }

    public override void FireProjectile()
    {
        throw new NotImplementedException();
    }

    public override void UseAbility()
    {
        throw new NotImplementedException();
    }


}
