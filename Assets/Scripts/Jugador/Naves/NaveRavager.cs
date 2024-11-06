using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NaveRavager : PlayerShip
{
    private void Start()
    {
        InitializeStats();
    }

    public override void InitializeStats()
    {
        shipName = "Ravager";
        description = "Está guapa";

        actualMaxHealth = 100;
        actualHealth = 100;
        movementSpeed = 50;
        rotationSpeed = 50;
        armor = 5;

        actualLevel = 1;
        actualExperience = 0;

        //skins;
        //chromas;
    }

    public override void FireProjectile()
    {
        throw new NotImplementedException();
    }

    public override void GainExperience()
    {
        throw new NotImplementedException();
    }

    public override void GetDamage(int damage)
    {
        throw new NotImplementedException();
    }

    public override void UseAbility()
    {
        throw new NotImplementedException();
    }


}
