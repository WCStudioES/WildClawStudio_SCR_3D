using System;
using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProyectilBasico : Proyectil
{
    // Start is called before the first frame update
    void Start()
    {
        dmg = 20;
        speed = 10f; 
        cadencia = 1.75f;
        type = Type.Simple;
    }

    public override void OnHit(IDamageable target, NetworkedPlayer dmgDealer)
    {
        if (IsEnServidor)
        {
            if(target != null)
            {
                target.GetDamage(this.dmg, dmgDealer);
            }
            Destroy(gameObject);
        }
    }

}
