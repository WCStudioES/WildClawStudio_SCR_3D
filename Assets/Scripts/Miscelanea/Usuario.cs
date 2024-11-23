using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[Serializable]
public class Usuario
{
     public string name;
     public string password;
     public Historial historial;
     

     private static string path = Application.persistentDataPath + "/";

     public Usuario(string name, string password)
     {
          this.name = name;
          this.password = password;
     }

     //SERIALIZA UN USUARIO
     public static string SerializeUsuario(Usuario usuario)
     {
          return JsonUtility.ToJson(usuario);
     }

     
     //DESERIALIZA UN USUARIO
     public static Usuario DeserializeUsuario(string usuario)
     {
          return JsonUtility.FromJson<Usuario>(usuario);
     }


     //COMPRUEBA SI UN USUARIO EXISTE
     public static bool ComprobarSiUsuarioExiste(string usuario)
     {
          return File.Exists(path + usuario);
     }
     
     //COMPRUEBA SI UN USUARIO ES VALIDO
     public static bool ComprobarSiUsuarioEsValido(string usuario, string password)
     {
          Usuario u = CargarUsuario(usuario);
          if(u != null)
               if (u.password == password)
                    return true;
          return false;
     }

     //CARGA UN USUARIO DE UN ARCHIVO
     public static Usuario CargarUsuario(string usuario)
     {
          Usuario u = null;
          try
          {
               string json = File.ReadAllText(path + usuario);
               u = DeserializeUsuario(json);
          }
          catch (Exception e)
          {
               Debug.Log(e.Message);
          }

          return u;
     }

     //GUARDA UN USUARIO A UN ARCHIVO
     public static void GuardarUsuario(Usuario usuario)
     {
          File.WriteAllText(path + usuario.name, SerializeUsuario(usuario));
     }
}

[Serializable]
public class Historial
{
     public List<string> rival = new List<string>(5);
     public List<int> puntuacionPropia = new List<int>(5);
     public List<int> puntuacionRival = new List<int>(5);
}
