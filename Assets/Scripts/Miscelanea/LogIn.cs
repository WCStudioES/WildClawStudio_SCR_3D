using System;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class LogIn : MonoBehaviour
{
    [SerializeField] private UI UIManager;
    [SerializeField]private TMP_InputField name;
    [SerializeField]private TMP_InputField password;
    [SerializeField] private GameObject MensajeDeError;
    [SerializeField]private GameObject PantallaDeRegistro;
    [SerializeField]private GameObject PantallaDeLogIn;
    [SerializeField]private Button BotonLogIn;
    [SerializeField] private Usuario usuario;
    

    //ACTIVA LA PANTALLA DE REGISTRO
    public void ActivarPantallaDeRegistro()
    {
        LimpiarPantalla();
        PantallaDeRegistro.SetActive(true);
        PantallaDeLogIn.SetActive(false);
    }
    
    //INTENTA INICIAR LA SESION DEL USUARIO
    public void IniciarSesion()
    {
            BotonLogIn.enabled = false;
            if(name.text != "")
                UIManager.ComprobarUsuarioServerRpc(name.text, password.text);
            else
                UsuarioInvalido();
    }

    //LIMPIA LOS CAMPOS DE TEXTO DE LA PANTALLA
    void LimpiarPantalla()
    {
        MensajeDeError.SetActive(false);
        name.text = "";
        password.text = "";
        BotonLogIn.enabled = true;
    }
    
    //SI EL USUARIO ES VALIDO, LO GUARDA E INICIA SESION
    public void UsuarioValido(string usuario)
    {
        BotonLogIn.enabled = true;
        this.usuario = Usuario.DeserializeUsuario(usuario);
        UIManager.IniciarSesion();
        LimpiarPantalla();
    }
    
    //SI EL USUARIO NO ES VALIDO LANZA UN MENSAJE DE ERROR
    public void UsuarioInvalido()
    {
        BotonLogIn.enabled = true;
        MensajeDeError.SetActive(true);
    }

}
