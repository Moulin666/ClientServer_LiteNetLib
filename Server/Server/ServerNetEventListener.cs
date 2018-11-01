using LiteNetLib;
using LiteNetLib.Utils;
using Server.Codes;
using System;
using System.Collections.Generic;
using System.Threading;


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
            foreach(var p in _peers)
            {
                _dataWriter.Reset();
                _dataWriter.Put((byte)NetOperationCode.SpawnPlayerCode);
                _dataWriter.Put(peer.ConnectId);

                p.Value.NetPeer.Send(_dataWriter, SendOptions.Sequenced);
            }

            if (_peers.Count > 0)
            {
                _dataWriter.Reset();
                _dataWriter.Put((byte)NetOperationCode.SpawnPlayersCode);
                _dataWriter.Put(_peers.Count);

                peer.Send(_dataWriter, SendOptions.Sequenced);
            }

            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.WorldEnter);
            _dataWriter.Put(peer.ConnectId);

            peer.Send(_dataWriter, SendOptions.Sequenced);

            _peers.Add(peer.ConnectId, new NetPlayer(peer));

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
        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
        {
            Console.WriteLine(string.Format("NetworkReceive. EndPoint: {0} | Reader: {1}", peer.EndPoint, reader.GetString()));
        }

        #endregion
    }
}
