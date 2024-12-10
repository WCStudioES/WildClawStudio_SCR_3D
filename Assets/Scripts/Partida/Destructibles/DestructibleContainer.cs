using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class DestructibleContainer : NetworkBehaviour
{
    public DestructibleAsset[] destructibles;
    public GameObject[] containers;
    void Start()
    {
        destructibles = GetComponentsInChildren<DestructibleAsset>();
    }

    public void Activation(bool mode)
    {
        if (IsServer)
        {
            ActivationClientRpc(mode);
            if (mode)
            {
                foreach (var a in containers)
                {
                    a.SetActive(true);
                }
            }
            else
            {
                foreach (var a in containers)
                {
                    a.SetActive(false);
                }
            }
        }
    }

    [ClientRpc]
    private void ActivationClientRpc(bool mode)
    {
        if (mode)
        {
            foreach (var a in containers)
            {
                a.SetActive(true);
            }
        }
        else
        {
            foreach (var a in containers)
            {
                a.SetActive(false);
            }
        }
    }

}
