using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandler
{
    public static void WelcomeReceived(Packet _packet)
    {
        string _fromClient = _packet.ReadString();

        Debug.Log($"El cliente {_fromClient} ha recibido correctamente la bienvenida");
    }

    public static void Disconnect(Packet _packet)
    {
        try
        {
            string _fromClient = _packet.ReadString();
            bool _clientFinished = _packet.ReadBool();

            Server.instance.data.clients[_fromClient].Disconnect(_clientFinished);
        }
        catch (Exception e)
        {
            Debug.Log($"Error al desconectar al cliente : {e}");
        }
    }
}
