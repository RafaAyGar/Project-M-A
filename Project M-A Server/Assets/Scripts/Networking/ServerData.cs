using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerData
{
    public Dictionary<string, InsideClient> clients;

    public delegate void ServerHandlePacket(Packet packet);
    public Dictionary<int, ServerHandlePacket> clientPackets;
    
    public ServerData()
    {
        clients = new Dictionary<string, InsideClient>();
        InitializeClientPackets();
    }

    private void InitializeClientPackets()
    {
        clientPackets = new Dictionary<int, ServerHandlePacket>();

        clientPackets.Add((int) ClientPackets.WelcomeReceived, ServerHandler.WelcomeReceived);
        clientPackets.Add((int)ClientPackets.Disconnect, ServerHandler.Disconnect);
    }
}
