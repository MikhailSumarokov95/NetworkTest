using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Authenticators;

[DisallowMultipleComponent]
[AddComponentMenu("Network/Network Manager HUD")]
[RequireComponent(typeof(NetworkManager))]
public class ConnectingPanel : MonoBehaviour
{
    [SerializeField] private NetworkManager manager;
    [SerializeField] private BasicAuthenticator basicAuthenticator;

    public int offsetX;
    public int offsetY;

    private string nameServer = "nameServer";
    private string passwordServer = "password";

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        // client ready
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            if (GUILayout.Button("Client Ready"))
            {
                NetworkClient.Ready();
                if (NetworkClient.localPlayer == null)
                {
                    NetworkClient.AddPlayer();
                }
            }
        }

        StopButtons();

        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        if (!NetworkClient.active)
        {
            // Server + Client
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (GUILayout.Button("Host (Server + Client)"))
                {
                    basicAuthenticator.SetNameServer(nameServer);
                    basicAuthenticator.SetPassServer(passwordServer);
                    basicAuthenticator.SetNameClient(nameServer);
                    basicAuthenticator.SetPassClient(passwordServer);
                    manager.StartHost();
                }
            }

            // Client + IP
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Client"))
            {
                basicAuthenticator.SetNameClient(nameServer);
                basicAuthenticator.SetPassClient(passwordServer);
                manager.StartClient();
            }
            // This updates networkAddress every frame from the TextField
            //manager.networkAddress = GUILayout.TextField(manager.networkAddress);
            nameServer = GUILayout.TextField(nameServer);
            passwordServer = GUILayout.TextField(passwordServer);
            GUILayout.EndHorizontal();

            // Server Only
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // cant be a server in webgl build
                GUILayout.Box("(  WebGL cannot be server  )");
            }
            else
            {
                if (GUILayout.Button("Server Only"))
                {
                    basicAuthenticator.SetNameServer(nameServer);
                    basicAuthenticator.SetPassServer(passwordServer);
                    manager.StartServer();
                }
            }
        }
        else
        {
            // Connecting
            GUILayout.Label($"Connecting to {manager.networkAddress}..");
            if (GUILayout.Button("Cancel Connection Attempt"))
            {
                manager.StopClient();
            }
        }
    }


    private void StatusLabels()
    {
        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            GUILayout.Label($"<b>Host</b>: running via {Transport.active}");
        }
        // server only
        else if (NetworkServer.active)
        {
            GUILayout.Label($"<b>Server</b>: running via {Transport.active}");
        }
        // client only
        else if (NetworkClient.isConnected)
        {
            GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.active}");
        }
    }

    private void StopButtons()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Host"))
            {
                manager.StopHost();
            }
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Client"))
            {
                manager.StopClient();
            }
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            if (GUILayout.Button("Stop Server"))
            {
                manager.StopServer();
            }
        }
    }
}
