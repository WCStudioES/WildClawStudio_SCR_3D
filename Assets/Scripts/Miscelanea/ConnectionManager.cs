using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Test;
using Unity.Netcode;
using UnityEngine.SceneManagement;

/// <summary>
/// Only attach this example component to the NetworkManager GameObject.
/// This will provide you with a single location to register for client
/// connect and disconnect events.  
/// </summary>
public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Singleton { get; internal set; }

    public float contador = 90;

    public enum ConnectionStatus
    {
        Connected,
        Disconnected
    }

    public int connectedClients = -1;

    /// <summary>
    /// This action is invoked whenever a client connects or disconnects from the game.
    ///   The first parameter is the ID of the client (ulong).
    ///   The second parameter is whether that client is connecting or disconnecting.
    /// </summary>
    public event Action<ulong, ConnectionStatus> OnClientConnectionNotification;

    private void Awake()
    {
        if (Singleton != null)
        {
            // As long as you aren't creating multiple NetworkManager instances, throw an exception.
            // (***the current position of the callstack will stop here***)
            throw new Exception($"Detected more than one instance of {nameof(ConnectionManager)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        Singleton = this;
    }

    private void Start()
    {
        if (Singleton != this){
            return; // so things don't get even more broken if this is a duplicate >:(
        }

        if (NetworkManager.Singleton == null)
        {
            // Can't listen to something that doesn't exist >:(
            throw new Exception($"There is no {nameof(NetworkManager)} for the {nameof(ConnectionManager)} to do stuff with! " +
                $"Please add a {nameof(NetworkManager)} to the scene.");
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        
    }

    private void Update()
    {
        if (connectedClients <= 0 && TestRelay.EsServidorPublico)
        {
            contador -= Time.deltaTime;
            if (contador <= 0)
            {
                contador = 90;
                ReiniciarServidor();
            }
        }
        else
        {
            contador = 30;
        }
    }
    
    private void ReiniciarServidor()
    {
        NetworkManager networkManager = GameObject.FindObjectOfType<NetworkManager>();
        networkManager.Shutdown();
        if(networkManager.gameObject != null)
            GameObject.Destroy(networkManager.gameObject);      
        SceneManager.LoadScene("SampleScene");
    }

    private void OnDestroy()
    {
        // Since the NetworkManager can potentially be destroyed before this component, only
        // remove the subscriptions if that singleton still exists.
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Connected);
        connectedClients++;
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Disconnected);
        connectedClients--;
    }
}
