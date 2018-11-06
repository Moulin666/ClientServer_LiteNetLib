using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using NetCommon;
using NetCommon.Codes;
using NetCommon.MessageObjects;


namespace Server
{
    public class ServerNetEventListener : INetEventListener
    {
        private NetManager _netServer;
        private NetDataWriter _dataWriter;

        private Dictionary<long, NetPlayer> _peers;

        public ServerNetEventListener(int maxConnections, string connectKey)
        {
            _dataWriter = new NetDataWriter();
            _netServer = new NetManager(this, maxConnections, connectKey);

            Console.WriteLine($"Server setup. MaxConnections: {maxConnections}, ConnectKey: {connectKey}");

            _peers = new Dictionary<long, NetPlayer>();
        } 

        public void Start (int port)
        {
            _netServer.Start(port);
            _netServer.UpdateTime = 15;

            Console.WriteLine($"Server setup at port: {port}");

            while (_netServer.IsRunning)
            {
                _netServer.PollEvents();
                Thread.Sleep(15);
            }

            Stop();
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

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode) => Console.WriteLine(string.Format("Network Error. EndPoint: {0} | ErrorCode: {1}", endPoint, socketErrorCode));

        public void OnPeerConnected(NetPeer peer)
        {
            NetPlayer newPeer = new NetPlayer(peer);

            var serializeNewPeerPlayerData = MessageSerializerService.SerializeObjectOfType(newPeer.PlayerData);

            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.SpawnPlayerCode);
            _dataWriter.Put(serializeNewPeerPlayerData);

            foreach (var p in _peers)
                p.Value.NetPeer.Send(_dataWriter, SendOptions.ReliableOrdered);

            if (_peers.Count > 0)
            {
                _dataWriter.Reset();
                _dataWriter.Put((byte)NetOperationCode.SpawnPlayersCode);
                _dataWriter.Put(_peers.Count);

                foreach (var p in _peers)
                    _dataWriter.Put(MessageSerializerService.SerializeObjectOfType(p.Value.PlayerData));

                peer.Send(_dataWriter, SendOptions.ReliableOrdered);
            }

            serializeNewPeerPlayerData = MessageSerializerService.SerializeObjectOfType(newPeer.PlayerData);

            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.WorldEnter);
            _dataWriter.Put(serializeNewPeerPlayerData);

            peer.Send(_dataWriter, SendOptions.ReliableOrdered);

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
            if (reader.Data == null)
                return;

            Console.WriteLine($"OnNetworkReceive: {reader.Data.Length}");

            NetOperationCode operationCode = (NetOperationCode)reader.GetByte();

            switch (operationCode)
            {
                case NetOperationCode.MovePlayerCode:
                    {
                        long id = reader.GetLong();

                        if (_peers.ContainsKey(id))
                        {
                            var serializePositionData = reader.GetString();
                            PositionData positionData = MessageSerializerService.DeserializeObjectOfType<PositionData>(serializePositionData);

                            var player = _peers[id];
                            player.PlayerData.PositionData = positionData;

                            _peers[id] = player;

                            _dataWriter.Reset();
                            _dataWriter.Put((byte)NetOperationCode.MovePlayerCode);
                            _dataWriter.Put(id);
                            _dataWriter.Put(serializePositionData);

                            foreach (var p in _peers)
                                if (p.Value.NetPeer.ConnectId != peer.ConnectId)
                                    p.Value.NetPeer.Send(_dataWriter, SendOptions.Sequenced);

                            Console.WriteLine(string.Format("Player move. Id: {0} | New pos: {1}, {2}, {3}",
                                player.PlayerData.Id, player.PlayerData.PositionData.X, player.PlayerData.PositionData.Y, player.PlayerData.PositionData.Z));
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Default handler");
                    break;
            }
        }

        #endregion
    }
}
