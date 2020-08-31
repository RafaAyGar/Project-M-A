using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

//Represents a extern client inside the server scope.
public class InsideClient
{
    private string id;
    public TCP tcp;

    public InsideClient(string _id, TcpClient _externClientSocket)
    {
        id = _id;
        tcp = new TCP();
        tcp.ConnectTCP(_externClientSocket);
    }

    public class TCP
    {
        public NetworkStream stream;
        TcpClient socket;
        byte[] receiveBuffer = new byte[4096];

        public void ConnectTCP(TcpClient client)
        {
            socket = client;
            stream = socket.GetStream();

            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReadCallback, null);
        }

        public void ReadCallback(IAsyncResult result)
        {
            try
            {
                if (!socket.Connected) return;

                int readLength = stream.EndRead(result);

                if (readLength <= 0) return;

                byte[] receivedData = new byte[readLength];
                Array.Copy(receiveBuffer, 0, receivedData, 0, readLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(receivedData))
                    {
                        int packetType = _packet.ReadInt();

                        Server.instance.data.clientPackets[packetType](_packet);
                    }
                });

                stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReadCallback, null);
            }
            catch (Exception e)
            {
                Debug.Log($"Error receiving TCP data : {e}");
            }
        }

        public void SendData(Packet _packet)
        {
            if (socket.Connected)
            {
                stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
            }
        }

        public void Disconnect()
        {
            socket.Close();
            stream.Close();
            Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
        }
    }

    public void Disconnect(bool _clientFinished)
    {
        try
        {
            if(!_clientFinished) ServerSend.Disconnect(id);
            tcp.Disconnect();
            Server.instance.DisconnectClient(id);
            id = null;
            Debug.Log($"El cliente {id} se ha desconectado");
        }
        catch (Exception e)
        {
            Debug.Log($"Error al desconectar al cliente {id} : {e}");
        }
    }
}
