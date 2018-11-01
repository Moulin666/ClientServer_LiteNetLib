using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using NetCommon;
using NetCommon.Codes;


public class ClientNetEventListener : MonoBehaviour, INetEventListener
{
    private NetDataWriter _dataWriter;
    private NetManager _netClient;
    private NetPeer _serverPeer;

    private readonly Dictionary<long, NetPeer> _peers;

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

        NetOperationCode operationCode = (NetOperationCode)reader.GetByte();

        switch(operationCode)
        {
            case NetOperationCode.SpawnPlayerCode:
                {
                    Debug.LogFormat("SpawnPlayer");
                }
                break;

            case NetOperationCode.SpawnPlayersCode:
                {
                    Debug.Log("SpawnPlayers");
                }
                break;

            case NetOperationCode.WorldEnter:
                {
                    Debug.Log("WorldEnter");
                }
                break;

            case NetOperationCode.MovePlayerCode:
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
