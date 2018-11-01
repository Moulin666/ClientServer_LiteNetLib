using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using Server;
using Server.Codes;


public class ClientNetEventListener : MonoBehaviour, INetEventListener
{
    private NetDataWriter _dataWriter;
    private NetManager _netClient;
    private NetPeer _serverPeer;

    private Dictionary<long, NetPlayer> _peers;

    private void Start()
    {
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

        byte operation = reader.GetByte();

        switch(operation)
        {
            case (byte)NetOperationCode.SpawnPlayerCode:
                {
                    PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"), GameObject.Find("Spawn").transform.position, Quaternion.identity))
                        .GetComponent<PlayerController>();
                    newPlayer.isMine = false;

                    Debug.LogFormat("Spawn new player. PlayerId: {0}", reader.GetLong());
                }
                break;

            case (byte)NetOperationCode.SpawnPlayersCode:
                {
                    Debug.LogFormat("Spawn players operation. PlayerCount: {0}", reader.GetInt());
                }
                break;

            case (byte)NetOperationCode.WorldEnter:
                {
                    PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"), GameObject.Find("Spawn").transform.position, Quaternion.identity))
                        .GetComponent<PlayerController>();
                    newPlayer.isMine = true;
                }
                break;

            case (byte)NetOperationCode.MovePlayerCode:
                {
                    Debug.Log("Player move");
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
