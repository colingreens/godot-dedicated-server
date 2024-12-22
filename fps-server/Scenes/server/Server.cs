using Godot;
using System;
using System.Diagnostics;

public partial class Server : Node
{

    const int PORT = 7777;
    const int MAX_CLIENTS = 64;

    private ENetMultiplayerPeer peer;

    public Server()
    {
        peer = new ENetMultiplayerPeer();
    }

    public override void _Ready()
    {
        var error = peer.CreateServer(PORT, MAX_CLIENTS);

        if (error != Error.Ok)
        {
            Debug.Print("Failed to start server ");
            return;
        }

        Multiplayer.MultiplayerPeer = peer;

        peer.PeerConnected += OnPeerConnected;
        peer.PeerDisconnected += OnPeerDisconnected;
    }

    private void OnPeerDisconnected(long id)
    {
        Debug.Print($"Client {id} disconnected from the server");
    }


    private void OnPeerConnected(long id)
    {
        Debug.Print($"Client {id} connected to server");
    }
}
