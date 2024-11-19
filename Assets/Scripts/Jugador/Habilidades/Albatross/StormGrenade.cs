using DefaultNamespace;
using DefaultNamespace.Proyectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormGrenade : Proyectil
{
    [SerializeField] private GameObject storm;
    [SerializeField] private Transform stormSpawn;
    private bool activo = true; //Variable apra ver si el misil ha explotado ya

    public override void OnHit(IDamageable target, NetworkedPlayer dmgDealer)
    {
        if (activo)
        {
            activo = false;

            Debug.Log("Creando tormenta en el server");

            var stormObject = Instantiate(storm, stormSpawn.position, Quaternion.identity);
            StormAoE explosionScript = stormObject.GetComponent<StormAoE>();
            explosionScript.CrearAreaDmg(CuerpoNaveDuena, dmgDealer, IsEnServidor);
            
        }
    }
}
