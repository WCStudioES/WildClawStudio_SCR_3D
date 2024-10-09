using System;
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
using UnityEngine.UI;

namespace Test
{
    public class TestRelay : MonoBehaviour
    {

        public TMP_InputField InputField;
        public TextMeshProUGUI codigo;

        [SerializeField]private GameObject UIServidor;
        private async void Start()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        [ContextMenu("CreateRelay")]
        private async void CreateRelay(bool isHost)
        {
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

                if (isHost)
                    NetworkManager.Singleton.StartHost();
                else
                    NetworkManager.Singleton.StartServer();
                
                codigo.text = joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async void JoinRelay(string joinCode)
        {
            try
            {
                Debug.Log("Joining Relay with `" + joinCode + "´ joinCode");
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                
                RelayServerData relayServerData = new RelayServerData(joinAllocation, "wss");
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();
                
                codigo.text = joinCode;
                
                OcultarUIServidor();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
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

        //OCULTA LA UI DEL SERVIDOR A LOS CLIENTES
        private void OcultarUIServidor()
        {
            UIServidor.SetActive(false);
        }
    }
}