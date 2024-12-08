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

    protected override void AdditionalEffectsOnEnter(Collider other)
    {
        playerShip = other.GetComponent<PlayerShip>();

        if (playerShip != null && IsInServidor)
        {
            playerShip.shipController.maxSpeed -= slow;
        }
    }

    private void OnDestroy()
    {
        if (playerShip != null && IsInServidor)
        {
            playerShip.shipController.maxSpeed += slow;
            playerShip = null;
        }
    }

    protected void OnTriggerExit(Collider other)
    {

        if (playerShip != null && IsInServidor)
        {
            playerShip.shipController.maxSpeed += slow;
            playerShip = null;
        }
    }

    protected override void AdditionalEffectsOnStay(Collider other)
    {
    }

    public override void ExtraBehaviour(float time)
    {
        if (isUpgraded)
        {
            //transform.localScale+=new Vector3(upscaling * time, 0, upscaling * time);
            Debug.Log("Mejroado" + isUpgraded);
        }
    }

    public IEnumerator IncreaseScale()
    {
        if (isUpgraded)
        {
            while (gameObject.activeInHierarchy)
            {
                yield return new WaitForSecondsRealtime(1f);
                transform.localScale+=new Vector3(upscaling , 0, upscaling);
                Debug.Log(upscaling + " escala " + transform.localScale + "daño" + dmg);
            }
        }
    }
}