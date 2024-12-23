using System.Collections.Generic;
using Godot;

namespace FPSServer.Scripts;
public partial class Lobby : Node3D
{
    public List<long> Clients { get; } = new();

    public void AddClient(long id)
    {
        Clients.Add(id);
    }

    public void RemoveClient(long id)
    {
        Clients.Remove(id);
    }
}
