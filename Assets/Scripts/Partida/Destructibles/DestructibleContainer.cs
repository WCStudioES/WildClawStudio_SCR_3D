using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class DestructibleContainer : NetworkBehaviour
{
    public DestructibleAsset[] destructibles;
    public GameObject[] containers;
    
    public void ActivarContenedores()
    {
        
        destructibles = GetComponentsInChildren<DestructibleAsset>(true);
        Debug.Log(destructibles.Length + " Destructibles "+ IsServer);
        
    }

    public void Activation(bool mode)
    {
        //destructibles = GetComponentsInChildren<DestructibleAsset>();
        //Debug.Log(destructibles.Length + " Destructibles "+ IsServer);
        if (IsServer)
        {
            Debug.Log(gameObject.name + " is activated");
            if (IsServer)
            {
                ActivationClientRpc(mode);
                if (mode)
                {
                    Debug.Log(gameObject.name + " is activated");
                    foreach (var a in containers)
                    {
                        a.SetActive(true);
                    }
                
                    //destructibles = GetComponentsInChildren<DestructibleAsset>();
                    //Debug.Log(destructibles.Length + " Destructibles "+ IsServer);
                }
                else
                {
                    Debug.Log(gameObject.name + " is deactivated");
                    foreach (var a in containers)
                    {
                        a.SetActive(false);
                    }
                    //destructibles = GetComponentsInChildren<DestructibleAsset>();
                    //Debug.Log(destructibles.Length + " Destructibles "+ IsServer);
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
