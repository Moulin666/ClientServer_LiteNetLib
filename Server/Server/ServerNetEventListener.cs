using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using NetCommon;
using NetCommon.Codes;


namespace Server
{
    public class ServerNetEventListener : INetEventListener
    {
        private NetManager _netServer;
        private NetDataWriter _dataWriter;

        private Dictionary<long, NetPlayer> _peers;

        public ServerNetEventListener()
        {
            _dataWriter = new NetDataWriter();
            _netServer = new NetManager(this, 100, "TestServer");
            _netServer.Start(15000);
            _netServer.UpdateTime = 15;

            _peers = new Dictionary<long, NetPlayer>();

            Console.WriteLine("Server setup.");

            while (_netServer.IsRunning)
            {
                _netServer.PollEvents();
                Thread.Sleep(15);
            }
        }

        public void Stop()
        {
            foreach (var peer in _peers)
                _netServer.DisconnectPeer(peer.Value.NetPeer);

            Console.WriteLine(string.Format("Server stopping. Disconnected {0} peers.", _peers.Count));

            _netServer.Stop();
        }

        #region Implements of INetEventListener

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) { }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
        {
            Console.WriteLine(string.Format("Network Error. EndPoint: {0} | ErrorCode: {1}", endPoint, socketErrorCode));
        }

        public void OnPeerConnected(NetPeer peer)
        {
            NetPlayer newPeer = new NetPlayer(peer);

            var serializeNewPeer = MessageSerializerService.SerializeObjectOfType(new PlayerData(peer.ConnectId));

            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.SpawnPlayerCode);
            _dataWriter.Put(serializeNewPeer);

            foreach (var p in _peers)
                p.Value.NetPeer.Send(_dataWriter, SendOptions.ReliableOrdered);

            if (_peers.Count > 0)
            {
                _dataWriter.Reset();
                _dataWriter.Put((byte)NetOperationCode.SpawnPlayersCode);
                _dataWriter.Put(MessageSerializerService.SerializeObjectOfType(new ParameterObject(NetParameterCode.CountOfPlayer, _peers.Count)));

                string[] playerArray = new string[_peers.Count];

                int i = 0;
                foreach (var p in _peers)
                {
                    playerArray[i] = MessageSerializerService.SerializeObjectOfType(p.Value);
                    i++;
                }

                _dataWriter.PutArray(playerArray);

                peer.Send(_dataWriter, SendOptions.ReliableOrdered);
            }

            serializeNewPeer = MessageSerializerService.SerializeObjectOfType(new PlayerData(peer.ConnectId, true));

            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.WorldEnter);
            _dataWriter.Put(serializeNewPeer);

            peer.Send(_dataWriter, SendOptions.Sequenced);

            _peers.Add(peer.ConnectId, newPeer);

            Console.WriteLine(string.Format("Connected peer. EndPoint: {0} | PeerId: {1}", peer.EndPoint, peer.ConnectId));
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine(string.Format("Disconnected peer. EndPoint: {0} | DisconnectReason: {1}", peer.EndPoint, disconnectInfo.Reason));

            if (_peers.ContainsKey(peer.ConnectId))
            {
                _peers.Remove(peer.ConnectId);
                Console.WriteLine(string.Format("(Peer{0}): Disconnected", peer.ConnectId));
            }

            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.DestroyPlayer);
            _dataWriter.Put(MessageSerializerService.SerializeObjectOfType(new ParameterObject(NetParameterCode.PlayerId, peer.ConnectId)));

            foreach (var p in _peers)
                p.Value.NetPeer.Send(_dataWriter, SendOptions.ReliableOrdered);
        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
        {
            Console.WriteLine(string.Format("NetworkReceive. EndPoint: {0} | Reader: {1}", peer.EndPoint, reader.GetString()));
        }

        #endregion
    }
}
