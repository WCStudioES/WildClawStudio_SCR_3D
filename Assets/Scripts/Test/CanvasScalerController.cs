using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerController : MonoBehaviour
{
    private CanvasScaler CS;
    private bool letterboxLastValue = false;
    
    void Start()
    {
        CS = GetComponent<CanvasScaler>();
    }

    void Update()
    {
        if (letterboxLastValue != KeepAspectRatio.letterBox)
        {
            letterboxLastValue = KeepAspectRatio.letterBox;
            changeScale();
        }
    }

    private void changeScale()
    {
        if (letterboxLastValue)
        {
            CS.matchWidthOrHeight = 0.0f;
        }
        else
        {
            CS.matchWidthOrHeight = 1.0f;
        }
    }
}
