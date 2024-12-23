using Godot;
using System;
using System.Diagnostics;

public partial class Server : Node
{
    const int PORT = 7777;
    const string ADDRESS = "127.0.0.1";

    private ENetMultiplayerPeer peer;

    public Server()
    {
        peer = new ENetMultiplayerPeer();
    }

    public override void _Ready()
    {
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


    private void OnConnectionFailed()
    {
        Debug.Print("Failed to Conenct to Server");
    }


    private void OnConnectedToServer()
    {
        Debug.Print("Connected to Server");
    }

}
