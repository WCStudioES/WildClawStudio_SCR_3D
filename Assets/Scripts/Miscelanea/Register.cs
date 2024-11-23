using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    [SerializeField]private UI UIManager;
    [SerializeField]private TMP_InputField name;
    [SerializeField]private TMP_InputField password1;
    [SerializeField]private TMP_InputField password2;
    [SerializeField]private GameObject MensajeDeError1;
    [SerializeField]private GameObject MensajeDeError2;
    [SerializeField]private GameObject PantallaDeRegistro;
    [SerializeField]private GameObject PantallaDeLogIn;
    [SerializeField]private Button BotonRegistrar;
    [SerializeField] private Usuario usuario;
    [SerializeField] private OpcionesJugador opcionesJugador;
    
    //ACTIVA LA PANTALLA DE LOGIN
    public void ActivarPantallaDeLogIn()
    {
        LimpiarPantalla();
        PantallaDeLogIn.SetActive(true);
        PantallaDeRegistro.SetActive(false);
    }
    
    //INTENTA INICIAR LA SESION DEL USUARIO
    public void IniciarSesion()
    {
        BotonRegistrar.enabled = false;
        MensajeDeError1.SetActive(false);
        MensajeDeError2.SetActive(false);
        if(name.text == "")
            UsuarioInvalido();
        else
        {
            if (password1.text == password2.text && password1.text != "")
            {
                
                UIManager.CrearUsuarioServerRpc(name.text, password1.text);
            }
            else
            {
                MensajeDeError2.SetActive(true);
                BotonRegistrar.enabled = true;
            }
        }

    }

    //LIMPIA LOS CAMPOS DE TEXTO DE LA PANTALLA
    public void LimpiarPantalla()
    {
        MensajeDeError1.SetActive(false);
        MensajeDeError2.SetActive(false);
        name.text = "";
        password1.text = "";
        password2.text = "";
    }

    
    //SI EL USUARIO ES VALIDO, LO GUARDA E INICIA SESION
    public void UsuarioValido(string usuario)
    {
        BotonRegistrar.enabled = true;
        this.usuario = Usuario.DeserializeUsuario(usuario);
        UIManager.IniciarSesion();
        LimpiarPantalla();
        opcionesJugador.usuario = this.usuario;
    }
    
    //SI EL USUARIO NO ES VALIDO LANZA UN MENSAJE DE ERROR
    public void UsuarioInvalido()
    {
        BotonRegistrar.enabled = true;
        MensajeDeError1.SetActive(true);
    }
}
