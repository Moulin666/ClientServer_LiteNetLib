using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using NetCommon.Codes;
using NetCommon;


public class ClientNetEventListener : MonoBehaviour, INetEventListener
{
    #region Public variables

    public delegate void MoveContainer(bool isMine, Vector3 newPosition);
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

    #endregion

    #region Private variables
    
    private NetManager _netManager;
    private NetPeer _serverPeer;

    private readonly Dictionary<long, NetObject> _netObjects = new Dictionary<long, NetObject>();

    #endregion

    #region Unity methods

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _netManager = new NetManager(this, ConnectionKey);

        if (_netManager.Start())
        {
            _netManager.Connect(ServerAddress, ServerPort);
            Debug.Log("Client net manager started!");
        }
        else
            Debug.LogError("Could not start client net manager!");

        _netManager.UpdateTime = 15;
    }

    private void FixedUpdate()
    {
        if (_netManager != null && _netManager.IsRunning)
        {
            _netManager.PollEvents();
        }
    }

    private void OnApplicationQuit()
    {
        if (_netManager != null && _netManager.IsRunning)
            _netManager.Stop();
    }

    #endregion

    #region Implements of INetEventListener

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {  }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) {  }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode) => Debug.LogError($"OnNetworkError: {socketErrorCode}");

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        if (reader.Data == null)
            return;

        Debug.Log($"OnNetworkReceive: {reader.Data.Length}");

        NetOperationCode operationCode = (NetOperationCode)reader.GetByte();

        switch(operationCode)
        {
            case NetOperationCode.SpawnPlayerCode:
                {
                    PlayerData playerData = MessageSerializerService.DeserializeObjectOfType<PlayerData>(reader.GetString());

                    PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"), GameObject.Find("Spawn").transform.position, Quaternion.identity))
                        .GetComponent<PlayerController>();

                    NetObject netObject = newPlayer.gameObject.GetComponent<NetObject>();
                    netObject.Id = playerData.Id;
                    netObject.IsMine = playerData.IsMine;

                    _netObjects.Add(playerData.Id, netObject);

                    Debug.LogFormat("SpawnPlayer. PlayerId: {0}", playerData.Id);
                }
                break;

            case NetOperationCode.SpawnPlayersCode:
                {
                    int playerCount = reader.GetInt();

                    for (int i = 0; i < playerCount; i++)
                    {
                        var p = reader.GetString();

                        Debug.Log(p);

                        PlayerData playerData = MessageSerializerService.DeserializeObjectOfType<PlayerData>(p);

                        PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"), new Vector3(playerData.X, playerData.Y, playerData.Z), Quaternion.identity))
                            .GetComponent<PlayerController>();

                        NetObject netObject = newPlayer.gameObject.GetComponent<NetObject>();
                        netObject.Id = playerData.Id;
                        netObject.IsMine = playerData.IsMine;

                        _netObjects.Add(playerData.Id, netObject);
                    }

                    Debug.LogFormat("SpawnPlayers. Count: {0}", playerCount);
                }
                break;

            case NetOperationCode.WorldEnter:
                {
                    PlayerData playerData = MessageSerializerService.DeserializeObjectOfType<PlayerData>(reader.GetString());

                    PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"), GameObject.Find("Spawn").transform.position, Quaternion.identity))
                        .GetComponent<PlayerController>();

                    NetObject netObject = newPlayer.gameObject.GetComponent<NetObject>();
                    netObject.Id = playerData.Id;
                    netObject.IsMine = playerData.IsMine;

                    _netObjects.Add(playerData.Id, netObject);

                    Debug.LogFormat("WorldEnter. PlayerId: {0}", playerData.Id);
                }
                break;

            case NetOperationCode.MovePlayerCode:
                {
                    PlayerData playerData = MessageSerializerService.DeserializeObjectOfType<PlayerData>(reader.GetString());

                    if (OnMove != null)
                    {
                        Vector3 newPosition = new Vector3(playerData.X, playerData.Y, playerData.Z);

                        OnMove(playerData.IsMine, newPosition);

                        Debug.LogFormat("Player move. Id: {0} | New pos: {1}", playerData.Id, newPosition);
                    }
                }
                break;

            case NetOperationCode.DestroyPlayer:
                {
                    ParameterObject parameter = MessageSerializerService.DeserializeObjectOfType<ParameterObject>(reader.GetString());
                    long id = (long)parameter.Parameter;

                    if (_netObjects.ContainsKey(id))
                    {
                        Destroy(_netObjects[id].gameObject);
                        _netObjects.Remove(id);
                    }

                    Debug.LogFormat("Destroy player. PlayerId: {0}", id);
                }
                break;

            default:
                Debug.Log("Default handler");
                break;
        }
    }
    
    public void OnPeerConnected(NetPeer peer)
    {
        _serverPeer = peer;

        Debug.Log("Connected. EndPoint: " + peer.EndPoint);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log(string.Format("Disconnected. DisconnectReason: {0} | ErrorCode: {1}",
            disconnectInfo.Reason, disconnectInfo.SocketErrorCode));
    }

    #endregion

    public void SendOperation(NetDataWriter dataWriter, SendOptions sendOptions)
    {
        _serverPeer.Send(dataWriter, sendOptions);
    }
}
