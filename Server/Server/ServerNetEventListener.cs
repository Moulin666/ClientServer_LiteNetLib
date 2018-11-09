using LiteNetLib;
using NetCommon.Codes;
using Server.GameData;
using Server.Message.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;


namespace Server
{
    public class ServerNetEventListener : INetEventListener
    {
        public Dictionary<long, Client> ConnectedClients;

        public List<INetMessageHandler> NetMessageHandlerList { get; protected set; }

        private NetManager _netServer;

        private Thread _poolEventsThread;

        public ServerNetEventListener ()
        {
            _netServer = new NetManager(this);

            Console.WriteLine($"Server setup.");

            ConnectedClients = new Dictionary<long, Client>();
        }

        public void Start (int port)
        {
            _netServer.Start(port);
            _netServer.UpdateTime = 15;

            Console.WriteLine($"Server setup at port: {port}");

            _poolEventsThread = new Thread(PoolEventsUpdate)
            {
                Name = "PoolEventsThread",
                IsBackground = true
            };
            _poolEventsThread.Start();

            GatherMessageHandlers();

            // TODO : Create session by API server and get session id from API server.
            byte[] sessionId = new byte[12]
            {
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1
            };
            SessionCache.Instance.CreateSession(sessionId, 30000);
        }

        public void PoolEventsUpdate ()
        {
            while (_netServer.IsRunning)
            {
                _netServer.PollEvents();
                Thread.Sleep(15);
            }
        }

        public void Stop ()
        {
            foreach (var peer in ConnectedClients)
                _netServer.DisconnectPeer(peer.Value.NetPeer);

            Console.WriteLine(string.Format("Server stopping. Disconnected {0} peers.", ConnectedClients.Count));

            ConnectedClients.Clear();

            _netServer.Stop(true);

            _poolEventsThread.Join();
            _poolEventsThread = null;
        }

        public void GatherMessageHandlers ()
        {
            var handlers = from t in Assembly.GetAssembly(GetType()).GetTypes().Where(t => t.GetInterfaces()
                .Contains(typeof (INetMessageHandler)))
                select Activator.CreateInstance(t) as INetMessageHandler;

            Console.WriteLine(string.Format("Load handlers. Found {0} handlers.", handlers.Count()));

            NetMessageHandlerList = handlers.ToList();
        }

        #region Implements of INetEventListener

        public void OnNetworkLatencyUpdate (NetPeer peer, int latency) { }

        public void OnNetworkReceiveUnconnected (IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType) { }

        public void OnConnectionRequest (ConnectionRequest request) => request.AcceptIfKey("TestServer");

        public void OnNetworkError (IPEndPoint endPoint, SocketError socketErrorCode)
            =>
                Console.WriteLine(string.Format("Network Error. EndPoint: {0} | ErrorCode: {1}", endPoint,
                    socketErrorCode));

        public void OnPeerConnected (NetPeer peer)
        {
            Client newPeer = new Client(peer, this);

            //var serializeNewPeerPlayerData = MessageSerializerService.SerializeObjectOfType(newPeer.PlayerData);

            //_dataWriter.Reset();
            //_dataWriter.Put((byte)NetOperationCode.SpawnPlayerCode);
            //_dataWriter.Put(serializeNewPeerPlayerData);

            //foreach (var p in ConnectedClients)
            //    p.Value.NetPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);

            //if (ConnectedClients.Count > 0)
            //{
            //    _dataWriter.Reset();
            //    _dataWriter.Put((byte)NetOperationCode.SpawnPlayersCode);
            //    _dataWriter.Put(ConnectedClients.Count);

            //    foreach (var p in ConnectedClients)
            //        _dataWriter.Put(MessageSerializerService.SerializeObjectOfType(p.Value.PlayerData));

            //    peer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
            //}

            //serializeNewPeerPlayerData = MessageSerializerService.SerializeObjectOfType(newPeer.PlayerData);

            //_dataWriter.Reset();
            //_dataWriter.Put((byte)NetOperationCode.WorldEnter);
            //_dataWriter.Put(serializeNewPeerPlayerData);

            //peer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);

            //Console.WriteLine(string.Format("Connected peer. EndPoint: {0} | PeerId: {1}", peer.EndPoint, peer.Id));
        }

        public void OnPeerDisconnected (NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (ConnectedClients.ContainsKey(peer.Id))
                ConnectedClients[peer.Id].Disconnect(disconnectInfo);

            //_dataWriter.Reset();
            //_dataWriter.Put((byte)NetOperationCode.DestroyPlayer);
            //_dataWriter.Put(
            //    MessageSerializerService.SerializeObjectOfType(new ParameterObject(NetParameterCode.PlayerId, peer.Id)));

            //foreach (var p in ConnectedClients)
            //    p.Value.NetPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }

        public void OnNetworkReceive (NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            if (reader.RawData == null)
                return;

            Console.WriteLine($"OnNetworkReceive: {reader.RawData.Length}");

            if (ConnectedClients.ContainsKey(peer.Id))
            {
                NetOperationCode operationCode = (NetOperationCode)reader.GetByte();

                ConnectedClients[peer.Id].NetworkReceive(operationCode, reader, deliveryMethod);
            }

            //switch (parameterCode)
            //{
            //    case NetOperationCode.MovePlayerCode:
            //    {
            //        long id = reader.GetLong();

            //        if (ConnectedClients.ContainsKey(id))
            //        {
            //            var serializePositionData = reader.GetString();
            //            PositionData positionData =
            //                MessageSerializerService.DeserializeObjectOfType<PositionData>(serializePositionData);

            //            var player = ConnectedClients[id];
            //            player.PlayerData.PositionData = positionData;

            //            ConnectedClients[id] = player;

            //            _dataWriter.Reset();
            //            _dataWriter.Put((byte)NetOperationCode.MovePlayerCode);
            //            _dataWriter.Put(id);
            //            _dataWriter.Put(serializePositionData);

            //            foreach (var p in ConnectedClients)
            //                if (p.Value.NetPeer.Id != peer.Id)
            //                    p.Value.NetPeer.Send(_dataWriter, DeliveryMethod.Sequenced);

            //            Console.WriteLine(string.Format("Player move. Id: {0} | New pos: {1}, {2}, {3}",
            //                player.PlayerData.Id, player.PlayerData.PositionData.X, player.PlayerData.PositionData.Y,
            //                player.PlayerData.PositionData.Z));
            //        }
            //    }
            //        break;

            //    default:
            //        Console.WriteLine("Default handler");
            //        break;
            //}
        }

        #endregion
    }
}
