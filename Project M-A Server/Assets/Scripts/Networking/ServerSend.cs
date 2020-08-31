using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    public static void Welcome(string _clientID, string _msg)
    {
        using (Packet _packet = new Packet((int) ServerPackets.Welcome))
        {
            _packet.Write(_clientID);
            _packet.Write(_msg);

            SendTCPData(_clientID, _packet);
        }
    }

    public static void Disconnect(string _clientID)
    {
        using (Packet _packet = new Packet((int)ServerPackets.Disconnect))
        {
            _packet.Write(_clientID);

            SendTCPData(_clientID, _packet);
        }
    }

    public static void SendTCPData(string _clientID, Packet _packet)
    {
        Server.instance.data.clients[_clientID].tcp.SendData(_packet);
    }
}
