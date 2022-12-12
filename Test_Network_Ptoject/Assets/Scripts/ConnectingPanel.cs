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

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Client"))
            {
                basicAuthenticator.SetNameClient(nameServer);
                basicAuthenticator.SetPassClient(passwordServer);
                manager.StartClient();
            }

            nameServer = GUILayout.TextField(nameServer);
            passwordServer = GUILayout.TextField(passwordServer);
            GUILayout.EndHorizontal();

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
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
            GUILayout.Label($"Connecting to {manager.networkAddress}..");
            if (GUILayout.Button("Cancel Connection Attempt"))
            {
                manager.StopClient();
            }
        }
    }


    private void StatusLabels()
    {
        if (NetworkServer.active && NetworkClient.active)
        {
            GUILayout.Label($"<b>Host</b>: running via {Transport.active}");
        }

        else if (NetworkServer.active)
        {
            GUILayout.Label($"<b>Server</b>: running via {Transport.active}");
        }

        else if (NetworkClient.isConnected)
        {
            GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.active}");
        }
    }

    private void StopButtons()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Host"))
            {
                manager.StopHost();
            }
        }

        else if (NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Client"))
            {
                manager.StopClient();
            }
        }

        else if (NetworkServer.active)
        {
            if (GUILayout.Button("Stop Server"))
            {
                manager.StopServer();
            }
        }
    }
}
