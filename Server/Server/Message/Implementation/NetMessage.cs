using LiteNetLib.Utils;
using NetCommon.Codes;
using Server.Message.Interfaces;


namespace Server.Message.Implementation
{
    public class NetMessage : INetMessage
    {
        private readonly NetOperationCode _code;

        private readonly NetDataReader _reader;

        public NetOperationCode Code => _code;

        public NetDataReader Reader => _reader;

        public NetMessage (NetOperationCode code, NetDataReader reader)
        {
            _code = code;
            _reader = reader;
        }
    }
}
