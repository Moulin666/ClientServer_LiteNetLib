using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using NetCommon;
using NetCommon.Codes;
using NetCommon.MessageObjects;
using System.Net;
using System.Net.Sockets;


namespace Server
{
    public class ServerNetEventListener : INetEventListener
    {
        private NetManager _netServer;
        private NetDataWriter _dataWriter;

        private Thread _poolEventsThread;

        private Dictionary<long, NetPlayer> _peers;

        public ServerNetEventListener()
        {
            _dataWriter = new NetDataWriter();
            _netServer = new NetManager(this);

            Console.WriteLine($"Server setup.");

            _peers = new Dictionary<long, NetPlayer>();
        } 

        public void Start (int port)
        {
            _netServer.Start(port);
            _netServer.UpdateTime = 15;

            Console.WriteLine($"Server setup at port: {port}");

            _poolEventsThread = new Thread(PoolEventsUpdate) { Name = "PoolEventsThread", IsBackground = true };
            _poolEventsThread.Start();
        }

        public void PoolEventsUpdate ()
        {
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

            _peers.Clear();

            _poolEventsThread.Join();
            _poolEventsThread = null;

            _netServer.Stop();
        }

        #region Implements of INetEventListener

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }

        public void OnConnectionRequest (ConnectionRequest request)
        {
            request.AcceptIfKey("TestServer");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode) => Console.WriteLine(string.Format("Network Error. EndPoint: {0} | ErrorCode: {1}", endPoint, socketErrorCode));

        public void OnPeerConnected(NetPeer peer)
        {
            NetPlayer newPeer = new NetPlayer(peer);

            var serializeNewPeerPlayerData = MessageSerializerService.SerializeObjectOfType(newPeer.PlayerData);

            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.SpawnPlayerCode);
            _dataWriter.Put(serializeNewPeerPlayerData);

            foreach (var p in _peers)
                p.Value.NetPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);

            if (_peers.Count > 0)
            {
                _dataWriter.Reset();
                _dataWriter.Put((byte)NetOperationCode.SpawnPlayersCode);
                _dataWriter.Put(_peers.Count);

                foreach (var p in _peers)
                    _dataWriter.Put(MessageSerializerService.SerializeObjectOfType(p.Value.PlayerData));

                peer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
            }

            serializeNewPeerPlayerData = MessageSerializerService.SerializeObjectOfType(newPeer.PlayerData);

            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.WorldEnter);
            _dataWriter.Put(serializeNewPeerPlayerData);

            peer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);

            _peers.Add(peer.Id, newPeer);

            Console.WriteLine(string.Format("Connected peer. EndPoint: {0} | PeerId: {1}", peer.EndPoint, peer.Id));
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine(string.Format("Disconnected peer. EndPoint: {0} | DisconnectReason: {1}", peer.EndPoint, disconnectInfo.Reason));

            if (_peers.ContainsKey(peer.Id))
            {
                _peers.Remove(peer.Id);
                Console.WriteLine(string.Format("(Peer {0}): Disconnected", peer.Id));
            }

            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.DestroyPlayer);
            _dataWriter.Put(MessageSerializerService.SerializeObjectOfType(new ParameterObject(NetParameterCode.PlayerId, peer.Id)));

            foreach (var p in _peers)
                p.Value.NetPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            if (reader.RawData == null)
                return;

            Console.WriteLine($"OnNetworkReceive: {reader.RawData.Length}");

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
                                if (p.Value.NetPeer.Id != peer.Id)
                                    p.Value.NetPeer.Send(_dataWriter, DeliveryMethod.Sequenced);

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
