using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        using (_packet)
        {
            string id = _packet.ReadString();
            string msg = _packet.ReadString();

            ExternClient.instance.id = id;
            Debug.Log($"Mensaje del servidor: {msg}");
            ClientSend.WelcomeReceived(id);
        }
    }

    public static void Disconnect(Packet _packet)
    {
        Debug.Log("He llegado al disconnect del cliente");
        using (_packet)
        {
            string client = _packet.ReadString();

            ExternClient.instance.Disconnect();
        }
    }
}
