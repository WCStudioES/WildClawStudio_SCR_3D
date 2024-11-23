using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXfromButton : MonoBehaviour
{
    public void PlaySFXFromButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSFX();
        }
        else
        {
            Debug.LogError("AudioManager.Instance no está inicializado.");
        }
    }
}
