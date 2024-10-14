using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class OpcionesJugador : NetworkBehaviour
{
    [SerializeField] private bool ActivarUI = true;

    [SerializeField]private UI UIJugador;
    
    //AL INICIAR, BUSCA LA UI Y SE ASIGNA COMO JUGADOR
    void Start()
    {
        //SI ES EL DUEÑO, SE ACTIVA LA PANTALLA DE LOGIN
        if (IsOwner && ActivarUI)
                UIJugador.ActivarUI();
    }

}
