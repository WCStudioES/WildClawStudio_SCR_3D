using System;
using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StormAoE : AreaDmg
{
    [SerializeField] private float slow;
    private PlayerShip playerShip;
    public bool isUpgraded;
    [SerializeField] private float upscaling;
    public override void OnHit(IDamageable target, NetworkedPlayer dmgDealer)
    {
        Debug.Log("Creando tormenta");
        target.GetDamage(dmg, dmgDealer);
        Debug.Log("Storm" + isUpgraded);
    }

    protected override void AdditionalEffectsOnEnter(IDamageable other)
    {
        if(other is NetworkedPlayer)
        {
            NetworkedPlayer player = other as NetworkedPlayer;
            if (player.nave != null && IsInServidor)
            {
                player.nave.maxSpeed -= slow;
            }
        }
    }

    protected override void AdditionalEffectsOnExit(IDamageable other)
    {
        if (other != null && other is NetworkedPlayer)
        {
            NetworkedPlayer player = other as NetworkedPlayer;
            if (player.nave != null && IsInServidor)
            {
                player.nave.maxSpeed += slow;
            }
        }
    }

    public override void ExtraBehaviour(float time)
    {
        if (isUpgraded)
        {
            transform.localScale += new Vector3(upscaling * time, 0, upscaling * time);
            Debug.Log("Mejroado" + isUpgraded);
        }
    }

    //public IEnumerator IncreaseScale()
    //{
    //    if (isUpgraded)
    //    {
    //        while (gameObject.activeInHierarchy)
    //        {
    //            yield return new WaitForSecondsRealtime(1f);
    //            transform.localScale+=new Vector3(upscaling , 0, upscaling);
    //            Debug.Log(upscaling + " escala " + transform.localScale + "daño" + dmg);
    //        }
    //    }
    //}
}