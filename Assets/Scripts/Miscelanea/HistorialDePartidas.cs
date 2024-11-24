using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistorialDePartidas : MonoBehaviour
{
    [SerializeField] private Usuario usuario;
    private Historial historial;
    

    // Update is called once per frame
    void Update()
    {
        if (historial != usuario.historial)
        {
            cargarHistorial();
        }
    }

    void cargarHistorial()
    {
        historial = usuario.historial;
    }
}
