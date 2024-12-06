using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Test
{
    public class TestRelay : MonoBehaviour
    {
        public TMP_InputField InputField;
        public TextMeshProUGUI codigo;

        [SerializeField]private GameObject UIServidor;

        public Button botonServidorPublico;
        
        public Button botonServidor;

        public Button botonCliente;

        public PublicServer publicServer = new PublicServer();

        public int maxQueueSize = 2048;

        public float TiempoContador = 2.0f;
        
        public float contador = 2.0f;

        public bool unidoAUnServidor = false;
        
        //public GameObject Fondo;
        private async void Start()
        {
            contador = TiempoContador;
            try
            {
                Request();
                
                await UnityServices.InitializeAsync();

                AuthenticationService.Instance.SignedIn += () =>
                {
                    Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
                };
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                Debug.Log(Application.persistentDataPath);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void Update()
        {
            if (!unidoAUnServidor)
            {
                contador -= Time.deltaTime;
                if (publicServer.joinCode != "AAAAAA")
                    botonServidorPublico.enabled = true;
                else
                    botonServidorPublico.enabled = false;
                if (contador <= 0)
                {
                    contador = TiempoContador;
                    try
                    {
                        Request();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                } 
            }
        }


        [ContextMenu("CreateRelay")]
        private async void CreateRelay(bool isHost)
        {
            botonServidor.enabled = false;
            botonCliente.enabled = false;
            botonServidorPublico.enabled = false;
            try
            {
                List<Region> Regions = await RelayService.Instance.ListRegionsAsync();
                foreach (var VARIABLE in Regions)
                {
                    //Debug.Log(VARIABLE.Id + "  " + VARIABLE.Description);
                }
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(100);
                
                
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                
                Debug.Log(joinCode);
                
                RelayServerData relayServerData = new RelayServerData(allocation, "wss");
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                
                //AJUSTAMOS EL MAXIMO NUMERO DE PAQUETES QUE SE PUEDEN TENER EN COLA
                NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPacketQueueSize = 8 * maxQueueSize;
                NetworkManager.Singleton.GetComponent<UnityTransport>().MaxSendQueueSize = 8 * maxQueueSize;

                if (isHost)
                {
                    NetworkManager.Singleton.StartHost();
                    //OcultarUIServidor(true);
                }
                else
                    NetworkManager.Singleton.StartServer();
                
                codigo.text = joinCode;
                setFrameLimit(true);

                Debug.Log("Tamaño de la cola de paquetes: " + NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPacketQueueSize);
                unidoAUnServidor = true;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                botonServidor.enabled = true;
                botonCliente.enabled = true;
                if(publicServer.joinCode != "AAAAAA")
                    botonServidorPublico.enabled = true;
            }
        }

        private async void JoinRelay(string joinCode)
        {
            botonServidor.enabled = false;
            botonCliente.enabled = false;
            botonServidorPublico.enabled = false;
            try
            {
                Debug.Log("Joining Relay with `" + joinCode + "´ joinCode");
                if (joinCode == "")
                    joinCode = "AAAAAA";
                
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                
                RelayServerData relayServerData = new RelayServerData(joinAllocation, "wss");
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                //AJUSTAMOS EL MAXIMO NUMERO DE PAQUETES QUE SE PUEDEN TENER EN COLA
                NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPacketQueueSize = 8 * maxQueueSize;
                NetworkManager.Singleton.GetComponent<UnityTransport>().MaxSendQueueSize = maxQueueSize;
                
                NetworkManager.Singleton.StartClient();
                
                codigo.text = joinCode;
                OcultarUIServidor(false);
                setFrameLimit(false);
                unidoAUnServidor = true;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                botonServidor.enabled = true;
                botonCliente.enabled = true;
                if(publicServer.joinCode != "AAAAAA")
                    botonServidorPublico.enabled = true;
            }
        }

        public void Host()
        {
            CreateRelay(true);
        }
        
        public void Server()
        {
            CreateRelay(false);
        }

        public void Client()
        {
            JoinRelay(InputField.text);
        }
        
        public void publicClient()
        {
            JoinRelay(publicServer.joinCode);
        }

        //OCULTA LA UI DEL SERVIDOR A LOS CLIENTES
        private void OcultarUIServidor(bool isHost)
        {
            UIServidor.SetActive(false);
            //if(!isHost)
                //Fondo.SetActive(false);
        }
        
        //ESTABLECE EL LIMITE DE FPS
        public static void setFrameLimit(bool server)
        {

            if (server)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 120;
            }

        }
        
        //PETICION DE SERVIDOR PUBLICO
        public void Request()
        {   UnityWebRequest publicServerRequest = UnityWebRequest.Get("https://jagonmes.github.io/PublicServerSCR3D/PublicServer.json");
            publicServerRequest.SendWebRequest();
            StartCoroutine(OnResponse(publicServerRequest));
        }

        private IEnumerator OnResponse(UnityWebRequest req)
        {
            yield return req;
            try
            {
                publicServer = JsonUtility.FromJson<PublicServer>(req.downloadHandler.text);
                if (req.result == UnityWebRequest.Result.InProgress)
                    StartCoroutine(OnResponse(req));
                else
                {
                    if(publicServer.joinCode != "AAAAAA" && publicServer.joinCode != "")
                        botonServidorPublico.enabled = true;
                    Debug.Log("Resultado de la petición: " + req.result + ".\nCódigo del servidor público: " + publicServer.joinCode);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            
        }
    }

    [Serializable]
    public class PublicServer
    {
        public string joinCode;

        public PublicServer()
        {
            joinCode = "AAAAAA";
        }
    }
}