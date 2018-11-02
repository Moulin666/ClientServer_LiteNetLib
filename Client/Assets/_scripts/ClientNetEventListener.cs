using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using NetCommon.Codes;
using NetCommon;

public class ClientNetEventListener : MonoBehaviour, INetEventListener
{
    private NetDataWriter _dataWriter;
    private NetManager _netClient;
    private NetPeer _serverPeer;

    private readonly Dictionary<long, NetPeer> _peers = new Dictionary<long, NetPeer>();

    private readonly Dictionary<long, GameObject> _netObjects = new Dictionary<long, GameObject>();

    private void Start()
    {
        Debug.developerConsoleVisible = true;

        _dataWriter = new NetDataWriter();
        _netClient = new NetManager(this, "TestServer");

        if (_netClient.Start())
        {
            _netClient.Connect("localhost", 15000);
            Debug.Log("Client net manager started!");
        }
        else
            Debug.LogError("Could not start client net manager!");

        _netClient.UpdateTime = 15;
    }

    private void FixedUpdate()
    {
        if (_netClient != null && _netClient.IsRunning)
        {
            _netClient.PollEvents();
        }
    }

    private void OnApplicationQuit()
    {
        if (_netClient != null && _netClient.IsRunning)
            _netClient.Stop();
    }

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
                    PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"), GameObject.Find("Spawn").transform.position, Quaternion.identity))
                        .GetComponent<PlayerController>();

                    newPlayer.playerData = MessageSerializerService.DeserializeObjectOfType<PlayerData>(reader.GetString());

                    _netObjects.Add(newPlayer.playerData.Id, newPlayer.gameObject);

                    Debug.LogFormat("SpawnPlayer. PlayerId: {0}", newPlayer.playerData.Id);
                }
                break;

            case NetOperationCode.SpawnPlayersCode:
                {
                    int playerCount = reader.GetInt();
                    string[] playerArray = reader.GetStringArray();

                    Debug.Log(playerArray[0]);

                    foreach(var p in playerArray)
                    {
                        PlayerData playerData = MessageSerializerService.DeserializeObjectOfType<PlayerData>(p);

                        PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"), new Vector3(playerData.X, playerData.Y, playerData.Z), Quaternion.identity))
                            .GetComponent<PlayerController>();

                        newPlayer.playerData = playerData;

                        _netObjects.Add(playerData.Id, newPlayer.gameObject);
                    }

                    Debug.LogFormat("SpawnPlayers. Count: {0}", playerCount);
                }
                break;

            case NetOperationCode.WorldEnter:
                {
                    PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"), GameObject.Find("Spawn").transform.position, Quaternion.identity))
                        .GetComponent<PlayerController>();

                    newPlayer.playerData = MessageSerializerService.DeserializeObjectOfType<PlayerData>(reader.GetString());

                    _netObjects.Add(newPlayer.playerData.Id, newPlayer.gameObject);

                    Debug.LogFormat("WorldEnter. PlayerId: {0}", newPlayer.playerData.Id);
                }
                break;

            case NetOperationCode.MovePlayerCode:
                {
                    Debug.Log("Player move");
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
}
