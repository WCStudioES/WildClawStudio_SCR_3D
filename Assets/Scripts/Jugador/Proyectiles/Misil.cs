using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Proyectiles;
using Unity.Netcode;
using UnityEngine;

public class Misil : Proyectil
{
    [SerializeField] private GameObject explosion; //Prefab de explosion para instanciar al petar
    [SerializeField] private Transform explosionSpawn;
    private bool activo = true; //Variable apra ver si el misil ha explotado ya
    
    public override void OnHit(IDamageable target, NetworkedPlayer dmgDealer)
    {
        if(activo)
        {
            activo = false;

            Debug.Log("Creando explosión en el server");

            var explosionObject = Instantiate(explosion, explosionSpawn.position, Quaternion.identity);
            Explosion explosionScript = explosionObject.GetComponent<Explosion>();
            explosionScript.dmg = dmg;
            explosionScript.CrearAreaDmg(CuerpoNaveDuena, dmgDealer, IsEnServidor);
        }
    }
}
