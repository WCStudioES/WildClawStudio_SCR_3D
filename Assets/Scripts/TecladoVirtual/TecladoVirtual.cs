using TMPro;
using UnityEngine;

public class TecladoVirtual : MonoBehaviour
{
    private bool esMovil = false;
    [SerializeField]private TMP_InputField campoDeTexto;
    private TouchScreenKeyboard keyboard;
    void Start()
    {
        esMovil = Application.isMobilePlatform && Application.platform == RuntimePlatform.WebGLPlayer;
    }

    public void abrirTecladoVirtual()
    {
        if (esMovil)
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);
        }
    }
    public void abrirTecladoVirtualPassword()
    {
        if (esMovil)
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true);
        }
    }

    void Update()
    {
        if (esMovil && TouchScreenKeyboard.visible)
        {
            if(keyboard != null && campoDeTexto.isActiveAndEnabled)
                campoDeTexto.text = keyboard.text;
        }
    }

}
