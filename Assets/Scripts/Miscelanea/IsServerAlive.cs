using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsServerAlive : NetworkBehaviour
{
    public float contador = 5.0f;
    public float contadorMax = 5.0f;

    public float contadorDeMensajes = 1.0f;
    public float contadorDeMensajesMax = 1.0f;
    private void Start()
    {
        contador = contadorMax;
        contadorDeMensajes = contadorDeMensajesMax;
    }


    // Update is called once per frame
    void Update()
    {
        DecreaseTime();
    
        if(IsOwner)
            if (contador < 0.0f)
            {
                OnServerDisconnect();
            }
        
        if (contadorDeMensajes <= 0f)
        {
            StillAliveClientRpc();
            contadorDeMensajes = contadorDeMensajesMax;
        }    
    }
    
    private void DecreaseTime()
    {
        if(IsOwner)
            contador -= Time.deltaTime;
        contadorDeMensajes -= Time.deltaTime;
    }
    
    [ClientRpc]
    private void StillAliveClientRpc()
    {
        contador = contadorMax;
    }
    
    private void OnServerDisconnect()
    {
        if(!IsHost)
        {
            AudioManager.Instance.StopMusic();
            NetworkManager networkManager = GameObject.FindObjectOfType<NetworkManager>();
            NetworkManager.Singleton.Shutdown();
            if(networkManager.gameObject != null)
                GameObject.Destroy(networkManager.gameObject);      

            SceneManager.LoadScene("SampleScene");
        }
        
    }
    
    
    public override void OnDestroy()
    {
        if(IsOwner)
            OnServerDisconnect();   
    }
}
