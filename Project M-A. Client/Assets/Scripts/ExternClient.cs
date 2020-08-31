using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEditor;
public class ExternClient : MonoBehaviour
{
    public static ExternClient instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Debug.Log("A client instance already exists, deleting this...");
            Destroy(this.gameObject);
        }
    }

    public string id;
    [SerializeField] bool isConnected = false;

    public TCP tcp;
    public ExternClientData data;

    public void Connect()
    {
        if(isConnected)
        {
            Debug.Log("Ya estas conectado al servidor");
            return;
        }

        data = new ExternClientData();
        tcp = new TCP(IPAddress.Parse("192.168.1.139"), 4966);

        tcp.ConnectTCP();
        Debug.Log("Intentando conectar al servidor...");
    }


    public class TCP
    {
        public int port;
        public IPAddress tcpIP;
        public TcpClient socket;
        public NetworkStream stream;
        public byte[] receiveBuffer = new byte[4096];
        public byte[] sendBuffer = new byte[4096];

        public TCP(IPAddress _ip, int _port)
        {
            tcpIP = _ip;
            port = _port;
        }

        public void ConnectTCP()
        {
            socket = new TcpClient();
            socket.BeginConnect(tcpIP, port, new AsyncCallback(TCPConnectCallback), null);
        }

        void TCPConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);

            if (!socket.Connected)
            {
                Debug.Log("El socket no esta conectado!");
                return;
            }

            instance.isConnected = true;
            socket.ReceiveBufferSize = receiveBuffer.Length;
            socket.SendBufferSize = sendBuffer.Length;

            stream = socket.GetStream();
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(TCPReadCallback), null);
        }

        void TCPReadCallback(IAsyncResult result)
        {
            int dataLength = stream.EndRead(result);

            if (dataLength <= 0)
            {
                return;
            }

            HandleRead(dataLength);

            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(TCPReadCallback), null);
        }

        private void HandleRead(int dataLength)
        {
            byte[] receivedData = new byte[dataLength];
            Array.Copy(receiveBuffer, 0, receivedData, 0, dataLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(receivedData))
                {
                    int packetType = _packet.ReadInt();
                    instance.data.packetHandlers[packetType](_packet);
                }
            });
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
            Array.Clear(sendBuffer, 0, sendBuffer.Length);
            port = -1;
            tcpIP = null;
        }
    }

    private void OnApplicationQuit()
    {
        TryDisconnect(true);
    }

    public void TryDisconnect(bool finishNow = false)
    {
        if (isConnected == false) return;

        ClientSend.Disconnect(id, finishNow);

        if (finishNow)
        {
            Disconnect();
        }
    }

    public void Disconnect()
    {
        isConnected = false;
        id = "";
        data.Reset();

        tcp.Disconnect();
        tcp = null;

        Debug.Log("Cliente desconectado");
    }
}
