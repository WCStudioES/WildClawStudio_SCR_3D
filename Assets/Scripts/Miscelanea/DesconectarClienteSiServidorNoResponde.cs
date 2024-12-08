using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DesconectarClienteSiServidorNoResponde : MonoBehaviour
{
    
    public float contador = 5.0f;
    public float contadorMax = 5.0f;

    public bool IniciarCuentaAtras = false;

    public OpcionesJugador oj;

    // Update is called once per frame
    void Update()
    {
        if (IniciarCuentaAtras && oj == null)
        {
            contador -= Time.deltaTime;
            if(contador <= 0)
                OnServerDisconnect();
        }
        else
        {
            contador = contadorMax;
        }
    }
    
    private void OnServerDisconnect()
    {
        AudioManager.Instance.StopMusic();
        NetworkManager networkManager = GameObject.FindObjectOfType<NetworkManager>();
        if(networkManager.gameObject != null)
            GameObject.Destroy(networkManager.gameObject);      
        SceneManager.LoadScene("SampleScene");     
    }
}
