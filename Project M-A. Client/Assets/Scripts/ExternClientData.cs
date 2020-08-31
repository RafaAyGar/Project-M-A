using System.Collections.Generic;

public class ExternClientData
{
    public delegate void PacketHandler(Packet _packet);
    public Dictionary<int, PacketHandler> packetHandlers;

    public ExternClientData()
    {
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        packetHandlers = new Dictionary<int, PacketHandler>();
        packetHandlers.Add((int) ServerPackets.Welcome, ClientHandler.Welcome);
        packetHandlers.Add((int)ServerPackets.Disconnect, ClientHandler.Disconnect);
    }

    public void Reset()
    {
        packetHandlers.Clear();
    }
}