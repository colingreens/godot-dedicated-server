using Godot;
using System;
using System.Diagnostics;

namespace Game.Scenes;
public partial class Server : Node
{
    public static Server Instance { get; private set; }

    const int PORT = 7777;
    const string ADDRESS = "127.0.0.1";

    public event Action<int, int> OnLobbyClientsUpdated;
    public event Action OnCantConnectToLobby;

    private ENetMultiplayerPeer peer;

    public Server()
    {
        peer = new ENetMultiplayerPeer();
    }

    public override void _Ready()
    {
        Instance = this;

        var error = peer.CreateClient(ADDRESS, PORT);

        if (error != Error.Ok)
        {
            Debug.Print("Failed to connect to server");
            return;
        }

        Multiplayer.MultiplayerPeer = peer;

        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void c_TryConnectClientToLobby()
    {
        GD.Print("Client requested to connect to a lobby");
    }

    public void TryConnectClientToLobby()
    {
        RpcId(1, nameof(c_TryConnectClientToLobby)); //server ID is always 1
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void s_ClientCantConnectToLobby()
    {
        OnCantConnectToLobby?.Invoke();
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void s_LobbyClientsUpdated(int connectedClients, int maxClients)
    {
        GD.Print($"{connectedClients} connected clients / {maxClients} max clients ");
        OnLobbyClientsUpdated?.Invoke(connectedClients, maxClients);
    }


    private void OnConnectionFailed()
    {
        Debug.Print("Failed to Conenct to Server");
    }


    private void OnConnectedToServer()
    {
        Debug.Print("Connected to Server");
    }

}
