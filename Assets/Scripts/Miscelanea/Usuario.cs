using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
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
     
     //AÑADE AL HISTORIAL UNA PARTIDA
     public void guardarPartidaEnHistorial(string rivalS, string naveRivalS, string navePropiaS, int pPropia, int pRival)
     {
          historial.guardarPartida(rivalS, naveRivalS, navePropiaS, pPropia, pRival);
          
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
     public List<string> naveRival = new List<string>(5);
     public List<string> navePropia = new List<string>(5);
     public List<int> puntuacionPropia = new List<int>(5);
     public List<int> puntuacionRival = new List<int>(5);

     public void guardarPartida(string rivalS, string naveRivalS, string navePropiaS, int pPropia, int pRival)
     {
          if (rival.Count >= 5)
          {
               rival.RemoveAt(4);
               navePropia.RemoveAt(4);
               naveRival.RemoveAt(4);
               puntuacionPropia.RemoveAt(4);
               puntuacionRival.RemoveAt(4);
          }
          rival.Insert(0, rivalS);
          navePropia.Insert(0, navePropiaS);
          naveRival.Insert(0, naveRivalS);
          puntuacionPropia.Insert(0, pPropia);
          puntuacionRival.Insert(0, pRival);
     }
}
