using Unity.Netcode;
using UnityEngine;

public class MostrarControlesMoviles : NetworkBehaviour
{
    private bool esMovil = false;
    [SerializeField] private GameObject ControlesTactiles;

    void Start()
    {
        esMovil = Application.isMobilePlatform && Application.platform == RuntimePlatform.WebGLPlayer;
        if(esMovil && IsOwner)
            ControlesTactiles.SetActive(true);
    }
    
}
