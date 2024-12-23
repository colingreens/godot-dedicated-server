using Server.Scripts;
using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Server;
public partial class Server : Node
{

    const int PORT = 7777;
    const int MAX_CLIENTS = 64;
    const int MAX_LOBBIES = 1;
    const int MAX_PLAYERS_PER_LOBBY = 2;
    const int DISTANCE_BETWEEN_LOBBIES = 100;

    private ENetMultiplayerPeer peer;

    private List<Lobby> lobbies;
    private List<long> idle_clients;
    private List<Lobby> lobby_spots;

    public Server()
    {
        peer = new ENetMultiplayerPeer();
        lobbies = new List<Lobby>();
        lobby_spots = new List<Lobby>();
        idle_clients = new List<long>();
    }

    public override void _Ready()
    {
        var error = peer.CreateServer(PORT, MAX_CLIENTS);

        if (error != Error.Ok)
        {
            Debug.Print("Failed to start server ");
            return;
        }

        Debug.Print("Server has started");

        Multiplayer.MultiplayerPeer = peer;

        peer.PeerConnected += OnPeerConnected;
        peer.PeerDisconnected += OnPeerDisconnected;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void c_TryConnectClientToLobby()
    {
        var clientId = Multiplayer.GetRemoteSenderId();
        var lobby = GetNonFullLobby();

        if (lobby != null)
        {
            lobby.AddClient(clientId);
            idle_clients.Remove(clientId);
            GD.Print($"Client {clientId} connected to Lobby {lobby.Name} ");
            LobbyClientsUpdated(lobby);
            if (lobby.Clients.Count >= MAX_CLIENTS)
            {
                LockLobby(lobby);
            }
            return;
        }

        RpcId(clientId, nameof(s_ClientCantConnectToLobby));
    }



    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void s_ClientCantConnectToLobby()
    {
        GD.Print("Client cannot connect to lobby");
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void s_LobbyClientsUpdated(int connectedClients, int maxClients)
    {
        GD.Print("Clients in lobby updated");
    }

    public void LobbyClientsUpdated(Lobby lobby)
    {
        lobby.Clients.ForEach(client => RpcId(client, nameof(s_LobbyClientsUpdated), lobby.Clients.Count.ToString(), MAX_CLIENTS));
    }

    private void OnPeerConnected(long id)
    {
        idle_clients.Add(id);
        GD.Print($"Client {id} connected to server");
    }

    private void OnPeerDisconnected(long id)
    {
        var lobby = GetLobbyFromClientId(id);
        if (lobby == null)
        {
            return;
        }

        lobby.RemoveClient(id);
        LobbyClientsUpdated(lobby);
        if (lobby.Clients.Count < 1)
        {
            lobbies.Remove(lobby);
            lobby_spots.Remove(lobby);
            lobby.QueueFree();
        }

        idle_clients.Remove(id);

        GD.Print($"Client {id} disconnected from the server");
    }

    private void LockLobby(Lobby lobby)
    {
        lobby.CurrentState = Lobby.GameState.LOCKED;
        CreateLobbyOnClients(lobby);
        //TODO: add rpc back to clients?
    }

    private void CreateLobbyOnClients(Lobby lobby)
    {
        lobby.Clients.ForEach(client => RpcId(client, nameof(CreateLobbyOnClients), lobby.Name));
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void s_CreateLobbyOnClients(string lobbyName)
    {
        GD.Print("Create Lobby on clients");
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void s_LobbyClientsUpdated()
    {
        GD.Print("Clients in lobby updated");
    }

    private Lobby GetLobbyFromClientId(long id)
    {
        return lobbies.FirstOrDefault(lobby => lobby.Clients.Contains(id));
    }

    private void UpdateLobbySpots()
    {
        var notInLobbySpot = lobbies.Where(lobby => !lobby_spots.Contains(lobby)).ToList();

        lobby_spots.AddRange(notInLobbySpot);

        for (int i = 0; i < lobby_spots.Count; i++)
        {
            lobby_spots[i].GlobalPosition = Vector3.Up * DISTANCE_BETWEEN_LOBBIES * i;
        }
    }

    private Lobby GetNonFullLobby()
    {
        return lobbies.FirstOrDefault(lobby => lobby.Clients.Count < MAX_PLAYERS_PER_LOBBY && lobby.CurrentState == Lobby.GameState.IDLE) ?? CreateLobby();
    }

    private Lobby CreateLobby()
    {
        if (lobbies.Count > MAX_LOBBIES)
        {
            return null;
        }

        var newLobby = new Lobby();
        newLobby.Name = newLobby.GetInstanceId().ToString();
        lobbies.Add(newLobby);
        UpdateLobbySpots();
        AddChild(newLobby, true);
        return newLobby;
    }
}
