using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CDReductionSupportItem : SupportItem
{
    private float reduction = 15;
    private float neededQuantity;
    public bool applied = false;
    public override void AddToPlayer()
    {
        if(!applied)
        {
            switch (owner.cuerpoNave.GetComponent<PlayerShip>())
            {
                case null: break;

                case NavePandora:neededQuantity = owner.cuerpoNave.GetComponent<PlayerShip>().activeAbility.neededResQuantity;
                    owner.cuerpoNave.GetComponent<PlayerShip>().activeAbility.neededResQuantity -= 1;
                    break;

                default:
                    neededQuantity = owner.cuerpoNave.GetComponent<PlayerShip>().activeAbility.neededResQuantity;
                    owner.cuerpoNave.GetComponent<PlayerShip>().activeAbility.neededResQuantity -= neededQuantity * reduction / 100;
                    break;
            }
            neededQuantity = owner.cuerpoNave.GetComponent<PlayerShip>().activeAbility.neededResQuantity;
            owner.cuerpoNave.GetComponent<PlayerShip>().activeAbility.neededResQuantity -= neededQuantity * reduction / 100;
        
            applied = true;
        }
    }
}
