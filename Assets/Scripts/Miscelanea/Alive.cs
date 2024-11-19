using System;
using UnityEngine;
using Unity.Netcode;

public class Alive : NetworkBehaviour
{
  public float contador = 5.0f;
  public float contadorMax = 5.0f;

  public float contadorDeMensajes = 1.0f;
  public float contadorDeMensajesMax = 1.0f;
  [SerializeField] private GameObject jugador;

  private void Start()
  {
    contador = contadorMax;
    contadorDeMensajes = contadorDeMensajesMax;
  }

  private void Update()
  {

    DecreaseTime();
    
    if (contador < 0.0f)
    {
      if (IsServer)
      {
        Destroy(jugador);
      }
    }
    
    if (IsOwner && contadorDeMensajes <= 0f)
    {
      StillAliveServerRpc();
      contadorDeMensajes = contadorDeMensajesMax;
    }
  }

  private void DecreaseTime()
  {
      contador -= Time.deltaTime;
      if(IsOwner)
        contadorDeMensajes -= Time.deltaTime;
  }

  [ServerRpc]
  private void StillAliveServerRpc()
  {
    contador = contadorMax;
  }

}
