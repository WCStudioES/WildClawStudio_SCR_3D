using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorial : MonoBehaviour
{
    public void CambiarAInstrucciones2()
    {
        if (IsOwner)
        {
            Instrucciones.SetActive(false);
            Instrucciones2.SetActive(true);
        }
    }
}
