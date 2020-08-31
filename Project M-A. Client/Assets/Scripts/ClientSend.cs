using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend
{
    #region TCP SENDERS
    public static void SendTCPData(Packet _packet)
    {
        ExternClient.instance.tcp.SendData(_packet);
    }
    #endregion
    public static void WelcomeReceived(string _fromClient)
    {
        using (Packet _packet = new Packet((int) ClientPackets.WelcomeReceived))
        {
            _packet.Write(_fromClient);

            SendTCPData(_packet);
        }
    }

    internal static void Disconnect(string _fromClient, bool _clientFinished)
    {
        using (Packet _packet = new Packet((int)ClientPackets.Disconnect))
        {
            _packet.Write(_fromClient);
            _packet.Write(_clientFinished);

            SendTCPData(_packet);
        }
    }
}
