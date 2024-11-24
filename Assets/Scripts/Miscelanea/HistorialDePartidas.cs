using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HistorialDePartidas : MonoBehaviour
{
    private Historial historial;
    [SerializeField] private TextMeshProUGUI rivales;
    [SerializeField] private TextMeshProUGUI resultados;
    [SerializeField] private TextMeshProUGUI victorias;
    [SerializeField] private TextMeshProUGUI derrotas;

    [SerializeField] private OpcionesJugador oj;
    

    // Update is called once per frame
    void Update()
    {
            cargarHistorial();
    }

    void cargarHistorial()
    {
        historial = oj.usuario.historial;
        rivales.text = "";
        resultados.text = "";
        victorias.text = "";
        derrotas.text = "";
        for(int i = 0; i < historial.rival.Count; i++)
        {
            rivales.text += historial.rival[i] + "\n";
            if (historial.puntuacionPropia[i] > historial.puntuacionRival[i])
                resultados.text += "WIN";
            else
                resultados.text += "LOSS";
            resultados.text += ": " + historial.puntuacionPropia[i] + " - " + historial.puntuacionRival[i] + "\n";
        }

        victorias.text = historial.ganadas.ToString();
        derrotas.text = historial.perdidas.ToString();
    }
}
