using LiteNetLib;
using NetCommon.Codes;
using NetCommon.MessageObjects;
using Server.GameData;
using Server.GameLogic.Session;
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

            // TODO : Create session by API server and get session data from API server.
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

            Dictionary<int, Unit> units = new Dictionary<int, Unit>()
            {
                {
                    0, new Unit(new UnitData(0, new PositionData(-3, 0.5f, 13), 100, 3, 5, 5, 10))
                },
                {
                    1, new Unit(new UnitData(1, new PositionData(-3, 0.5f, 15), 100, 3, 5, 5, 10))
                },
                {
                    2, new Unit(new UnitData(2, new PositionData(-3, 0.5f, 17), 100, 3, 5, 5, 10))
                },
                {
                    3, new Unit(new UnitData(3, new PositionData(-3, 0.5f, 19), 100, 3, 5, 5, 10))
                },
                {
                    4, new Unit(new UnitData(4, new PositionData(-3, 0.5f, 21), 100, 3, 5, 5, 10))
                },
                {
                    5, new Unit(new UnitData(5, new PositionData(-3, 0.5f, 23), 100, 3, 5, 5, 10))
                },

                {
                    6, new Unit(new UnitData(6, new PositionData(0, 0.5f, 13), 100, 3, 5, 5, 10))
                },
                {
                    7, new Unit(new UnitData(7, new PositionData(0, 0.5f, 15), 100, 3, 5, 5, 10))
                },
                {
                    8, new Unit(new UnitData(8, new PositionData(0, 0.5f, 17), 100, 3, 5, 5, 10))
                },
                {
                    9, new Unit(new UnitData(9, new PositionData(0, 0.5f, 19), 100, 3, 5, 5, 10))
                },
                {
                    10, new Unit(new UnitData(10, new PositionData(0, 0.5f, 21), 100, 3, 5, 5, 10))
                },
                {
                    11, new Unit(new UnitData(11, new PositionData(0, 0.5f, 23), 100, 3, 5, 5, 10))
                },
            };

            SessionCache.Instance.CreateSession(sessionId, units, 30000);
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

            //Console.WriteLine($"OnNetworkReceive: {reader.RawData.Length}");

            if (ConnectedClients.ContainsKey(peer.Id))
            {
                NetOperationCode operationCode = (NetOperationCode)reader.GetByte();

                ConnectedClients[peer.Id].NetworkReceive(operationCode, reader, deliveryMethod);
            }
        }

        #endregion
    }
}
