using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public Image brightnessPanel; // Panel negro

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string BrightnessKey = "Brightness";

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Cargar el volumen al inicio
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f); // Volumen por defecto: 1
        AudioManager.Instance.SetMusicVolume(savedMusicVolume);

        float savedSFXVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f); // Volumen por defecto: 1
        AudioManager.Instance.SetSFXVolume(savedSFXVolume);

        LoadBrightness();
    }

    //VOLUMEN
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
        AudioManager.Instance.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
        PlayerPrefs.Save();
        AudioManager.Instance.SetSFXVolume(volume);
    }


    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
    }

    //BRILLO

    public void SetBrightness(float pAlpha)
    {
        float alpha = Mathf.Abs(pAlpha);

        PlayerPrefs.SetFloat(BrightnessKey, alpha);
        PlayerPrefs.Save();

        Debug.Log("BrilloCambiado: " + alpha);
        Debug.Log(brightnessPanel == null);

        if (brightnessPanel != null)
        {
            Color color = brightnessPanel.color;
            color.a = alpha; // Cambia la opacidad
            brightnessPanel.color = color;
            Debug.Log(brightnessPanel.color.a);
        }
    }

    public float GetBrightness()
    {
        float toReturn = PlayerPrefs.GetFloat(BrightnessKey, 0f);
        if (toReturn > 0)
        {
            return (-1 * toReturn);
        }
        return toReturn;
    }

    public void LoadBrightness()
    {
        // Cargar el brillo
        float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, 0f); // Volumen por defecto: 1
        SetBrightness(savedBrightness);
    }
}