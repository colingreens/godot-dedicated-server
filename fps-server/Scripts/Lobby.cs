using System.Collections.Generic;
using Godot;

namespace Server.Scripts;
public partial class Lobby : Node3D
{
    public List<long> Clients { get; } = new();

    public enum GameState
    {
        IDLE = 0,
        LOCKED = 1,
        GAME = 2
    }

    public GameState CurrentState { get; set; } = GameState.IDLE;

    public void AddClient(long id)
    {
        Clients.Add(id);
    }

    public void RemoveClient(long id)
    {
        Clients.Remove(id);
    }
}
