using LiteNetLib.Utils;
using NetCommon.Codes;


namespace Server.Message.Interfaces
{
    public interface INetMessage
    {
        NetOperationCode Code { get; }

        NetDataReader Reader { get; }
    }
}
