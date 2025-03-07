using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generador : MonoBehaviour
{
    public List<GameObject> zonasGOPrefabs;
    public List<GameObject> zonasGO = new List<GameObject>();
    public Zona zonaInicial;
    public List<Zona> zonasGeneradas = new List<Zona>();

    private void Awake()
    {
        foreach (var zona in zonasGOPrefabs)
        {
            zonasGO.Add(Instantiate(zona, transform));
        }
    }

    public void GenerarMapa()
    {
        if (zonaInicial == null)
            return;

        if (zonasGO.Count == 0 && zonasGeneradas.Count != 0)
        {
            zonasGeneradas.Remove(zonaInicial);
            zonaInicial.cerrarPuertas();
            foreach (var zona in zonasGeneradas)
            {
                zona.cerrarPuertas();
                zonasGO.Add(zona.gameObject);
            }

            zonasGeneradas.Clear();
        }

        zonasGeneradas.Add(zonaInicial);
        while (true)
        {
            if (zonasGO.Count == 0)
            {
                return;
            }
            bool zonaElegidaBool = false;
            Zona zonaElegida = zonaInicial.GetComponent<Zona>();
            while (!zonaElegidaBool)
            {
                zonaElegida = zonasGeneradas[Random.Range(0, zonasGeneradas.Count)];
                if (zonaElegida.puertaLibre())
                    zonaElegidaBool = true;
            }
            int puertaElegida = zonaElegida.elegirPuerta();
            List<GameObject> poolDeZonas = new List<GameObject>();
            foreach (var zona in zonasGO)
            {
                for(int i = 0; i < zona.GetComponent<Zona>().probability * 100; i++)
                {
                    poolDeZonas.Add(zona);
                }
            }
            GameObject nuevaZona = poolDeZonas[Random.Range(0, poolDeZonas.Count)];
            zonasGO.Remove(nuevaZona);
            switch (puertaElegida)
            {
                case 0:
                    nuevaZona.GetComponent<RectTransform>().localPosition = new Vector3(zonaElegida.GetComponent<RectTransform>().localPosition.x + zonaElegida.getSize(puertaElegida), zonaElegida.GetComponent<RectTransform>().localPosition.y , 0);
                    break;
                case 1:
                    nuevaZona.GetComponent<RectTransform>().localPosition = new Vector3(zonaElegida.GetComponent<RectTransform>().localPosition.x  , zonaElegida.GetComponent<RectTransform>().localPosition.y + zonaElegida.getSize(puertaElegida), 0);
                    break;
                case 2:
                    nuevaZona.GetComponent<RectTransform>().localPosition = new Vector3(zonaElegida.GetComponent<RectTransform>().localPosition.x - zonaElegida.getSize(puertaElegida), zonaElegida.GetComponent<RectTransform>().localPosition.y, 0);
                    break;
                case 3:
                    nuevaZona.GetComponent<RectTransform>().localPosition = new Vector3(zonaElegida.GetComponent<RectTransform>().localPosition.x  , zonaElegida.GetComponent<RectTransform>().localPosition.y - zonaElegida.getSize(puertaElegida), 0);
                    break;
            }

            bool seSolapan = false;
            foreach (Zona zona in zonasGeneradas)
            {
                if (zona.SeSolapan(nuevaZona.GetComponent<Zona>()))
                {
                    seSolapan = true;
                }
            }

            if (seSolapan)
            {
                nuevaZona.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                zonasGO.Add(nuevaZona);
                continue;
            }

            switch (puertaElegida)
            {
                case 0:
                    zonaElegida.puertas[0].Abrir();
                    nuevaZona.GetComponent<Zona>().puertas[2].Abrir();
                    break;
                case 1:
                    zonaElegida.puertas[1].Abrir();
                    nuevaZona.GetComponent<Zona>().puertas[3].Abrir();
                    break;
                case 2:
                    zonaElegida.puertas[2].Abrir();
                    nuevaZona.GetComponent<Zona>().puertas[0].Abrir();
                    break;
                case 3:
                    zonaElegida.puertas[3].Abrir();
                    nuevaZona.GetComponent<Zona>().puertas[1].Abrir();
                    break;
            }
            nuevaZona.GetComponent<Zona>().texto.text = zonasGeneradas.Count.ToString();
            zonasGeneradas.Add(nuevaZona.GetComponent<Zona>());
            ComprobarVecinos();
        }
    }

    private void ComprobarVecinos()
    {
        foreach (var zona in zonasGeneradas)
        {
            float zonaX = zona.GetComponent<RectTransform>().localPosition.x;
            float zonaY = zona.GetComponent<RectTransform>().localPosition.y;
            float size0 = zona.getSize(0);
            float size1 = zona.getSize(1);
            float size2 = zona.getSize(2);
            float size3 = zona.getSize(3);
            foreach (var vecino in zonasGeneradas)
            {
                if (zona != vecino)
                {
                    float vecinoX = vecino.GetComponent<RectTransform>().localPosition.x;
                    float vecinoY = vecino.GetComponent<RectTransform>().localPosition.y;
                    if (vecinoX == zonaX)
                    {
                        if (vecinoY == zonaY + size1)
                        {
                            zona.puertas[1].Abrir();
                        }

                        if (vecinoY == zonaY - size3)
                        {
                            zona.puertas[3].Abrir();
                        }
                    }

                    if (vecinoY == zonaY)
                    {
                        if (vecinoX == zonaX + size0)
                        {
                            zona.puertas[0].Abrir();
                        }

                        if (vecinoX == zonaX - size2)
                        {
                            zona.puertas[2].Abrir();
                        }
                    }
                }
            }
        }
    }
    
    
}
