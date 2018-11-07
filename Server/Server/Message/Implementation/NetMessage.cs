using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon.Codes;
using Server.Message.Interfaces;


namespace Server.Message.Implementation
{
    public class NetMessage : INetMessage
    {
        private readonly NetOperationCode _code;

        private readonly Client _client;

        private readonly NetDataReader _reader;

        private readonly DeliveryMethod _deliveryMethod;

        public NetOperationCode Code => _code;

        public Client Client => _client;

        public NetDataReader Reader => _reader;

        public DeliveryMethod DeliveryMethod => _deliveryMethod;

        public NetMessage (NetOperationCode code, Client client, NetDataReader reader, DeliveryMethod deliveryMethod)
        {
            _code = code;
            _client = client;
            _reader = reader;
            _deliveryMethod = deliveryMethod;
        }
    }
}
