using LiteNetLib;
using NetCommon.Codes;
using Server.Message.Implementation;
using System;
using System.Linq;


namespace Server
{
    public class Client
    {
        public NetPeer NetPeer { get; set; }

        private ServerNetEventListener _server;

        public Client (NetPeer peer, ServerNetEventListener server)
        {
            NetPeer = peer;

            _server = server;
            _server.ConnectedClients.Add(NetPeer.Id, this);

            Console.WriteLine($"Connected new client. ClientId: {NetPeer.Id}");
        }

        public void Disconnect (DisconnectInfo disconnectInfo)
        {
            Console.WriteLine(string.Format("Disconnected peer. PeerId: {0} | EndPoint: {1} | DisconnectReason: {2}", NetPeer.Id, NetPeer.EndPoint,
                disconnectInfo.Reason));

            _server.ConnectedClients.Remove(NetPeer.Id);
        }

        public void NetworkReceive (NetOperationCode operationCode, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var message = new NetMessage(operationCode, this, reader, deliveryMethod);
            var handlers = _server.NetMessageHandlerList.Where(h => (byte)h.Code == (byte)message.Code);

            if (handlers == null || handlers.Count() == 0)
                Console.WriteLine("Default message handler: {0}", operationCode);

            foreach (var handler in handlers)
                handler.HandleMessage(message);
        }
    }
}
