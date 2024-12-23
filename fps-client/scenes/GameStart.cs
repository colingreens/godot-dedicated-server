using Godot;
using System;

namespace Game.Scenes;
public partial class GameStart : Node
{
    // References to UI elements
    private Label inLobby;
    private Label lobbyPlayers;
    private Label maxPlayers;

    private Button findLobby;
    private Button leaveLobby;
    private Button start;
    private Button quit;

    private Server _server;

    public override void _Ready()
    {
        _server = Server.Instance;

        // Initialize UI element references
        inLobby = GetNode<Label>("UI/Play Buttons/LobbyInfo/InLobby");
        lobbyPlayers = GetNode<Label>("UI/Play Buttons/LobbyInfo/LobbyPlayers");
        maxPlayers = GetNode<Label>("UI/Play Buttons/LobbyInfo/MaxPlayers");

        findLobby = GetNode<Button>("UI/Play Buttons/FindLobby");
        leaveLobby = GetNode<Button>("UI/Play Buttons/LeaveLobby");
        start = GetNode<Button>("UI/Play Buttons/Start");
        quit = GetNode<Button>("UI/Play Buttons/Quit");

        //connect to button events
        findLobby.Pressed += OnFindLobbyPressed;
        leaveLobby.Pressed += OnLeaveLobbyPressed;
        start.Pressed += OnStartPressed;
        quit.Pressed += OnQuitPressed;

        // Connect to server events
        _server.OnCantConnectToLobby += OnCantConnectToLobby;
        _server.OnLobbyClientsUpdated += OnLobbyClientsUpdated;

        inLobby.Text = "Not In Lobby";
    }

    private void OnFindLobbyPressed()
    {
        _server.TryConnectClientToLobby();
    }

    private void OnLeaveLobbyPressed()
    {
        //server call leave lobby
    }

    private void OnStartPressed()
    {
        // Replace with start logic
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }

    private void OnCantConnectToLobby()
    {
        GD.Print("Cannot connect to lobby: ");
    }

    private void OnLobbyClientsUpdated(int players, int maxPlayersConnected)
    {
        GD.Print($"Players: {players}, Max Players: {maxPlayersConnected}");
        // Update UI elements if needed
        lobbyPlayers.Text = players.ToString();
        maxPlayers.Text = maxPlayersConnected.ToString();
    }
}
