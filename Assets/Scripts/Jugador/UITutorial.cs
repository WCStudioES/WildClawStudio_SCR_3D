using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UITutorial : NetworkBehaviour
{
    [SerializeField] private GameObject uiTutorial;
    [SerializeField] private GameObject[] uiPantallas;
    private int index = 0;
    
    public void CambiarPantalla(bool isNext)
    {
        if (IsOwner)
        {
            uiPantallas[index].SetActive(false);
            if (isNext)
            {
                if (index == uiPantallas.Length - 1)
                    index = 0;
                else
                    index++;
                
                uiPantallas[index].SetActive(true);
            }

            else
            {
                if (index == uiPantallas.Length - 1)
                    index = 0;
                else
                    index++;
                
                uiPantallas[index].SetActive(true);
            }
  
        }
    }
}
