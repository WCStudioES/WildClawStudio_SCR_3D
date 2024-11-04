using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRavagerShip", menuName = "PlayerShip/Ravager")]
public class NaveRavager : PlayerShip
{
    public override void InitializeStats()
    {
        shipName = "Ravager";
        description = "Está guapa";
        sprite = Resources.Load<Sprite>("Assets/Sprites/Ravager.png");

        actualMaxHealth = 100;
        actualHealth = 100;
        movementSpeed = 50;
        rotationSpeed = 50;
        armor = 5;

        actualLevel = 1;
        actualExperience = 0;

        shipModel = Resources.Load<GameObject>("Assets/Modelos 3D/Naves/Nave1SCR3D.fbx");
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

    public override void GetDamage(int damage, NetworkedPlayer jugador)
    {
        throw new NotImplementedException();
    }

    public override void UseAbility()
    {
        throw new NotImplementedException();
    }


}
