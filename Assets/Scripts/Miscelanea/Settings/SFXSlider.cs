using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour
{
    private Slider slider;
    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = SettingsManager.Instance.GetSFXVolume();
    }
    //public void OnValueChange(float value)
    //{
    //    SettingsManager.Instance.SetBrightness(value);
    //}
}
