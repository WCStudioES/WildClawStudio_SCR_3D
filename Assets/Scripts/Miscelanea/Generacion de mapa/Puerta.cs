using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puerta : MonoBehaviour
{
    public bool disponible = true;
    public float size;
    public bool abierta = false;
    public Image imagen;

    private void Start()
    {
        if(!abierta && imagen != null)
            imagen.color = Color.red;
    }

    public void Abrir()
    {
        abierta = true;
        disponible = false;
        if(imagen != null)
            imagen.color = Color.green;
    }

    public void Cerrar()
    {
        abierta = false;
        disponible = true;
        if(imagen != null)
            imagen.color = Color.red;
    }
    
    public Vector2 GetLocalPosition()
    {
        return this.GetComponent<RectTransform>().localPosition;
    }
}
