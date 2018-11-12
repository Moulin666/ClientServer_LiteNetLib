using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using NetCommon.Codes;
using System.Linq;
using System.Net;
using System.Net.Sockets;


public class ClientNetEventListener : MonoBehaviour, INetEventListener
{
    #region Public variables

    public delegate void MoveContainer (long id, Vector3 newPosition);

    public event MoveContainer OnMove;

    /// <summary>
    /// Server Address.
    /// </summary>
    public string ServerAddress = "localhost";

    /// <summary>
    /// Server port.
    /// </summary>
    public int ServerPort = 15000;

    /// <summary>
    /// Connection key for connect.
    /// </summary>
    public string ConnectionKey = "TestServer";

    /// <summary>
    /// ClientNetEventListener singleton class.
    /// </summary>
    public static ClientNetEventListener Instance = null;

    public Dictionary<long, NetObject> NetObjects = new Dictionary<long, NetObject>();

    #endregion

    #region Private variables

    private NetManager _netManager;
    private NetPeer _serverPeer;

    /// <summary>
    /// List event net messages.
    /// </summary>
    protected List<NetMessage> NetMessageList = new List<NetMessage>();

    #endregion

    #region Unity methods

    private void Awake ()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        GatherMessageHandlers();
    }

    private void Start ()
    {
        _netManager = new NetManager(this);

        if (_netManager.Start())
        {
            _netManager.Connect(ServerAddress, ServerPort, ConnectionKey);
            Debug.Log("Client net manager started!");
        }
        else
            Debug.LogError("Could not start client net manager!");

        _netManager.UpdateTime = 15;
    }

    private void FixedUpdate ()
    {
        if (_netManager != null && _netManager.IsRunning)
            _netManager.PollEvents();
    }

    private void OnApplicationQuit ()
    {
        if (_netManager != null && _netManager.IsRunning)
            _netManager.Stop();
    }

    #endregion

    #region Implements of INetEventListener

    public void OnNetworkLatencyUpdate (NetPeer peer, int latency) { }

    public void OnNetworkError (IPEndPoint endPoint, SocketError socketErrorCode)
        => Debug.LogError($"OnNetworkError: {socketErrorCode}");

    public void OnNetworkReceiveUnconnected (IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType) { }

    public void OnConnectionRequest (ConnectionRequest request)
        => Debug.Log($"ConnectionRequest. RemoteEndPoint: {request.RemoteEndPoint}");

    public void OnPeerDisconnected (NetPeer peer, DisconnectInfo disconnectInfo)
        => Debug.Log(string.Format("Disconnected. DisconnectReason: {0} | ErrorCode: {1}",
            disconnectInfo.Reason, disconnectInfo.SocketErrorCode));

    public void OnNetworkReceive (NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        if (reader.RawData == null)
            return;

        //Debug.Log($"OnNetworkReceive: {reader.RawData.Length}");

        NetOperationCode operationCode = (NetOperationCode)reader.GetByte();

        var handlers = NetMessageList.Where(h => (byte)h.Code == (byte)operationCode);

        if (handlers == null || handlers.Count() == 0)
            Debug.LogFormat("Default NET handler: Operation code: {0}", operationCode);

        foreach (var handler in handlers)
            handler.Notify(reader);
    }

    public void OnPeerConnected (NetPeer peer)
    {
        _serverPeer = peer;

        Debug.Log("Connected. EndPoint: " + peer.EndPoint);
    }

    #endregion

    public void OnMoveEvent (long id, Vector3 newPosition) => OnMove?.Invoke(id, newPosition);

    /// <summary>
    /// Send operation to the server.
    /// </summary>
    /// <param name="dataWriter">Writer parameters</param>
    /// <param name="sendOptions">Options for sending</param>
    public void SendOperation (NetDataWriter dataWriter, DeliveryMethod deliveryMethod)
        => _serverPeer.Send(dataWriter, deliveryMethod);

    /// <summary>
    /// Get all net message handlers and sort it.
    /// </summary>
    public void GatherMessageHandlers ()
    {
        foreach (NetMessage message in Resources.LoadAll<NetMessage>(""))
            NetMessageList.Add(message);

        Debug.Log($"Load handlers... Found {NetMessageList.Count} handlers.");
    }
}
