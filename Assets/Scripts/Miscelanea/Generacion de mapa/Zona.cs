using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Zona : MonoBehaviour
{
    public int size;
    public float probability = 0.5f;
    public bool colocada = false;
    public List<Puerta> puertas;
    public TextMeshProUGUI texto;

    public bool puertaLibre()
    {
        foreach (var puerta in puertas)
        {
            if (puerta.disponible)
                return true;
        }

        return false;
    }

    public int elegirPuerta()
    {
        while(true)
        {
            int puerta = Random.Range(0, puertas.Count);
            if (puertas[puerta].disponible)
                return puerta;
        }
    }
    
    public float getSize(int puerta)
    {
        return size + puertas[puerta].size;
    }

    public void cerrarPuertas()
    {
        foreach (var puerta in puertas)
        {
            puerta.Cerrar();
        }
    }
    
    public bool SeSolapan(Zona otraZona)
    {
        Rect rect1 = new Rect(transform.position.x, transform.position.y, size, size);
        Rect rect2 = new Rect(otraZona.transform.position.x, otraZona.transform.position.y, otraZona.size, otraZona.size);

        return rect1.Overlaps(rect2);
    }
}
