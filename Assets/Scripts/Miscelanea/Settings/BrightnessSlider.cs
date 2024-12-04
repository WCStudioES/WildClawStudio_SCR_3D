using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSlider : MonoBehaviour
{
    private Slider slider;
    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = SettingsManager.Instance.GetBrightness();
    }
    public void OnValueChange(float value)
    {
        SettingsManager.Instance.SetBrightness(value);
    }
}
