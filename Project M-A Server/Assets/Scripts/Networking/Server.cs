using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Server : MonoBehaviour
{
    public static Server instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public int maxClients;
    public int port = 4966;
    public IPAddress ip = IPAddress.Parse("192.168.1.139");
    public TcpListener socket;

    public ServerData data;

    private void Start()
    {
        data = new ServerData();
        socket = new TcpListener(ip, port);
        maxClients = 3;

        socket.Start();
        socket.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), null);

        Debug.Log($"Servidor iniciado en el puerto {port}");
    }

    public void ClientConnectCallback(IAsyncResult result)
    {
        TcpClient _clientConnected = socket.EndAcceptTcpClient(result);

        socket.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), null);

        if (data.clients.Count >= maxClients)
        {
            Debug.Log("Server is full!, impossible to connect.");
            return;
        }
        else
        {
            try
            {
                string newClientID = DateTime.Now.Second.ToString() + _clientConnected.Client.RemoteEndPoint.ToString();
                data.clients.Add(newClientID, new InsideClient(newClientID, _clientConnected));
                ServerSend.Welcome(newClientID, "Welcome to my M-A Server");
                Debug.Log($"Ahora hay {data.clients.Count} clientes conectados");
            }
            catch (Exception e)
            {
                Debug.Log($"Algo salió mal al inicializar el cliente en el servidor: {e}");
            }
        }
    }

    public void DisconnectClient(string _client)
    {
        data.clients.Remove(_client);
    }
}
