using LiteNetLib.Utils;
using NetCommon.Codes;


namespace Server.Message.Interfaces
{
    public interface INetMessageHandler
    {
        NetOperationCode Code { get; }

        NetDataReader Reader { get; }

        bool HandleMessage(INetMessage message);
    }
}
